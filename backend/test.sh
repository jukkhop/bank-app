#!/bin/bash

set -euo pipefail

cd "$(dirname "$0")/tests"

dotnet test --logger "console;verbosity=detailed"
