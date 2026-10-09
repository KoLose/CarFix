#!/bin/bash
set -e

export DISPLAY=:1
rm -f /tmp/.X1-lock /tmp/.X11-unix/X1

Xvfb :1 -screen 0 1366x768x24 &
sleep 1

x11vnc -display :1 -forever -shared -rfbport 5900 -nopw -quiet &
websockify --web=/usr/share/novnc 6080 localhost:5900 &

sleep 1
exec dotnet AvaloniaApp.dll
