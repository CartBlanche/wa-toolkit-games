@echo off
setlocal 
%~d0
cd "%~dp0"

echo.
echo ========= Running Deployment Script... =========
powershell.exe -ExecutionPolicy Unrestricted -NonInteractive -command %~dp0deployWAz
echo =========   Deployment Script done!    =========

@pause