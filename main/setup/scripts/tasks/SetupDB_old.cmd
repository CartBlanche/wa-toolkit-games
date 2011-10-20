@echo off
setlocal
%~d0
cd "%~dp0"

IF EXIST %WINDIR%\SysWow64 (
	SET powerShellDir=%WINDIR%\SysWow64\windowspowershell\v1.0
) ELSE (
	SET powerShellDir=%WINDIR%\system32\windowspowershell\v1.0
)

ECHO ------------------------------------------------------------------
ECHO  After completing this setup please follow the instructions 
ECHO  in the Readme document to run the sample.
ECHO.
ECHO  The setup will now create a database for the sample.
ECHO ------------------------------------------------------------------
PAUSE
ECHO.
ECHO.

%powerShellDir%\powershell.exe -NonInteractive -NoProfile -Command "Set-ExecutionPolicy unrestricted"
%powerShellDir%\powershell.exe -NonInteractive -NoProfile -command "%~dp0SetupDB"

if errorlevel 1 (
   echo -------------------------------------------------------------------------------
   echo   Database creation failed. Please check error messages above.
   echo -------------------------------------------------------------------------------
)

PAUSE