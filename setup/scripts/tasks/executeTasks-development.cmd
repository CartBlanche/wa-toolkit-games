@echo off
setlocal 

%~d0
cd "%~dp0"

CALL .\ValidateConfiguration.cmd
@REM CALL .\installPSSnapIn.cmd
CALL .\SetupDB.cmd

pause