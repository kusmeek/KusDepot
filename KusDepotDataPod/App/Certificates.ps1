using namespace System.Security.Cryptography
using namespace System.Security.Cryptography.X509Certificates

param(
	[string]$Password = "server"
)

$ErrorActionPreference = 'Stop'

$scriptRoot = Split-Path -Parent $PSCommandPath
$dataPodHome = Join-Path $scriptRoot 'DataPodHome'
$certificatesRoot = Join-Path $dataPodHome 'certificates'
$rootDirectory = Join-Path $certificatesRoot 'root'
$catalogDirectory = Join-Path $certificatesRoot 'catalog'
$dataControlDirectory = Join-Path $certificatesRoot 'datacontrol'

$securePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force
$localhostSan = '2.5.29.17={text}DNS=localhost&IPAddress=127.0.0.1&IPAddress=::1'
$serverAuthenticationEku = '2.5.29.37={text}1.3.6.1.5.5.7.3.1'
$clientAuthenticationEku = '2.5.29.37={text}1.3.6.1.5.5.7.3.2'
$certificateAuthorityConstraints = '2.5.29.19={critical}{text}ca=1&pathlength=1'

foreach($directory in @($rootDirectory,$catalogDirectory,$dataControlDirectory))
{
	New-Item -ItemType Directory -Path $directory -Force | Out-Null
}

function Remove-CertificateBySubject
{
	param(
		[Parameter(Mandatory=$true)][StoreLocation]$StoreLocation,
		[Parameter(Mandatory=$true)][StoreName]$StoreName,
		[Parameter(Mandatory=$true)][string]$SubjectName,
		[switch]$IgnoreAccessDenied
	)	

	$store = [X509Store]::new($StoreName,$StoreLocation)
	$store.Open([OpenFlags]::ReadWrite)

	try
	{
		$matches = $store.Certificates.Find([X509FindType]::FindBySubjectName,$SubjectName,$false)

		foreach($certificate in @($matches))
		{
			try
			{
				$store.Remove($certificate)
			}
			catch [System.Security.Cryptography.CryptographicException]
			{
				if(-not $IgnoreAccessDenied) { throw }

				Write-Warning "Skipping removal of $($certificate.Subject) from $StoreLocation\\$StoreName because access was denied."
			}
		}
	}
	finally
	{
		$store.Close()
	}
}

function Export-CertificatePem
{
	param(
		[Parameter(Mandatory=$true)][X509Certificate2]$Certificate,
		[Parameter(Mandatory=$true)][string]$Path
	)

	$bytes = $Certificate.Export([X509ContentType]::Cert)
	$base64 = [Convert]::ToBase64String($bytes,[Base64FormattingOptions]::InsertLineBreaks)
	$content = "-----BEGIN CERTIFICATE-----`r`n$base64`r`n-----END CERTIFICATE-----`r`n"

	[System.IO.File]::WriteAllText($Path,$content,[System.Text.Encoding]::ASCII)
}

function Remove-CertificateByThumbprint
{
	param(
		[Parameter(Mandatory=$true)][StoreLocation]$StoreLocation,
		[Parameter(Mandatory=$true)][StoreName]$StoreName,
		[Parameter(Mandatory=$true)][string]$Thumbprint,
		[switch]$IgnoreAccessDenied
	)

	$store = [X509Store]::new($StoreName,$StoreLocation)
	$store.Open([OpenFlags]::ReadWrite)

	try
	{
		$matches = $store.Certificates.Find([X509FindType]::FindByThumbprint,$Thumbprint,$false)

		foreach($certificate in @($matches))
		{
			try
			{
				$store.Remove($certificate)
			}
			catch [System.Security.Cryptography.CryptographicException]
			{
				if(-not $IgnoreAccessDenied) { throw }

				Write-Warning "Skipping removal of thumbprint $Thumbprint from $StoreLocation\\$StoreName because access was denied."
			}
		}
	}
	finally
	{
		$store.Close()
	}
}

function Import-CertificateIfMissing
{
	param(
		[Parameter(Mandatory=$true)][string]$FilePath,
		[Parameter(Mandatory=$true)][StoreLocation]$StoreLocation,
		[Parameter(Mandatory=$true)][StoreName]$StoreName
	)

	$certificate = [X509Certificate2]::new($FilePath)
	$store = [X509Store]::new($StoreName,$StoreLocation)
	$store.Open([OpenFlags]::ReadWrite)

	try
	{
		$matches = $store.Certificates.Find([X509FindType]::FindByThumbprint,$certificate.Thumbprint,$false)

		if($matches.Count -eq 0)
		{
			Import-Certificate -FilePath $FilePath -CertStoreLocation ("Cert:\{0}\{1}" -f $StoreLocation,$StoreName) | Out-Null
		}
	}
	finally
	{
		$store.Close()
		$certificate.Dispose()
	}
}

function New-PolicyExtension
{
	param([Parameter(Mandatory=$true)][string]$PolicyOid)

	$oid = New-Object Oid($PolicyOid)
	$asn = New-Object AsnEncodedData($oid,[Byte[]]@())

	return [X509Extension]::new($asn,$false)
}

