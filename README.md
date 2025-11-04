# Unity Automated Compilation System for Claude AI

This is a white-label template for enabling AI-assisted Unity compilation without requiring the Unity Editor to be open. This system allows Claude (or other AI assistants) to compile Unity projects, detect errors, and fix them iteratively - similar to Unreal Engine's command-line build capabilities.

## 📋 What This Does

- **Automated Compilation**: Compile Unity projects from the command line without user intervention
- **AI-Readable Reports**: Structured error reports that AI assistants can parse and act on
- **Exit Code Support**: Returns 0 for success, 1 for failure (scriptable)
- **Zero-Intervention Workflow**: No manual steps required once set up
- **Race-Condition Safe**: Properly waits for compilation to complete before terminating Unity

## 🚀 Quick Start

### 1. Copy Files to Your Unity Project

```
YourUnityProject/
├── Assets/
│   └── YourProjectName/
│       └── Editor/
│           ├── CompilationReporter.cs          ← Copy here
│           └── CompilationReporterCLI.cs       ← Copy here
├── CompileProject_Silent.bat                   ← Copy here (Windows)
└── CompileProject.sh                           ← Copy here (Linux/Mac)
```

### 2. Update Project-Specific Names (Optional)

The white-label version uses the global namespace (no namespace required), making setup simpler. You can optionally customize the log markers:

**Optional:** Update log messages containing `[YOURPROJECT AUTO-COMPILE]` to match your project name:
- `CompilationReporter.cs` - All log messages
- `CompilationReporterCLI.cs` - All log messages
- `CompileProject_Silent.bat` - Line 58 (completion message check)
- `CompileProject_Silent.sh` - Line 49 (completion message check)

**Note:** Namespace customization is NOT needed - the CLI uses the global namespace for simplicity.

### 3. Update Unity Paths (Batch Script)

Edit `CompileProject_Silent.bat` (or `.sh` for Linux) and update the Unity executable paths to match your installed Unity versions:

```batch
REM Find Unity executable
set UNITY_PATH=""
if exist "C:\Program Files\Unity\Hub\Editor\YOUR_VERSION\Editor\Unity.exe" (
    set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\YOUR_VERSION\Editor\Unity.exe"
)
```

To find your Unity version:
- Open Unity Hub → Installs → Note the version number (e.g., `2022.3.19f1`)

### 4. Test the System

**Windows:**
```batch
.\CompileProject_Silent.bat
```

**Linux/Mac:**
```bash
chmod +x CompileProject.sh
./CompileProject.sh
```

**Expected Output:**
```
SUCCESS: Project compiled with no errors
```

Check the generated report at:
```
Temp/CompilationErrors.log
```

## 📦 What's Included

### CompilationReporter.cs
- Auto-loads when Unity Editor opens (`[InitializeOnLoad]`)
- Hooks into Unity's `CompilationPipeline` events
- Captures errors and warnings during compilation
- Writes structured reports to `Temp/CompilationErrors.log`
- Includes manual menu items: `YourProject/Compile and Report Errors`

### CompilationReporterCLI.cs
- Provides command-line interface for batch mode
- Method: `YourProject.Editor.CompilationReporterCLI.CompileAndExit()`
- Waits for compilation to complete (60-second timeout)
- Generates report without exiting Unity (batch script handles termination)

### CompileProject_Silent.bat (Windows)
- Launches Unity in batch mode
- Executes `CompileAndExit()` method
- Waits for report file (2-minute timeout)
- Terminates Unity process
- Returns exit code 0 (success) or 1 (failure)

### CompileProject.sh (Linux/Mac)
- Same functionality as `.bat` but for Unix-based systems
- Requires `chmod +x` to make executable

## 🛠️ How It Works

```
┌─────────────────────────────────────────────────────────┐
│ 1. Batch Script Deletes Old Reports                    │
│    Forces fresh compilation check                       │
└───────────────────┬─────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────┐
│ 2. Batch Script Launches Unity in Batch Mode           │
│    .\CompileProject_Silent.bat                          │
└───────────────────┬─────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────┐
│ 3. Unity Loads Project & Fully Initializes              │
│    CompilationReporter hooks into CompilationPipeline  │
└───────────────────┬─────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────┐
│ 4. CompilationReporterCLI.CompileAndExit() Executes    │
│    - Forces recompilation with RequestScriptCompilation │
│    - Waits for compilation to start (up to 20s)         │
│    - Waits for compilation to finish (up to 60s)        │
│    - Generates Temp/CompilationErrors.log               │
│    - Logs completion message                            │
│    - Does NOT exit (intentional!)                       │
└───────────────────┬─────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────┐
│ 5. Batch Script Detects Report File                    │
│    - Waits up to 2 minutes                              │
└───────────────────┬─────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────┐
│ 6. Batch Script Waits for Completion Message           │
│    - Checks log for "[YOURPROJECT AUTO-COMPILE]        │
│      Compilation complete" message                       │
│    - Ensures compilation actually finished               │
└───────────────────┬─────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────┐
│ 7. Batch Script Terminates Unity                       │
│    taskkill /IM Unity.exe /F                            │
└───────────────────┬─────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────┐
│ 8. Exit Code Returned                                   │
│    0 = Success  |  1 = Failure                          │
└─────────────────────────────────────────────────────────┘
```

