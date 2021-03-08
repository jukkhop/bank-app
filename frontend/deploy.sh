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

bucket_name="${TF_VAR_frontend_dns_name}"
distribution_id="$(get_cloudfront_distribution_id ${bucket_name})"

s3deploy \
  -source=build/ \
  -region=${TF_VAR_aws_region} \
  -key=${TF_VAR_aws_access_key} \
  -secret=${TF_VAR_aws_secret_key} \
  -distribution-id=${distribution_id} \
  -bucket=${bucket_name}
