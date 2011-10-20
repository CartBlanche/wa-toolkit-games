@echo off
setlocal
%~d0
cd "%~dp0"

SET powerShellDir=%WINDIR%\system32\windowspowershell\v1.0

%powerShellDir%\powershell.exe -NonInteractive -Command "Set-ExecutionPolicy unrestricted"
%powerShellDir%\powershell.exe -NonInteractive -command "& {%~dp0SetupACS ..\..\.. ..\..\..\code\SocialGames.Cloud ..\..\..\code\SocialGames.Web; exit $LastExitCode}"

if %errorlevel%==1 (
  echo ---------------------------------------------------------------------------------
  echo   Access Control Service configuration failed. Please check error messages above.
  echo ---------------------------------------------------------------------------------
  @PAUSE
  EXIT
)
