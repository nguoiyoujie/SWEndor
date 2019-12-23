@echo off
set EXE=SWEndor.exe

echo -------------------------------------
echo Checking prequisites...
call check
if defined ERR (
  echo Prerequisite error: %ERR%
  echo. 
  pause
  echo Exiting...
  goto :eof
)

echo -------------------------------------
echo Searching for latest version...
if exist versions.txt (
  set SEARCH=versions.txt
) else (
  set SEARCH='dir /b /ad /o-dn'
)

for /f "tokens=*" %%a in (%SEARCH%) do (
  if exist "%%a\%EXE%" (
    set GAMEPATH=%%a
    goto :runfile
  )
)
echo No version found!
echo. 
pause
echo Exiting...
goto :eof

:runfile
echo Version: %GAMEPATH%
echo Starting game...
start /d %GAMEPATH% %EXE%
