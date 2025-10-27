@echo off
REM Unity Automated Compilation System (Interactive)
REM For manual testing and development
REM Copyright (c) 2025. Licensed under the MIT License.
REM
REM WHITE-LABEL TEMPLATE - Update the following:
REM 1. Unity executable paths (lines 17-23)
REM 2. Project-specific log markers in C# files (optional)
REM No namespace customization needed!

echo Unity Automated Compilation System
echo ===================================
echo.

REM Find Unity executable - UPDATE THESE PATHS FOR YOUR UNITY VERSIONS
set UNITY_PATH=""
if exist "C:\Program Files\Unity\Hub\Editor\6000.0.60f1\Editor\Unity.exe" (
    set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\6000.0.60f1\Editor\Unity.exe"
    echo Found Unity: 6000.0.60f1
) else if exist "C:\Program Files\Unity\Hub\Editor\2022.3.19f1\Editor\Unity.exe" (
    set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2022.3.19f1\Editor\Unity.exe"
    echo Found Unity: 2022.3.19f1
) else (
    echo ERROR: Unity executable not found
    echo Please update Unity paths in this batch file
    pause
    exit /b 1
)

echo.

REM Get project path
set PROJECT_PATH=%~dp0
set PROJECT_PATH=%PROJECT_PATH:~0,-1%

echo Starting compilation...
echo This may take 1-2 minutes depending on project size
echo.

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
echo [%TIMEOUT_COUNTER%/%MAX_TIMEOUT%] Waiting for compilation...
goto WAIT_FOR_REPORT

:TIMEOUT_REACHED
echo.
echo ========================================
echo TIMEOUT: Compilation did not complete
echo ========================================
echo.
echo The compilation report was not generated after 2 minutes.
echo This could mean:
echo   - Unity is still compiling (large project)
echo   - Unity crashed during compilation
echo   - There are pre-existing compilation errors
echo.
echo Check Temp\UnityBatchCompile.log for Unity's output
echo.
taskkill /IM Unity.exe /F >nul 2>&1
echo Press any key to exit...
pause >nul
exit /b 1

:REPORT_FOUND
echo.
echo Compilation complete! Generating report...
echo.

REM Give Unity time to finish writing
timeout /t 2 /nobreak >nul

REM Kill Unity
taskkill /IM Unity.exe /F >nul 2>&1

REM Display report
echo ========================================
echo COMPILATION REPORT
echo ========================================
echo.
type "%PROJECT_PATH%\Temp\CompilationErrors.log"
echo.
echo ========================================
echo.

REM Check report status
findstr /C:"Status: SUCCESS" "%PROJECT_PATH%\Temp\CompilationErrors.log" >nul
if %errorlevel% EQU 0 (
    echo Result: SUCCESS - Project compiled with no errors
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 0
) else (
    echo Result: FAILED - Compilation errors detected
    echo See above for details
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

