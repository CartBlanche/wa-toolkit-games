@echo off
setlocal
%~d0
cd "%~dp0"

SET powerShellDir=%WINDIR%\system32\windowspowershell\v1.0

%powerShellDir%\powershell.exe -NonInteractive -Command "Set-ExecutionPolicy unrestricted"
%powerShellDir%\powershell.exe -NonInteractive -command "%~dp0SetupDB ..\database ..\..\.. ..\..\..\code\SocialGames.Cloud ..\..\..\code\SocialGames.Web"

if errorlevel 1 (
  echo -------------------------------------------------------------------------------
  echo   Database creation failed. Please check error messages above.
  echo -------------------------------------------------------------------------------
  @PAUSE
)

