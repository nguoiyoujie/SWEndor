@echo off
set DBG_PATH=..\build\debug
set REL_PATH=..\releases\devbuild
for %%a in (%DBG_PATH%) do set FULL_DBG_PATH=%%~fa
for %%a in (%REL_PATH%) do set FULL_REL_PATH=%%~fa
setlocal EnableDelayedExpansion

for /f "tokens=*" %%a in ('dir /b /ad /o-dn') do (

  echo Copying %%a to %DBG_PATH%\%%a
  xcopy ".\%%a"  "%DBG_PATH%\%%a" /e /y /i /d
  echo.
  echo Copying %%a to %REL_PATH%\%%a
  xcopy ".\%%a"  "%REL_PATH%\%%a" /e /y /i /d
  echo.
)

for /f "tokens=*" %%a in ('dir /b /ad /o-dn') do (
  
  echo Cleaning %DBG_PATH%\%%a
  for /f "tokens=*" %%1 in ('dir "%DBG_PATH%\%%a" /b /s /ad /o-dn') do (
    call set SHORT_PATH=%%1
    call set SHORT_PATH=!SHORT_PATH:%FULL_DBG_PATH%\%%a=!
    if not exist ".\%%a\!SHORT_PATH!" (
      if exist "%DBG_PATH%\%%a\!SHORT_PATH!" (
        echo delete "%DBG_PATH%\%%a\!SHORT_PATH!"
        rd /s /q "%DBG_PATH%\%%a\!SHORT_PATH!"
      )
    )
  )

  for /f "tokens=*" %%1 in ('dir "%DBG_PATH%\%%a" /b /s /a-d /o-dn') do (
    call set SHORT_PATH=%%1
    call set SHORT_PATH=!SHORT_PATH:%FULL_DBG_PATH%\%%a=!
    if not exist ".\%%a\!SHORT_PATH!" (
      if exist "%DBG_PATH%\%%a\!SHORT_PATH!" (
        echo delete "%DBG_PATH%\%%a\!SHORT_PATH!"
        del "%DBG_PATH%\%%a\!SHORT_PATH!"
      )
    )
  )
  echo.

  echo Cleaning %REL_PATH%\%%a
  for /f "tokens=*" %%1 in ('dir "%REL_PATH%\%%a" /b /s /ad /o-dn') do (
    call set SHORT_PATH=%%1
    call set SHORT_PATH=!SHORT_PATH:%FULL_REL_PATH%\%%a=!
    if not exist ".\%%a\!SHORT_PATH!" (
      if exist "%REL_PATH%\%%a\!SHORT_PATH!" (
        echo delete "%REL_PATH%\%%a\!SHORT_PATH!"
        rd /s /q "%REL_PATH%\%%a\!SHORT_PATH!"
      )
    )
  )

  for /f "tokens=*" %%1 in ('dir "%REL_PATH%\%%a" /b /s /a-d /o-dn') do (
    call set SHORT_PATH=%%1
    call set SHORT_PATH=!SHORT_PATH:%FULL_REL_PATH%\%%a=!
    if not exist ".\%%a\!SHORT_PATH!" (
      if exist "%REL_PATH%\%%a\!SHORT_PATH!" (
        echo delete "%REL_PATH%\%%a\!SHORT_PATH!"
        del "%REL_PATH%\%%a\!SHORT_PATH!"
      )
    )
  )
  echo.
)

pause
