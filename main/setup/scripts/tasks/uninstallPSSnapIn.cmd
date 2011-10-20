@echo off
%~d0
cd "%~dp0"

ECHO.
ECHO --------------------------------------
ECHO Uninstalling Windows Azure Cmdlets PSSnapIn
ECHO --------------------------------------

IF EXIST %WINDIR%\SysWow64 (
	set installUtilDir=%WINDIR%\Microsoft.NET\Framework64\v2.0.50727
) ELSE (
	set installUtilDir=%WINDIR%\Microsoft.NET\Framework\v2.0.50727
)

SET assemblyPath="..\WindowsAzureCmdlets\Microsoft.WindowsAzure.Samples.ManagementTools.PowerShell.dll"

ECHO.
ECHO Uninstalling PSSnapIn...
ECHO.
%installUtilDir%\installutil.exe -u %assemblyPath%
ECHO.

if errorlevel 1 (
  echo -----------------------------------------------------------------------------------------------
  echo   An error occured while uninstalling the PowerShell SnapIn. Please check error messages above.
  echo -----------------------------------------------------------------------------------------------
  PAUSE
)
