#!/bin/bash

dotnet restore
dotnet tool install -g Amazon.Lambda.Tools --framework netcoreapp3.1
dotnet lambda package --configuration Debug --framework netcoreapp3.1
