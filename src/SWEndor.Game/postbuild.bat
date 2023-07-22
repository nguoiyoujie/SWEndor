@echo off
echo Run postbuild...
set VERSIONBAT=version
set TEMPBAT=temp

if exist %TEMPBAT% (
  del %VERSIONBAT% 2>nul
  ren %TEMPBAT% %VERSIONBAT%
)

echo Build complete!
