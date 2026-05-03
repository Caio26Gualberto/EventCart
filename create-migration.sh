#!/bin/bash

services=("order-service" "payment-service" "inventory-service" "catalog-service")

echo "Select a service:"
select service in "${services[@]}"
do
  if [[ -n "$service" ]]; then
    echo "Selected: $service"
    break
  else
    echo "Invalid option, try again."
  fi
done

read -p "Enter migration name: " migrationName

if [[ -z "$migrationName" ]]; then
  echo "Migration name cannot be empty."
  exit 1
fi

echo "Creating migration '$migrationName' for $service..."

dotnet ef migrations add "$migrationName" \
  --project "./$service/$service.csproj" \
  --startup-project "./$service/$service.csproj"

echo "Done."