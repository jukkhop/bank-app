#!/bin/bash

set -euo pipefail

function get_rds_endpoint_address {
  local DB_HOST="$( \
    aws rds describe-db-instances \
    --db-instance-identifier ${1} \
    --query 'DBInstances[*].[Endpoint.Address]' \
    --output text \
  )"

  echo "$DB_HOST"
}

function get_cloudfront_distribution_id {
  local DISTRIBUTION_ID="$( \
    aws cloudfront list-distributions \
    --query 'DistributionList.Items[*].{id:Id,origin:Origins.Items[0].Id}[?origin=='"'"${1}"'"'].id' \
    --output text \
  )"

  echo "$DISTRIBUTION_ID"
}