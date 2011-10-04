@echo off

sc config wuauserv start= demand
wusa.exe "%~dp0Windows6.1-KB974405-x64.msu" /quiet /norestart
sc config wuauserv start= disabled