## 🎯 Critical Design Notes

### Why No `-quit` Flag & Compilation Verification?

**Problem 1:** Unity's `-quit` flag causes Unity to exit immediately after executing the method, often before the report file is fully written.

**Problem 2:** If Unity already compiled the project, it might not recompile, leading to false positives from cached reports.

**Solution:** 
- **Delete old reports** before starting Unity (forces fresh check)
- **Force recompilation** using `CompilationPipeline.RequestScriptCompilation()`
- **Wait for compilation to actually start** (up to 20 seconds, retrying if needed)
- **Wait for compilation to finish** (up to 60 seconds)
- Launch Unity with `start /B` (background process)
- Let `CompileAndExit()` generate the report and log completion message
- Batch script waits for report file to appear
- **Batch script waits for completion message** in log (ensures compilation finished)
- Batch script explicitly kills Unity with `taskkill`

This prevents the race condition where Unity exits before writing the file, and ensures Unity actually compiles rather than using cached results.

### Report Format

```
===========================================
YOURPROJECT COMPILATION REPORT
🤖 AI-READABLE AUTOMATED COMPILATION CHECK
===========================================
Generated: 2025-10-26 19:19:26
Report ID: YOURPROJECT-A42C70DF

Status Check:
  Compiling: False
  Play Mode Enabled: False

===========================================
Status: SUCCESS - Project compiled successfully
No compilation errors detected
===========================================
```

If errors are present:
```
Assembly: Assembly-CSharp.dll
-------------------------------------------
ERRORS: 2
  [Error] Assets/Scripts/Player.cs(42,17): CS0103: The name 'missingVar' does not exist in the current context
  [Error] Assets/Scripts/Enemy.cs(15,5): CS0246: The type or namespace name 'InvalidType' could not be found

===========================================
Status: FAILED - Compilation errors detected
===========================================
```

## 🔍 Troubleshooting

### Unity Not Found
**Error:** `ERROR: Unity executable not found`

**Fix:** Edit `CompileProject_Silent.bat` and update Unity path:
```batch
if exist "C:\Program Files\Unity\Hub\Editor\YOUR_VERSION\Editor\Unity.exe" (
```

### Report Not Generated
**Error:** `TIMEOUT: Compilation report not generated`

**Possible Causes:**
1. Unity is taking longer than 2 minutes to compile
2. Scripts have compilation errors preventing completion
3. Unity crashed during compilation

**Fix:**
- Increase timeout in batch script (line 26): `set MAX_TIMEOUT=240` (4 minutes)
- Check `Temp/UnityBatchCompile.log` for Unity's output
- Open project manually to see if there are pre-existing errors

### Wrong Method Name
**Error:** `The type or namespace name 'CompilationReporterCLI' could not be found`

**Fix:** The white-label version uses the global namespace (no namespace required). Ensure batch script uses:
```batch
-executeMethod CompilationReporterCLI.CompileAndExit
```

**Note:** If you customize namespaces, ensure the batch script matches:
```batch
-executeMethod YourProject.Editor.CompilationReporterCLI.CompileAndExit
```

### Unity Won't Close
**Symptom:** Multiple Unity processes remain open

**Fix:** Manually kill Unity:
```batch
taskkill /IM Unity.exe /F
```

Or on Linux:
```bash
killall Unity
```

## 📚 Integration with Claude AI

Once set up, Claude can use this system to:

1. **Compile the project:**
   ```bash
   .\CompileProject_Silent.bat
   ```

2. **Check exit code:**
   - `0` = Success, no errors
   - `1` = Failure, compilation errors detected

3. **Read the report:**
   ```
   Temp/CompilationErrors.log
   ```

4. **Parse errors and fix them automatically:**
   - Extract file paths, line numbers, error codes
   - Open files and apply fixes
   - Recompile to verify
   - Iterate until all errors are resolved

This creates a **fully autonomous development loop** where Claude can:
- Make code changes
- Compile the project
- Detect errors
- Fix errors
- Verify fixes
- Continue development

All without requiring you to open Unity Editor or provide feedback!

## 📝 License

This template is provided as-is for use in any project. Modify freely to suit your needs.

## 🤝 Credits

Developed for the LBEAST SDK project by AJ Campbell and Claude AI (Anthropic).

## 🔗 See Also

- [Unity Documentation - Batch Mode](https://docs.unity3d.com/Manual/CommandLineArguments.html)
- [CompilationPipeline API](https://docs.unity3d.com/ScriptReference/Compilation.CompilationPipeline.html)




