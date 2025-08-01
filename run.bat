@echo off
title PDF Receiver Rename Tool

echo Starting PDF Receiver Rename Tool...
echo.

cd /d "%~dp0ReceiverRename"

if exist "bin\Debug\net8.0\ReceiverRename.exe" (
    echo Running from Debug build...
    "bin\Debug\net8.0\ReceiverRename.exe" %*
) else if exist "bin\Release\net8.0\ReceiverRename.exe" (
    echo Running from Release build...
    "bin\Release\net8.0\ReceiverRename.exe" %*
) else (
    echo Building application...
    dotnet build
    if errorlevel 1 (
        echo Build failed!
        pause
        exit /b 1
    )
    echo Running application...
    dotnet run %*
)

pause