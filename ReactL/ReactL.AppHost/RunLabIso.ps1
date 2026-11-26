param(
    [switch]$Teardown
)

$networkName = "reactsnet"
$containerNames = @("reacts", "reactf", "reactg", "reactj", "reactn", "reactp", "reactr")

if ($Teardown)
{
    docker rm -f $containerNames 2>$null

    docker network inspect $networkName > $null 2>&1

    if ($LASTEXITCODE -eq 0)
    {
        docker network rm $networkName
    }

    return
}

docker network inspect $networkName > $null 2>&1

if ($LASTEXITCODE -ne 0)
{
    docker network create $networkName | Out-Null
}

docker rm -f $containerNames 2>$null

docker run -d --name reactf --network $networkName -p 8081:8081 -p 8082:8082 kusdepotreactf:latest
docker run -d --name reactg --network $networkName -p 8083:8083 -p 8084:8084 kusdepotreactg:latest
docker run -d --name reactj --network $networkName -p 8085:8085 -p 8086:8086 kusdepotreactj:latest
docker run -d --name reactn --network $networkName -p 8087:8087 -p 8088:8088 kusdepotreactn:latest
docker run -d --name reactp --network $networkName -p 8089:8089 -p 8090:8090 kusdepotreactp:latest
docker run -d --name reactr --network $networkName -p 8091:8091 -p 8092:8092 kusdepotreactr:latest

docker run -d --name reacts --network $networkName -p 8080:8080 -e "services__reactf__F-HTTP__0=http://reactf:8081" -e "services__reactf__F-GRPC__0=http://reactf:8082" -e "services__reactg__G-HTTP__0=http://reactg:8083" -e "services__reactg__G-GRPC__0=http://reactg:8084" -e "services__reactj__J-HTTP__0=http://reactj:8085" -e "services__reactj__J-GRPC__0=http://reactj:8086" -e "services__reactn__N-HTTP__0=http://reactn:8087" -e "services__reactn__N-GRPC__0=http://reactn:8088" -e "services__reactp__P-HTTP__0=http://reactp:8089" -e "services__reactp__P-GRPC__0=http://reactp:8090" -e "services__reactr__R-HTTP__0=http://reactr:8091" -e "services__reactr__R-GRPC__0=http://reactr:8092" kusdepotreacts:latest