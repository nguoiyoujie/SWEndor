@echo off
set VERSIONBAT=version
set TEMPBAT=temp

if exist %TEMPBAT% (
  del %VERSIONBAT% 2>nul
  ren %TEMPBAT% %VERSIONBAT%
)

