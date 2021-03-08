#!/bin/bash

set -euo pipefail

cd "$(dirname "$0")"

[ -z "${1-}" ] && (
  echo "Usage: $0 [env]"
  exit 128
)

env="${1}"

set -o allexport

source "../environment/${env}-secrets.env"
source "../environment/${env}-variables.env"

set +o allexport

export AWS_ACCESS_KEY_ID="${TF_VAR_aws_access_key}"
export AWS_SECRET_ACCESS_KEY="${TF_VAR_aws_secret_key}"

source "../environment/scripts/aws-utils.sh"

db_host="$(get_rds_endpoint_address ${env})"

serverless deploy \
  --region ${TF_VAR_aws_region} \
  --stage ${TF_VAR_environment} \
  --postgres-host ${db_host} \
  --postgres-port ${TF_VAR_db_port} \
  --postgres-user ${TF_VAR_db_user} \
  --postgres-password ${TF_VAR_db_password} \
  --postgres-database ${TF_VAR_db_database} \
  --verbose
