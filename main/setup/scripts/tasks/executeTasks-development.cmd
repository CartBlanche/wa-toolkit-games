@echo off
setlocal 

%~d0
cd "%~dp0"

CALL .\ValidateConfiguration.cmd
CALL .\installPSSnapIn.cmd
CALL .\SetupDB.cmd
CALL .\SetupACS.cmd

pause