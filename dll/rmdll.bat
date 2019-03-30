@echo off
echo "Removing DLLs..."
ping 127.0.0.1 -n 3 > nul
cd %~dp0
@echo on
del "..\*.dll"
@echo off
ping 127.0.0.1 -n 2 > nul