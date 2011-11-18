@ECHO off
%~d0
cd "%~dp0"

ECHO.
ECHO ----------------------------------------
ECHO Installing Windows Azure Cmdlets PSSnapIn
ECHO ----------------------------------------

ECHO.
ECHO Copying Microsoft.WindowsAzure.Diagnostics.dll...
ECHO.

IF EXIST "c:\Program Files\Windows Azure SDK\v1.6\ref\Microsoft.WindowsAzure.Diagnostics.dll" (
	copy "c:\Program Files\Windows Azure SDK\v1.6\ref\Microsoft.WindowsAzure.Diagnostics.dll" ..\WindowsAzureCmdlets\ /Y
) ELSE (
	echo ---------------------------------------------------------------------------------------------
	echo   The diagnostics assembly for the Windows Azure SDK 1.6 could not be found at "c:\Program Files\Windows Azure SDK\v1.5\ref\Microsoft.WindowsAzure.Diagnostics.dll"
	echo ---------------------------------------------------------------------------------------------
	
	exit 1
)

IF EXIST %WINDIR%\SysWow64 (
	set installUtilDir=%WINDIR%\Microsoft.NET\Framework64\v2.0.50727
) ELSE (
	set installUtilDir=%WINDIR%\Microsoft.NET\Framework\v2.0.50727
)

SET assemblyPath="..\WindowsAzureCmdlets\Microsoft.WindowsAzure.Samples.ManagementTools.PowerShell.dll"

ECHO.
ECHO Installing PSSnapIn...
ECHO.
%installUtilDir%\installutil.exe -i %assemblyPath%
ECHO.

if errorlevel 1 (
  echo ---------------------------------------------------------------------------------------------
  echo   An error occured while installing the PowerShell SnapIn. Please check error messages above.
  echo ---------------------------------------------------------------------------------------------
  @PAUSE
)
