@echo off
set TITLE=SWEndor.Core
set MAJORVERSION=0
set MINORVERSION=0

for /f "tokens=2-4 delims=/ "  %%a in ("%DATE%") do (set MM=%%a& set DD=%%b& set YYYY=%%c)
set BUILDDATE=%YYYY%%MM%%DD%

set ASMYINFO_CS=Properties\AssemblyInfo.cs
set BUILD_CS=Build.cs

set VERSIONBAT=version
set TEMPBAT=temp
del %TEMPBAT% 2>nul

set REVISION=0
set NEWDATE=0

setlocal EnableDelayedExpansion
if exist %VERSIONBAT% (
  for /f "tokens=1,2 delims== " %%a in (%VERSIONBAT%) do (
    if %%a==MAJORVERSION (
      set MAJORVERSION=%%b
    ) else if %%a==MINORVERSION (
      set MINORVERSION=%%b
    ) else if %%a==REVISION (
      if not !NEWDATE!==1 (
        set /a REVISION=%%b+1
      )
    ) else if %%a==BUILDDATE (
      if not %%b==%BUILDDATE% (
        set REVISION=0
        set NEWDATE=1
      )
    )
  )
)

if %REVISION% lss 10 set REVISION=0%REVISION%

echo MAJORVERSION=%MAJORVERSION% > %TEMPBAT%
echo MINORVERSION=%MINORVERSION% >> %TEMPBAT%
echo BUILDDATE=%BUILDDATE% >> %TEMPBAT%
echo REVISION=%REVISION% >> %TEMPBAT%

set VERSION=%MAJORVERSION%.%MINORVERSION%.%BUILDDATE%.%REVISION%
set MSVERSION=%MAJORVERSION%.%MINORVERSION%.*
set MSFILEVERSION=%MAJORVERSION%.%MINORVERSION%.%YYYY:~1,3%%MM%.%DD%%REVISION%
echo %VERSION%

attrib -r %BUILD_CS%
echo namespace %TITLE% > %BUILD_CS%
echo { >> %BUILD_CS%
echo   //This file is auto generated by %~nx0 >> %BUILD_CS%
echo   public static class Build >> %BUILD_CS%
echo   { >> %BUILD_CS%
echo     public static string Time = "%DATE% %TIME%"; >> %BUILD_CS%
echo     public static string BuildDate = "%BUILDDATE%"; >> %BUILD_CS%
echo     public static string Revision = "%REVISION%"; >> %BUILD_CS%
echo   } >> %BUILD_CS%
echo } >> %BUILD_CS%

attrib -r %ASMYINFO_CS%

echo using System.Reflection; > %ASMYINFO_CS%
echo using System.Runtime.InteropServices; >> %ASMYINFO_CS%
echo using System.Runtime.CompilerServices; >> %ASMYINFO_CS%
echo.>> %ASMYINFO_CS%
echo //This file is auto generated by %~nx0 >> %ASMYINFO_CS%
echo [assembly: AssemblyTitle("%TITLE%")] >> %ASMYINFO_CS%
echo [assembly: AssemblyConfiguration("")] >> %ASMYINFO_CS%
echo [assembly: AssemblyCompany("")] >> %ASMYINFO_CS%
echo [assembly: AssemblyProduct("%TITLE%")] >> %ASMYINFO_CS%
echo [assembly: AssemblyCopyright("Copyright ? 2017-%YYYY%. All rights reserved.")] >> %ASMYINFO_CS%
echo [assembly: AssemblyTrademark("")] >> %ASMYINFO_CS%
echo [assembly: AssemblyCulture("")] >> %ASMYINFO_CS%
echo.>> %ASMYINFO_CS%
echo [assembly: ComVisible(false)] >> %ASMYINFO_CS%
echo [assembly: AssemblyVersion("%MSVERSION%")] >> %ASMYINFO_CS%
echo [assembly: AssemblyFileVersion("%MSFILEVERSION%")] >> %ASMYINFO_CS%
echo.>> %ASMYINFO_CS%
echo //Debug >> %ASMYINFO_CS%
echo #if DEBUG >> %ASMYINFO_CS%
echo [assembly: InternalsVisibleTo("SWEndorTest")] >> %ASMYINFO_CS%
echo #endif >> %ASMYINFO_CS%