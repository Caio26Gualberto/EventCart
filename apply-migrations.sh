#!/bin/bash

services=("order-service" "payment-service" "inventory-service")

for service in "${services[@]}"
do
  echo "Applying migrations for $service..."

  if [ ! -f "./$service/$service.csproj" ]; then
    echo "Project not found for $service, skipping..."
    continue
  fi

  dotnet ef database update \
    --project "./$service/$service.csproj" \
    --startup-project "./$service/$service.csproj"

  if [ $? -ne 0 ]; then
    echo "Error updating $service. Stopping script."
    exit 1
  fi

  echo "Done for $service"
  echo "-----------------------------"
done

echo "All databases updated."