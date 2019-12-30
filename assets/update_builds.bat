@echo off
set DBG_PATH=..\build\debug
set REL_PATH=..\releases\devbuild

for /f "tokens=*" %%a in ('dir /b /ad /o-dn') do (
  echo Copying %%a to %DBG_PATH%\%%a
  xcopy .\%%a  %DBG_PATH%\%%a /e /y /i /d
  echo.
  echo Copying %%a to %REL_PATH%\%%a
  xcopy .\%%a  %REL_PATH%\%%a /e /y /i /d
  echo.
)

pause
