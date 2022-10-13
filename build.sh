#!/bin/bash

echo "Building"
dotnet publish Trace \
  -c Release -r linux-x64 --self-contained false -o bin/temp/ \
  -p:GenerateDocumentationFile=false \
  -p:PublishSingleFile=true \
  -p:DebugType=None \
  -p:UseSharedCompilation=false \
  -nodeReuse:false

# # edit sudoers file to allow for passwordless service stop/start
# $ sudo visudo -f /etc/sudoers.d/trace
# %sudo ALL=NOPASSWD: /bin/systemctl stop trace.service, /bin/systemctl start trace.service

systemctl is-active trace.service
is_active=$?

if [[ $is_active -eq 0 ]]; then
  echo "Stopping service"
  sudo systemctl stop trace.service
fi

echo "Moving files"
rm -r bin/publish
mv bin/{temp,publish}

if [[ $is_active -eq 0 ]]; then
  echo "Starting service"
  sudo systemctl start trace.service
fi
