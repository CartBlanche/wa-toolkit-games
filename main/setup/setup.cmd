@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION
%~d0
CD "%~dp0"

   ECHO --------------------------IMPORTANT-------------------------------
   ECHO  Please now follow the instructions in the StartHere document
   ECHO  to complete the settings in the Configuration.xml file.
   ECHO ------------------------------------------------------------------
   PAUSE
   SET /P Install="Have you completed the Configuration.xml file successfully? Setup will now install the sample Web application. (Y/N): "
   
   IF /I "!Install!"=="Y"  (
     cscript scripts\tasks\runAs.vbs scripts\tasks\executeTasks-development.cmd	
   ) ELSE (
      ECHO.
      ECHO The sample application will not be installed.
   )

ECHO.
PAUSE