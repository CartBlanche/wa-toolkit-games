@echo off
setlocal 
%~d0
cd "%~dp0"

echo.
echo ========= Running Deployment Script... =========
powershell.exe -ExecutionPolicy Unrestricted -NonInteractive -File "%~dp0deployWAz.ps1"
echo =========   Deployment Script done!    =========

@pause