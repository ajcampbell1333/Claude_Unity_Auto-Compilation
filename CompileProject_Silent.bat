@echo off
REM Unity Automated Compilation System (Silent/Non-Interactive)
REM For AI assistants, CI/CD, and automated testing
REM Copyright (c) 2025. Licensed under the MIT License.
REM
REM WHITE-LABEL TEMPLATE - Update the following:
REM 1. Unity executable paths (lines 9-13)
REM 2. Project-specific log markers in C# files (optional)
REM No namespace customization needed!

REM Find Unity executable - UPDATE THESE PATHS FOR YOUR UNITY VERSIONS
set UNITY_PATH=""
if exist "C:\Program Files\Unity\Hub\Editor\6000.0.60f1\Editor\Unity.exe" (
    set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\6000.0.60f1\Editor\Unity.exe"
) else if exist "C:\Program Files\Unity\Hub\Editor\2022.3.19f1\Editor\Unity.exe" (
    set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2022.3.19f1\Editor\Unity.exe"
) else (
    echo ERROR: Unity executable not found
    echo Please update Unity paths in this batch file
    exit /b 1
)

REM Get project path
set PROJECT_PATH=%~dp0
set PROJECT_PATH=%PROJECT_PATH:~0,-1%

REM Start Unity without -quit flag (CRITICAL: Do not add -quit!)
REM No namespace customization needed - CompilationReporterCLI is in global namespace
start "Unity Auto-Compiler" /B %UNITY_PATH% -batchmode -nographics -projectPath "%PROJECT_PATH%" -executeMethod CompilationReporterCLI.CompileAndExit -logFile "%PROJECT_PATH%\Temp\UnityBatchCompile.log"

REM Wait for report (2 minute timeout)
set TIMEOUT_COUNTER=0
set MAX_TIMEOUT=120

:WAIT_FOR_REPORT
if exist "%PROJECT_PATH%\Temp\CompilationErrors.log" goto REPORT_FOUND
timeout /t 1 /nobreak >nul
set /a TIMEOUT_COUNTER+=1
if %TIMEOUT_COUNTER% GEQ %MAX_TIMEOUT% goto TIMEOUT_REACHED
goto WAIT_FOR_REPORT

:TIMEOUT_REACHED
echo TIMEOUT: Compilation report not generated after 2 minutes
echo Check Temp\UnityBatchCompile.log for details
taskkill /IM Unity.exe /F >nul 2>&1
exit /b 1

:REPORT_FOUND
REM Give Unity time to finish writing
timeout /t 2 /nobreak >nul

REM Kill Unity
taskkill /IM Unity.exe /F >nul 2>&1

REM Check report status
findstr /C:"Status: SUCCESS" "%PROJECT_PATH%\Temp\CompilationErrors.log" >nul
if %errorlevel% EQU 0 (
    echo SUCCESS: Project compiled with no errors
    exit /b 0
) else (
    echo FAILED: Compilation errors detected
    echo See Temp\CompilationErrors.log for details
    exit /b 1
)

