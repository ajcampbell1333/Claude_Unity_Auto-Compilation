#!/bin/bash
# Unity Automated Compilation System (Silent/Non-Interactive)
# For AI assistants, CI/CD, and automated testing
# Copyright (c) 2025. Licensed under the MIT License.
#
# WHITE-LABEL TEMPLATE - Update the following:
# 1. Unity executable paths (lines 13-20)
# 2. Project-specific log markers in C# files (optional)
# No namespace customization needed!

# Find Unity executable - UPDATE THESE PATHS FOR YOUR UNITY VERSIONS
UNITY_PATH=""
if [ -f "/Applications/Unity/Hub/Editor/6000.0.60f1/Unity.app/Contents/MacOS/Unity" ]; then
    UNITY_PATH="/Applications/Unity/Hub/Editor/6000.0.60f1/Unity.app/Contents/MacOS/Unity"
elif [ -f "/Applications/Unity/Hub/Editor/2022.3.19f1/Unity.app/Contents/MacOS/Unity" ]; then
    UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.19f1/Unity.app/Contents/MacOS/Unity"
elif [ -f "$HOME/Unity/Hub/Editor/6000.0.60f1/Editor/Unity" ]; then
    UNITY_PATH="$HOME/Unity/Hub/Editor/6000.0.60f1/Editor/Unity"
elif [ -f "$HOME/Unity/Hub/Editor/2022.3.19f1/Editor/Unity" ]; then
    UNITY_PATH="$HOME/Unity/Hub/Editor/2022.3.19f1/Editor/Unity"
else
    echo "ERROR: Unity executable not found"
    echo "Please update Unity paths in this script"
    exit 1
fi

# Get project path
PROJECT_PATH="$(cd "$(dirname "$0")" && pwd)"

# Remove old report
rm -f "$PROJECT_PATH/Temp/CompilationErrors.log"
rm -f "$PROJECT_PATH/Temp/UnityBatchCompile.log"

# Start Unity without -quit flag (CRITICAL: Do not add -quit!)
# No namespace customization needed - CompilationReporterCLI is in global namespace
"$UNITY_PATH" -batchmode -nographics -projectPath "$PROJECT_PATH" -executeMethod CompilationReporterCLI.CompileAndExit -logFile "$PROJECT_PATH/Temp/UnityBatchCompile.log" &
UNITY_PID=$!

# Wait for report (2 minute timeout)
TIMEOUT_COUNTER=0
MAX_TIMEOUT=120

while [ $TIMEOUT_COUNTER -lt $MAX_TIMEOUT ]; do
    if [ -f "$PROJECT_PATH/Temp/CompilationErrors.log" ]; then
        # Report found - give Unity time to finish writing
        sleep 2
        
        # Kill Unity
        kill -9 $UNITY_PID 2>/dev/null
        
        # Check report status
        if grep -q "Status: SUCCESS" "$PROJECT_PATH/Temp/CompilationErrors.log"; then
            echo "SUCCESS: Project compiled with no errors"
            exit 0
        else
            echo "FAILED: Compilation errors detected"
            echo "See Temp/CompilationErrors.log for details"
            exit 1
        fi
    fi
    
    sleep 1
    TIMEOUT_COUNTER=$((TIMEOUT_COUNTER + 1))
done

# Timeout reached
echo "TIMEOUT: Compilation report not generated after 2 minutes"
echo "Check Temp/UnityBatchCompile.log for details"
kill -9 $UNITY_PID 2>/dev/null
exit 1