function New-ServerCertificate
{
	param(
		[Parameter(Mandatory=$true)][string]$Subject,
		[Parameter(Mandatory=$true)][X509Certificate2]$Signer
	)

	return New-SelfSignedCertificate `
		-Subject $Subject `
		-CertStoreLocation 'Cert:\LocalMachine\My' `
		-Signer $Signer `
		-Type SSLServerAuthentication `
		-KeyExportPolicy Exportable `
		-KeyAlgorithm RSA `
		-KeyLength 2048 `
		-HashAlgorithm SHA256 `
		-KeyUsage DigitalSignature , KeyEncipherment `
		-TextExtension @($localhostSan,$serverAuthenticationEku) `
		-NotAfter (Get-Date).AddYears(2)
}

function New-ClientCertificate
{
	param(
		[Parameter(Mandatory=$true)][string]$Subject,
		[Parameter(Mandatory=$true)][string]$PolicyOid,
		[Parameter(Mandatory=$true)][X509Certificate2]$Signer
	)

	return New-SelfSignedCertificate `
		-Subject $Subject `
		-CertStoreLocation 'Cert:\CurrentUser\My' `
		-Signer $Signer `
		-Type Custom `
		-KeyExportPolicy Exportable `
		-KeyAlgorithm RSA `
		-KeyLength 2048 `
		-HashAlgorithm SHA256 `
		-KeyUsage DigitalSignature `
		-TextExtension @($clientAuthenticationEku) `
		-Extension @((New-PolicyExtension -PolicyOid $PolicyOid)) `
		-NotAfter (Get-Date).AddYears(2)
}

function Export-CertificateSet
{
	param(
		[Parameter(Mandatory=$true)][X509Certificate2]$Certificate,
		[Parameter(Mandatory=$true)][string]$Path,
		[Parameter(Mandatory=$true)][SecureString]$Password
	)

	if(Test-Path $Path) { Remove-Item $Path -Force }

	Export-PfxCertificate -Cert $Certificate -FilePath $Path -Password $Password | Out-Null
}

foreach($subject in @('Simulated Root CA','KusDepot.Data.Catalog','CatalogUser','KusDepot.Data.Control','DataControlUser'))
{
	Remove-CertificateBySubject -StoreLocation CurrentUser -StoreName My -SubjectName $subject -IgnoreAccessDenied
	Remove-CertificateBySubject -StoreLocation LocalMachine -StoreName My -SubjectName $subject -IgnoreAccessDenied
	Remove-CertificateBySubject -StoreLocation CurrentUser -StoreName Root -SubjectName $subject -IgnoreAccessDenied
	Remove-CertificateBySubject -StoreLocation LocalMachine -StoreName Root -SubjectName $subject -IgnoreAccessDenied
}

$rca = New-SelfSignedCertificate -Subject 'Simulated Root CA' -CertStoreLocation 'Cert:\CurrentUser\My' -Type Custom -KeyExportPolicy Exportable -KeyAlgorithm RSA -KeyLength 2048 -HashAlgorithm SHA256 -KeyUsage CertSign , CRLSign -TextExtension @($certificateAuthorityConstraints) -NotAfter (Get-Date).AddYears(5)

$clscert = New-ServerCertificate -Subject 'KusDepot.Data.Catalog' -Signer $rca

$clccert = New-ClientCertificate -Subject 'CatalogUser' -PolicyOid '2.5.29.32.853207195657' -Signer $rca

$dcscert = New-ServerCertificate -Subject 'KusDepot.Data.Control' -Signer $rca

$dcccert = New-ClientCertificate -Subject 'DataControlUser' -PolicyOid '2.5.29.32.853207196923' -Signer $rca

$rootFilePath = Join-Path $rootDirectory 'root.cer'

if(Test-Path $rootFilePath) { Remove-Item $rootFilePath -Force }

Export-CertificatePem -Certificate $rca -Path $rootFilePath

Import-CertificateIfMissing -FilePath $rootFilePath -StoreLocation CurrentUser -StoreName Root
Import-CertificateIfMissing -FilePath $rootFilePath -StoreLocation LocalMachine -StoreName Root

Export-CertificateSet -Certificate $clscert -Path (Join-Path $catalogDirectory 'server.pfx') -Password $securePassword
Export-CertificateSet -Certificate $clccert -Path (Join-Path $catalogDirectory 'client.pfx') -Password $securePassword
Export-CertificateSet -Certificate $dcscert -Path (Join-Path $dataControlDirectory 'server.pfx') -Password $securePassword
Export-CertificateSet -Certificate $dcccert -Path (Join-Path $dataControlDirectory 'client.pfx') -Password $securePassword

Remove-CertificateByThumbprint -StoreLocation CurrentUser -StoreName My -Thumbprint $rca.Thumbprint -IgnoreAccessDenied

Write-Host "Generated DataPod certificates in $certificatesRoot"
