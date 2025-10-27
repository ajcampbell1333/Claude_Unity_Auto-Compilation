# Unity Auto-Compile Quick Reference Card

## 🎯 Commands

### Windows
```batch
.\CompileProject_Silent.bat    # AI/CI (no output, exit code only)
.\CompileProject.bat           # Manual (full output, interactive)
```

### Linux/Mac
```bash
./CompileProject_Silent.sh     # AI/CI (no output, exit code only)
./CompileProject.sh            # Manual (full output, interactive)
```

## 📊 Exit Codes

| Code | Meaning |
|------|---------|
| 0    | Success - No compilation errors |
| 1    | Failure - Compilation errors detected OR timeout |

## 📁 File Locations

| File | Purpose | Gitignore? |
|------|---------|------------|
| `Temp/CompilationErrors.log` | Structured error report | ✅ Yes |
| `Temp/UnityBatchCompile.log` | Unity's raw output | ✅ Yes |
| `Assets/*/Editor/CompilationReporter.cs` | Auto-loads, monitors compilation | ❌ No |
| `Assets/*/Editor/CompilationReporterCLI.cs` | Batch mode interface | ❌ No |

## 🔍 Find & Replace for New Project

### Step 1: Update Log Markers (C# files - optional)
```
Find:    YOURPROJECT AUTO-COMPILE
Replace: MYGAME AUTO-COMPILE
```

### Step 2: Update Report Headers (C# files - optional)
```
Find:    YOURPROJECT COMPILATION REPORT
Replace: MYGAME COMPILATION REPORT
```

**Note:** No namespace customization needed! Scripts use global namespace for simplicity.

## 🛠️ Common Unity Paths

### Windows
```
C:\Program Files\Unity\Hub\Editor\[VERSION]\Editor\Unity.exe
```

### macOS
```
/Applications/Unity/Hub/Editor/[VERSION]/Unity.app/Contents/MacOS/Unity
```

### Linux
```
$HOME/Unity/Hub/Editor/[VERSION]/Editor/Unity
```

## 📝 Unity Versions Format
```
2022.3.19f1    # LTS
2023.2.10f1    # Tech Stream
6000.0.60f1    # Unity 6 (new versioning)
```

## ⚙️ Batch Script Anatomy

```batch
UNITY_PATH        → Path to Unity.exe
PROJECT_PATH      → Path to Unity project root
MAX_TIMEOUT       → Seconds to wait (default: 120)
-executeMethod    → C# method to invoke
-logFile          → Where Unity writes its log
```

## 🔧 Troubleshooting Commands

### Windows
```batch
REM Find Unity processes
tasklist | findstr Unity

REM Kill Unity
taskkill /IM Unity.exe /F

REM View recent log lines
powershell Get-Content Temp\UnityBatchCompile.log -Tail 50

REM Check if report exists
dir Temp\CompilationErrors.log
```

### Linux/Mac
```bash
# Find Unity processes
ps aux | grep Unity

# Kill Unity
killall Unity

# View recent log lines
tail -50 Temp/UnityBatchCompile.log

# Check if report exists
ls -l Temp/CompilationErrors.log
```

## 🎯 Menu Items (In Unity Editor)

| Menu Path | Action |
|-----------|--------|
| `YourProject > Compile and Report Errors` | Force compilation and generate report |
| `YourProject > Open Compilation Report` | Open report in default text editor |
| `YourProject > CLI Test - Compile and Report` | Test CLI without batch script |

## 🤖 Claude AI Usage Pattern

```
1. Run:  .\CompileProject_Silent.bat
2. Wait: [script completes]
3. Read: Exit code (0 or 1)
4. If 1: Read Temp/CompilationErrors.log
5. Fix:  Modify files based on errors
6. Loop: Back to step 1
```

## 📊 Report Format Cheat Sheet

```
===========================================
[PROJECT] COMPILATION REPORT
🤖 AI-READABLE AUTOMATED COMPILATION CHECK
===========================================
Generated: [timestamp]
Report ID: [PROJECT-GUID]

[Optional: Assembly + Errors/Warnings]

===========================================
Status: SUCCESS | FAILED
===========================================
```

## ⏱️ Typical Timings

| Project Size | First Compile | Subsequent |
|-------------|---------------|------------|
| Small (<100 scripts) | 20-40s | 10-20s |
| Medium (100-500) | 40-80s | 20-40s |
| Large (500+) | 80-120s | 40-80s |

## 🔒 Security Note

These scripts use `taskkill /F` (Windows) or `kill -9` (Linux/Mac) to forcefully terminate Unity. This is safe for compilation checks but will lose any unsaved work in the Editor. **Never run while Unity Editor is open manually.**

## 📞 Support

- **Documentation**: See README.md, SETUP.md, and SUMMARY.md
- **Unity Forums**: https://forum.unity.com/
- **CompilationPipeline API**: https://docs.unity3d.com/ScriptReference/Compilation.CompilationPipeline.html

---

**Print this page and keep it handy!** 📄✂️

