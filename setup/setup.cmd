@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION
%~d0
CD "%~dp0"

   ECHO ------------------------OPTIONAL STEP-------------------------------
   ECHO  Update the configuration.xml file if you would like to customize 
   ECHO  the configuration settings of this project. You may skip this step 
   ECHO  if you want to use the default settings. For more information see
   ECHO  the StartHere document.
   ECHO --------------------------------------------------------------------
   PAUSE
   cscript scripts\tasks\runAs.vbs scripts\tasks\executeTasks-development.cmd	

ECHO.
