@echo off
setlocal
%~d0
cd "%~dp0"

IF EXIST %WINDIR%\SysWow64 (
	SET powerShellDir=%WINDIR%\SysWow64\windowspowershell\v1.0
) ELSE (
	SET powerShellDir=%WINDIR%\system32\windowspowershell\v1.0
)

%powerShellDir%\powershell.exe -NonInteractive -Command "Set-ExecutionPolicy unrestricted"
%powerShellDir%\powershell.exe -NonInteractive -command "& {%~dp0ValidateConfiguration.ps1 ..\..\..; exit $LastExitCode}

if %errorlevel%==0 (
  echo ----------------------------------------------------------------------------
  echo    No errors or warnings were found in Configuration.xml. 
  echo ----------------------------------------------------------------------------
  rem @pause
  rem exit
)

if %errorlevel%==1 (
  echo ----------------------------------------------------------------------------
  echo    Errors were found in Configuration.xml. 
  echo    Please follow the instructions described in StartHere.htm 
  echo ----------------------------------------------------------------------------
  @PAUSE
  exit 
)

if %errorlevel%==2 (
  choice /C:YN /M "Warnings were found in Configuration.xml. Do you want to continue performing the setup anyway ?"
  
  IF ERRORLEVEL 2 (
	ECHO.
	ECHO ========== Setup script execution aborted ==========
	ECHO.

	@PAUSE
	EXIT
  )
)
