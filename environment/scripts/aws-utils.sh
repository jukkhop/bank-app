#!/bin/bash

set -euo pipefail

function get_rds_endpoint_address {
  local DB_HOST="$( \
    aws rds describe-db-instances \
    --db-instance-identifier bank-db-${1} \
    --query 'DBInstances[*].[Endpoint.Address]' \
    --output text \
  )"

  echo "$DB_HOST"
}
