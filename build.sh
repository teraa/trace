#!/bin/bash
set -e

echo "Building"
dotnet publish Trace \
  -c Release -r linux-x64 --self-contained false -o bin/temp/ \
  -p:GenerateDocumentationFile=false \
  -p:PublishSingleFile=true \
  -p:DebugType=None \
  -p:UseSharedCompilation=false \
  -nodeReuse:false
