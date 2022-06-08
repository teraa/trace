#!/bin/bash

echo "Building"
dotnet publish -c Release -r linux-x64 -o bin/publish/ --self-contained false /p:PublishSingleFile=true /p:DebugType=None /p:UseSharedCompilation=false /nodeReuse:false TwitchLogger/
