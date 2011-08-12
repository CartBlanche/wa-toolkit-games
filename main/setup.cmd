@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION
%~d0
CD "%~dp0"

start Setup\ContentInstallerClient\ContentInstallerClient.exe /depi:setup\SocialGamingToolkit.depi

SET /P SuccessfullyFinished="Did the Content Installer finish installing the required dependencies? (Y/N): "

IF /I "%SuccessfullyFinished%"=="Y" (
   ECHO ------------------------------------------------------------------
   ECHO  After completing this setup please follow the instructions 
   ECHO  in the Readme document to run the sample.
   ECHO ------------------------------------------------------------------
   PAUSE
   ECHO.
   ECHO.
   SET /P Install="Setup will now configure the connection strings for the sample. Do you want to continue? (Y/N): "
   
   IF /I "!Install!"=="Y"  (
     cscript Setup\Scripts\tasks\runAs.vbs Setup\Scripts\tasks\SetupDB.cmd	
   ) ELSE (
      ECHO.
      ECHO The Social Gaming Toolkit conection strings will not be configured.
   )
   
) ELSE (
   ECHO.
   ECHO Please, run the Content Installer again and install all the required dependencies so you can continue with the setup.
)

ECHO.
PAUSE