@echo off

setlocal 
%~d0
cd "%~dp0"

SET SqlAzure=%1%
SET dbName=%2%
SET User=%3%
SET Password=%4%

if "%2%"=="" set dbName=SocialGames

echo.
echo ========= Dropping Database %dbName% =========
echo.

OSQL -S %SqlAzure% -d master -U %User% -P %Password% -b -n -Q "DROP DATABASE [%dbName%]"

echo.
echo ========= Creating Database %dbName% =========
echo.

OSQL -S %SqlAzure% -d master -U %User% -P %Password% -b -n -Q "CREATE DATABASE [%dbName%]"

echo.
echo ========= Creating Database Schema =========
echo.

OSQL -S %SqlAzure% -d %dbName% -U %User% -P %Password%  -b -n -i "%~dp0SqlAzure.CreateSchema.sql"

echo.
echo Creating %dbName% database Done!

exit %errorlevel%