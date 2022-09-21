#!/bin/bash

echo "Building"
dotnet publish Trace \
  -c Release -r linux-x64 --self-contained false -o bin/publish/ \
  -p:GenerateDocumentationFile=false \
  -p:PublishSingleFile=true \
  -p:DebugType=None \
  -p:UseSharedCompilation=false \
  -nodeReuse:false
