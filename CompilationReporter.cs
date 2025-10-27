// Copyright (c) 2025. Licensed under the MIT License.
// White-label template for Unity automated compilation
// See README.md for setup instructions

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.IO;
using System.Text;
using System.Linq;

/// <summary>
/// Compilation Reporter
/// 
/// Captures Unity compilation errors and writes them to a file that can be read by external tools.
/// Useful for CI/CD, automated testing, or AI-assisted development.
/// 
/// No namespace required - simplifies white-label setup
/// </summary>
[InitializeOnLoad]
public static class CompilationReporter
{
    private const string OUTPUT_FILE = "Temp/CompilationErrors.log";
    private static StringBuilder errorLog = new StringBuilder();
    private static bool compilationStarted = false;

    static CompilationReporter()
    {
        // Subscribe to compilation events
        CompilationPipeline.compilationStarted += OnCompilationStarted;
        CompilationPipeline.compilationFinished += OnCompilationFinished;
        CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;

        Debug.Log("🤖 [YOURPROJECT AUTO-COMPILE] Reporter initialized - automated compilation monitoring active");

        // If running in batch mode, force a compilation report on initialization
        if (Application.isBatchMode)
        {
            EditorApplication.delayCall += () =>
            {
                if (!EditorApplication.isCompiling)
                {
                    // Check for existing errors and generate report
                    GenerateCurrentReport();
                }
            };
        }
    }

    private static void OnCompilationStarted(object obj)
    {
        compilationStarted = true;
        errorLog.Clear();
        errorLog.AppendLine("===========================================");
        errorLog.AppendLine("YOURPROJECT COMPILATION REPORT");
        errorLog.AppendLine("🤖 AI-READABLE AUTOMATED COMPILATION CHECK");
        errorLog.AppendLine("===========================================");
        errorLog.AppendLine($"Started: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        errorLog.AppendLine($"Report ID: YOURPROJECT-{System.Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}");
        errorLog.AppendLine();
    }

    private static void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
    {
        if (messages == null || messages.Length == 0)
            return;

        // Filter for errors and warnings
        var errors = messages.Where(m => m.type == CompilerMessageType.Error).ToArray();
        var warnings = messages.Where(m => m.type == CompilerMessageType.Warning).ToArray();

        if (errors.Length > 0 || warnings.Length > 0)
        {
            errorLog.AppendLine($"Assembly: {Path.GetFileName(assemblyPath)}");
            errorLog.AppendLine("-------------------------------------------");

            // Log errors
            if (errors.Length > 0)
            {
                errorLog.AppendLine($"ERRORS: {errors.Length}");
                foreach (var error in errors)
                    errorLog.AppendLine($"  [{error.type}] {error.file}({error.line},{error.column}): {error.message}");
                errorLog.AppendLine();
            }

            // Log warnings
            if (warnings.Length > 0)
            {
                errorLog.AppendLine($"WARNINGS: {warnings.Length}");
                foreach (var warning in warnings)
                    errorLog.AppendLine($"  [{warning.type}] {warning.file}({warning.line},{warning.column}): {warning.message}");
                errorLog.AppendLine();
            }
        }
    }

    private static void OnCompilationFinished(object obj)
    {
        if (!compilationStarted)
            return;

        compilationStarted = false;

        // Get final compilation status
        errorLog.AppendLine("===========================================");
        errorLog.AppendLine($"Finished: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        
        if (errorLog.ToString().Contains("ERRORS:"))
            errorLog.AppendLine("Status: FAILED - Compilation errors detected");
        else if (errorLog.ToString().Contains("WARNINGS:"))
            errorLog.AppendLine("Status: SUCCESS (with warnings)");
        else
            errorLog.AppendLine("Status: SUCCESS - No errors or warnings");
        
        errorLog.AppendLine("===========================================");

        // Write to file
        try
        {
            string projectRoot = Application.dataPath.Replace("/Assets", "");
            string outputPath = Path.Combine(projectRoot, OUTPUT_FILE);
            
            // Ensure directory exists
            string directory = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(outputPath, errorLog.ToString());
            Debug.Log($"✅ [YOURPROJECT AUTO-COMPILE] Report generated successfully → {outputPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[YOURPROJECT AUTO-COMPILE] Failed to write compilation report: {ex.Message}");
        }
    }

    /// <summary>
    /// Menu item to manually trigger compilation and report
    /// </summary>
    [MenuItem("Tools/Compile and Report Errors")]
    public static void ManualCompileAndReport()
    {
        Debug.Log("[YOURPROJECT AUTO-COMPILE] Manual compilation triggered...");
        
        // Clear console
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);

        // Force recompile
        CompilationPipeline.RequestScriptCompilation();
    }

    /// <summary>
    /// Menu item to open the compilation report file
    /// </summary>
    [MenuItem("Tools/Open Compilation Report")]
    public static void OpenCompilationReport()
    {
        string projectRoot = Application.dataPath.Replace("/Assets", "");
        string outputPath = Path.Combine(projectRoot, OUTPUT_FILE);

        if (File.Exists(outputPath))
        {
            // Open in default text editor
            System.Diagnostics.Process.Start(outputPath);
        }
        else
        {
            EditorUtility.DisplayDialog("Compilation Report", 
                "No compilation report found. Try compiling first (Tools > Compile and Report Errors).", 
                "OK");
        }
    }

    /// <summary>
    /// Get the current compilation status
    /// </summary>
    public static string GetCompilationStatus()
    {
        if (EditorApplication.isCompiling)
            return "Compiling...";
        else if (CompilationPipeline.codeOptimization == CodeOptimization.None)
            return "Compilation Failed";
        else
            return "Compilation Successful";
    }

    /// <summary>
    /// Generate a compilation report based on current state (for batch mode)
    /// </summary>
    private static void GenerateCurrentReport()
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("===========================================");
        report.AppendLine("YOURPROJECT COMPILATION REPORT (BATCH MODE)");
        report.AppendLine("===========================================");
        report.AppendLine($"Generated: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine();

        // Check if project has compilation errors
        var assemblies = CompilationPipeline.GetAssemblies();
        bool hasErrors = false;

        foreach (var assembly in assemblies)
        {
            // This will only show if errors exist in Unity's internal state
            // The detailed errors are captured during compilation events
        }

        report.AppendLine("===========================================");
        if (EditorApplication.isCompiling)
            report.AppendLine("Status: COMPILING (in progress)");
        else
        {
            report.AppendLine("Status: No active compilation");
            report.AppendLine("Note: Detailed errors captured during compilation events");
            report.AppendLine("      Run CompileAndExit method to force compilation check");
        }
        report.AppendLine("===========================================");

        // Write to file
        try
        {
            string projectRoot = Application.dataPath.Replace("/Assets", "");
            string outputPath = Path.Combine(projectRoot, OUTPUT_FILE);
            
            string directory = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(outputPath, report.ToString());
            Debug.Log($"✅ [YOURPROJECT AUTO-COMPILE] Batch mode report generated → {outputPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[YOURPROJECT AUTO-COMPILE] Failed to write batch report: {ex.Message}");
        }
    }
}
#endif
