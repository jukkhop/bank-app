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

source "../environment/scripts/aws-utils.sh"

db_host="$(get_rds_endpoint_address bank-app-${env}-db)"

flyway \
  -url="jdbc:postgresql://${db_host}:${TF_VAR_db_port}/${TF_VAR_db_database}" \
  -user="${TF_VAR_db_user}" \
  -password="${TF_VAR_db_password}" \
  migrate
