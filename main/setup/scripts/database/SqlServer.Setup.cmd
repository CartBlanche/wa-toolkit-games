@echo off

setlocal 
%~d0
cd "%~dp0"

SET sqlServer=%1%
SET dbName=%2%

if "%1%"=="" set sqlServer=.\SQLEXPRESS
if "%2%"=="" set dbName=SocialGames

SqlCmd -S %sqlServer% -Q "exit(select 99 AS DbExists FROM [master].dbo.sysdatabases WHERE name = '%dbName%')"

IF ERRORLEVEL 99 (
    choice /C:YN /M "WARNING, the database %dbName% already exists, if you continue all data will be lost, do you want to continue and drop the database ?"
	
	IF ERRORLEVEL 2 (
		ECHO.
		ECHO ========== Database script execution aborted ==========
		ECHO.

		EXIT 0
	)
)

echo.
echo ========= Dropping Database %dbName% =========
echo.
SqlCmd -S %sqlServer% -d master -Q "DROP DATABASE [%dbName%]"

echo.
echo ========= Creating Database %dbName% =========
echo.

SqlCmd -S %sqlServer% -d master -Q "CREATE DATABASE [%dbName%]"

echo.
echo ========= Creating Database Schema =========
echo.

SqlCmd -S %sqlServer% -E -d %dbName% -i "%~dp0SqlServer.CreateSchema.sql"

echo.
echo ========= Setting Database Permissions =========
echo.

SqlCmd -S %sqlServer% -E -d %dbName% -i "%~dp0SqlServer.SetPermissions.sql"

echo.
echo Creating %dbName% database Done!

exit %errorlevel%