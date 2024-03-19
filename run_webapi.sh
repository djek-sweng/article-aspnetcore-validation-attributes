#!/bin/sh

cd "./src/CustomValidationAttributes"

CSPROJ="CustomValidationAttributes.WebApi/CustomValidationAttributes.WebApi.csproj"

dotnet run --project "$CSPROJ" -c Release
