@echo off

echo Searching for .NET 4.0
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP" /s|FIND "v4" >nul
if ERRORLEVEL 0 (
  echo .NET 4.0 install OK!
) else (
  set ERR=.NET 4.0 not installed
)
