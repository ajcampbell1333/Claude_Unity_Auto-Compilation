#!/bin/bash
# Unity Automated Compilation System (Interactive)
# For manual testing and development
# Copyright (c) 2025. Licensed under the MIT License.
#
# WHITE-LABEL TEMPLATE - Update the following:
# 1. Unity executable paths (lines 17-28)
# 2. Project-specific log markers in C# files (optional)
# No namespace customization needed!

echo "Unity Automated Compilation System"
echo "==================================="
echo ""

# Find Unity executable - UPDATE THESE PATHS FOR YOUR UNITY VERSIONS
UNITY_PATH=""
if [ -f "/Applications/Unity/Hub/Editor/6000.0.60f1/Unity.app/Contents/MacOS/Unity" ]; then
    UNITY_PATH="/Applications/Unity/Hub/Editor/6000.0.60f1/Unity.app/Contents/MacOS/Unity"
    echo "Found Unity: 6000.0.60f1"
elif [ -f "/Applications/Unity/Hub/Editor/2022.3.19f1/Unity.app/Contents/MacOS/Unity" ]; then
    UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.19f1/Unity.app/Contents/MacOS/Unity"
    echo "Found Unity: 2022.3.19f1"
elif [ -f "$HOME/Unity/Hub/Editor/6000.0.60f1/Editor/Unity" ]; then
    UNITY_PATH="$HOME/Unity/Hub/Editor/6000.0.60f1/Editor/Unity"
    echo "Found Unity: 6000.0.60f1 (Linux)"
elif [ -f "$HOME/Unity/Hub/Editor/2022.3.19f1/Editor/Unity" ]; then
    UNITY_PATH="$HOME/Unity/Hub/Editor/2022.3.19f1/Editor/Unity"
    echo "Found Unity: 2022.3.19f1 (Linux)"
else
    echo "ERROR: Unity executable not found"
    echo "Please update Unity paths in this script"
    read -p "Press Enter to exit..."
    exit 1
fi

echo ""

# Get project path
PROJECT_PATH="$(cd "$(dirname "$0")" && pwd)"

echo "Starting compilation..."
echo "This may take 1-2 minutes depending on project size"
echo ""

# Start Unity without -quit flag (CRITICAL: Do not add -quit!)
# No namespace customization needed - CompilationReporterCLI is in global namespace
"$UNITY_PATH" -batchmode -nographics -projectPath "$PROJECT_PATH" -executeMethod CompilationReporterCLI.CompileAndExit -logFile "$PROJECT_PATH/Temp/UnityBatchCompile.log" &
UNITY_PID=$!

# Wait for report (2 minute timeout)
TIMEOUT_COUNTER=0
MAX_TIMEOUT=120

while [ $TIMEOUT_COUNTER -lt $MAX_TIMEOUT ]; do
    if [ -f "$PROJECT_PATH/Temp/CompilationErrors.log" ]; then
        echo ""
        echo "Compilation complete! Generating report..."
        echo ""
        
        # Give Unity time to finish writing
        sleep 2
        
        # Kill Unity
        kill -9 $UNITY_PID 2>/dev/null
        
        # Display report
        echo "========================================"
        echo "COMPILATION REPORT"
        echo "========================================"
        echo ""
        cat "$PROJECT_PATH/Temp/CompilationErrors.log"
        echo ""
        echo "========================================"
        echo ""
        
        # Check report status
        if grep -q "Status: SUCCESS" "$PROJECT_PATH/Temp/CompilationErrors.log"; then
            echo "Result: SUCCESS - Project compiled with no errors"
            echo ""
            read -p "Press Enter to exit..."
            exit 0
        else
            echo "Result: FAILED - Compilation errors detected"
            echo "See above for details"
            echo ""
            read -p "Press Enter to exit..."
            exit 1
        fi
    fi
    
    sleep 1
    TIMEOUT_COUNTER=$((TIMEOUT_COUNTER + 1))
    if [ $((TIMEOUT_COUNTER % 10)) -eq 0 ]; then
        echo "[$TIMEOUT_COUNTER/$MAX_TIMEOUT] Waiting for compilation..."
    fi
done

# Timeout reached
echo ""
echo "========================================"
echo "TIMEOUT: Compilation did not complete"
echo "========================================"
echo ""
echo "The compilation report was not generated after 2 minutes."
echo "This could mean:"
echo "  - Unity is still compiling (large project)"
echo "  - Unity crashed during compilation"
echo "  - There are pre-existing compilation errors"
echo ""
echo "Check Temp/UnityBatchCompile.log for Unity's output"
echo ""
kill -9 $UNITY_PID 2>/dev/null
read -p "Press Enter to exit..."
exit 1

