@echo off
dotnet build ./REMod

cd REMod\bin\Debug\net7.0
.\REMod.exe
pause