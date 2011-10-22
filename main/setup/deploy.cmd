@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION
%~d0
CD "%~dp0"

   ECHO --------------------------IMPORTANT-------------------------------
   ECHO  Please now follow the instructions in the StartHere document
   ECHO  to complete the settings in the Configuration.xml file.
   ECHO ------------------------------------------------------------------
   PAUSE
   SET /P Install="Have you completed the Configuration.xml file successfully? This procedure will now deploy the Social Games sample Web application to Windows Azure. (Y/N): "
   
   IF /I "!Install!"=="Y"  (
     cscript scripts\tasks\runAs.vbs scripts\tasks\executeTasks.cmd	
   ) ELSE (
      ECHO.
      ECHO The 'Blob Share' sample application will not be installed.
   )

ECHO.
PAUSE