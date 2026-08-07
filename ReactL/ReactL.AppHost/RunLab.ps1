param(
    [switch]$Teardown
)

$networkName = "reactorlabnet"

if ($Teardown)
{
    docker rm -f reacts reactorlab 2>$null

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

docker rm -f reacts reactorlab 2>$null

docker run -d --name reactorlab --network $networkName -p 8081:8081 -p 8082:8082 -p 8083:8083 -p 8084:8084 -p 8085:8085 -p 8086:8086 -p 8087:8087 -p 8088:8088 -p 8089:8089 -p 8090:8090 -p 8091:8091 -p 8092:8092 kusdepotreactorlab

docker run -d --name reacts --network $networkName -p 8080:8080 -e "services__reactf__F-HTTP__0=http://reactorlab:8081" -e "services__reactf__F-GRPC__0=http://reactorlab:8082" -e "services__reactg__G-HTTP__0=http://reactorlab:8083" -e "services__reactg__G-GRPC__0=http://reactorlab:8084" -e "services__reactj__J-HTTP__0=http://reactorlab:8085" -e "services__reactj__J-GRPC__0=http://reactorlab:8086" -e "services__reactn__N-HTTP__0=http://reactorlab:8087" -e "services__reactn__N-GRPC__0=http://reactorlab:8088" -e "services__reactp__P-HTTP__0=http://reactorlab:8089" -e "services__reactp__P-GRPC__0=http://reactorlab:8090" -e "services__reactr__R-HTTP__0=http://reactorlab:8091" -e "services__reactr__R-GRPC__0=http://reactorlab:8092" kusdepotreacts:latest
