#!/bin/bash
# # edit sudoers file to allow for passwordless service stop/start
# $ sudo visudo -f /etc/sudoers.d/trace
# %sudo ALL=NOPASSWD: /bin/systemctl stop trace.service, /bin/systemctl start trace.service
svc=trace.service

echo -n "Status: "
systemctl is-active $svc
is_active=$?

# exit on first error
set -e

if [[ $is_active -eq 0 ]]; then
  echo "Stopping service"
  sudo systemctl stop $svc
fi

echo "Moving files"
rm -rf bin/current
mv bin/{publish,current}

if [[ $is_active -eq 0 ]]; then
  echo "Starting service"
  sudo systemctl start $svc
fi
