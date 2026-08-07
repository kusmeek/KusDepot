#!/bin/sh
dotnet /app/reactf/KusDepot.ReactF.dll &
/app/kusdepot.reactg &
java --enable-native-access=ALL-UNNAMED -jar /app/ShapeAPI.jar &
node /app/reactn/target/StartUp.js &
/app/venv/bin/python -m reactp.main &
/app/KusDepot-ReactR &
wait