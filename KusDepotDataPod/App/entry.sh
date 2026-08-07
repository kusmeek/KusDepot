#!/bin/bash
set -euo pipefail

dotnet /app/services/KusDepotDataPod.Services.dll &
services_pid=$!

dotnet /app/catalog/KusDepotDataPod.Catalog.dll &
catalog_pid=$!

dotnet /app/datacontrol/KusDepotDataPod.Control.dll &
datacontrol_pid=$!

cleanup() {
	kill "$services_pid" "$catalog_pid" "$datacontrol_pid" 2>/dev/null || true
}

trap cleanup INT TERM

wait -n "$services_pid" "$catalog_pid" "$datacontrol_pid"
status=$?

cleanup
wait || true

exit "$status"
