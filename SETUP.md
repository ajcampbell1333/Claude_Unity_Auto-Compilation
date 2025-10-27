# Quick Setup Guide

This guide will walk you through installing the Unity Automated Compilation System in your project.

## ⏱️ Time Required: ~5 minutes

## 📋 Prerequisites

- Unity project (any version)
- Unity Hub installed with at least one Unity Editor version
- Text editor (VS Code, Notepad++, etc.)

## 🚀 Installation Steps

### Step 1: Copy Files

Copy these files from `Claude_Unity_AutoCompilation/` to your Unity project:

```
YourUnityProject/
├── Assets/
│   └── YourProjectName/
│       └── Editor/                          ← Create this folder if needed
│           ├── CompilationReporter.cs       ← Copy here
│           └── CompilationReporterCLI.cs    ← Copy here
│
├── CompileProject_Silent.bat                ← Copy to project root (Windows)
├── CompileProject.bat                       ← Copy to project root (Windows)
├── CompileProject_Silent.sh                 ← Copy to project root (Linux/Mac)
└── CompileProject.sh                        ← Copy to project root (Linux/Mac)
```

### Step 2: Update Log Messages (Optional but Recommended)

In both C# files, replace log markers for clarity:

**Find & Replace:**
- Find: `YOURPROJECT AUTO-COMPILE`
- Replace: `MYGAME AUTO-COMPILE` (your project name in uppercase)

This makes logs easier to identify in multi-project environments.

### Step 3: Update Unity Executable Paths (Batch/Shell Scripts)

**Windows (CompileProject_Silent.bat and CompileProject.bat):**

1. Open Unity Hub
2. Go to **Installs** tab
3. Note your Unity version (e.g., `2022.3.19f1`)
4. Edit batch files and update paths on lines 9-13:

```batch
if exist "C:\Program Files\Unity\Hub\Editor\2022.3.19f1\Editor\Unity.exe" (
    set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2022.3.19f1\Editor\Unity.exe"
```

**Linux/Mac (CompileProject_Silent.sh and CompileProject.sh):**

1. Find Unity installation path (usually `/Applications/Unity/Hub/Editor/`)
2. Edit shell scripts and update paths on lines 13-20:

```bash
if [ -f "/Applications/Unity/Hub/Editor/2022.3.19f1/Unity.app/Contents/MacOS/Unity" ]; then
    UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.19f1/Unity.app/Contents/MacOS/Unity"
```

### Step 4: Make Scripts Executable (Linux/Mac Only)

```bash
chmod +x CompileProject_Silent.sh
chmod +x CompileProject.sh
```

### Step 5: Test the Installation

**Windows:**
```batch
.\CompileProject.bat
```

**Linux/Mac:**
```bash
./CompileProject.sh
```

**Expected Output:**
```
Unity Automated Compilation System
===================================

Found Unity: 2022.3.19f1

Starting compilation...
This may take 1-2 minutes depending on project size

[10/120] Waiting for compilation...
[20/120] Waiting for compilation...

Compilation complete! Generating report...

========================================
COMPILATION REPORT
========================================

===========================================
MYGAME COMPILATION REPORT
🤖 AI-READABLE AUTOMATED COMPILATION CHECK
===========================================
Generated: 2025-10-26 19:19:26
Report ID: MYGAME-A42C70DF

Status Check:
  Compiling: False
  Play Mode Enabled: False

===========================================
Status: SUCCESS - Project compiled successfully
No compilation errors detected
===========================================

========================================

Result: SUCCESS - Project compiled with no errors

Press any key to exit...
```

## ✅ Verification Checklist

- [ ] Files copied to correct locations
- [ ] Log markers updated (optional)
- [ ] Unity paths updated in batch/shell scripts
- [ ] Scripts made executable (Linux/Mac)
- [ ] Test run completed successfully
- [ ] Report generated at `Temp/CompilationErrors.log`

## 🔧 Troubleshooting

### "Unity executable not found"
- Verify Unity is installed via Unity Hub
- Check Unity version in batch/shell script matches installed version
- Ensure path format is correct for your OS

### "Compilation report not generated"
- Check for pre-existing compilation errors in Unity Editor
- Increase timeout in batch/shell script (line 26/30)
- Review `Temp/UnityBatchCompile.log` for Unity's error output

### "The type or namespace name 'CompilationReporterCLI' could not be found"
- Verify files are in `Assets/[ProjectName]/Editor/` folder (Unity only recognizes Editor scripts in folders named `Editor`)
- Ensure C# files were copied correctly with no compilation errors
- Restart Unity Editor after copying files

## 📚 Next Steps

Once installed:
- Use `CompileProject_Silent.bat/.sh` for AI-assisted compilation
- Use `CompileProject.bat/.sh` for manual testing
- Add to `.gitignore`: `Temp/CompilationErrors.log` and `Temp/UnityBatchCompile.log`
- Integrate with your CI/CD pipeline if desired

## 🤖 For Claude AI

This system is now ready for automated compilation! You can:
1. Run `.\CompileProject_Silent.bat` (Windows) or `./CompileProject_Silent.sh` (Linux/Mac)
2. Check exit code (0 = success, 1 = failure)
3. Read `Temp/CompilationErrors.log` for detailed error information
4. Parse errors and fix them automatically

See [README.md](README.md) for complete documentation.

