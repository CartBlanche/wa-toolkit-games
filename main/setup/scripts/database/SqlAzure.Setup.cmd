@echo off

setlocal 
%~d0
cd "%~dp0"

SET SqlAzure=%1%
SET dbName=%2%
SET User=%3%
SET Password=%4%

if "%2%"=="" set dbName=SocialGames

SqlCmd -S %SqlAzure% -U %User% -P %Password% -b -Q "exit(select 99 AS DbExists FROM [master].dbo.sysdatabases WHERE name = '%dbName%')"

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

SqlCmd -S %SqlAzure% -d master -U %User% -P %Password% -b -n -Q "DROP DATABASE [%dbName%]"

echo.
echo ========= Creating Database %dbName% =========
echo.

SqlCmd -S %SqlAzure% -d master -U %User% -P %Password% -b -n -Q "CREATE DATABASE [%dbName%]"

echo.
echo ========= Creating Database Schema =========
echo.

SqlCmd -S %SqlAzure% -d %dbName% -U %User% -P %Password%  -b -n -i "SqlAzure.CreateSchema.sql"

exit %errorlevel%