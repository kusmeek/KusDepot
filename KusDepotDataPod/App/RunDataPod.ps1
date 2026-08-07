param(
	[switch]$Teardown,
	[string]$CatalogCertificatePassword = "server",
	[string]$DataControlCertificatePassword = "server"
)

$networkName = "kusdepotdatapodnet"
$catalogDbContainerName = "catalogdb"
$dataPodContainerName = "kusdepotdatapod"
$catalogDbImage = "kusdepotcatalogdb"
$dataPodImage = "kusdepotdatapod"

if ($Teardown)
{
	docker rm -f $catalogDbContainerName $dataPodContainerName 2>$null

	docker network inspect $networkName > $null 2>&1

	if ($LASTEXITCODE -eq 0)
	{
		docker network rm $networkName | Out-Null
	}

	return
}

docker network inspect $networkName > $null 2>&1

if ($LASTEXITCODE -ne 0)
{
	docker network create $networkName | Out-Null
}

#docker rm -f $catalogDbContainerName $dataPodContainerName 2>$null

docker run -d --name $catalogDbContainerName --network $networkName -p 5432:5432 $catalogDbImage

docker run -d --name $dataPodContainerName --network $networkName -p 5006:5006 -p 5007:5007 -p 11115:11115 -p 30003:30003 -e "KUSDEPOT_DATAPOD_CATALOG_CERTIFICATE_PASSWORD=$CatalogCertificatePassword" -e "KUSDEPOT_DATAPOD_DATACONTROL_CERTIFICATE_PASSWORD=$DataControlCertificatePassword" $dataPodImage
