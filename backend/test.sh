#!/bin/bash

set -euo pipefail

cd "$(dirname "$0")/tests"

dotnet test
