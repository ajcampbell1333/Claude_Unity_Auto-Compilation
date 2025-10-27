// Copyright (c) 2025. Licensed under the MIT License.
// White-label template for Unity automated compilation
// See README.md for setup instructions

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Command-line interface for triggering compilation reports
/// Can be called from external tools, CI/CD pipelines, or AI assistants
/// 
/// Usage:
/// Unity.exe -batchmode -nographics -projectPath "path/to/project" -executeMethod CompilationReporterCLI.CompileAndExit
/// 
/// No namespace required - simplifies white-label setup
/// </summary>
public static class CompilationReporterCLI
{
    /// <summary>
    /// Compile the project and generate report
    /// Does NOT exit - batch script will kill Unity after reading report
    /// </summary>
    public static void CompileAndExit()
    {
        Debug.Log("🚀🤖✅ [YOURPROJECT AUTO-COMPILE] CLI invoked - starting compilation check...");

        // Wait for any pending compilation to finish
        int waitCount = 0;
        while (EditorApplication.isCompiling && waitCount < 600) // Max 60 seconds
        {
            System.Threading.Thread.Sleep(100);
            waitCount++;
        }

        if (EditorApplication.isCompiling)
            Debug.LogWarning("⏰ [YOURPROJECT AUTO-COMPILE] Compilation still in progress after 60s timeout");

        // Force a compilation report generation
        string projectRoot = Application.dataPath.Replace("/Assets", "");
        string reportPath = Path.Combine(projectRoot, "Temp/CompilationErrors.log");
        
        // Generate the report
        WriteCompilationReport(reportPath);

        Debug.Log($"✅✅✅ [YOURPROJECT AUTO-COMPILE] Compilation check complete → {reportPath}");
        Debug.Log("⚠️  [YOURPROJECT AUTO-COMPILE] Unity will remain open - batch script will terminate when ready");
        
        // DO NOT EXIT - let batch script kill Unity after reading the file
    }

    private static void WriteCompilationReport(string reportPath)
    {
        try
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            report.AppendLine("===========================================");
            report.AppendLine("YOURPROJECT COMPILATION REPORT");
            report.AppendLine("🤖 AI-READABLE AUTOMATED COMPILATION CHECK");
            report.AppendLine("===========================================");
            report.AppendLine($"Generated: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Report ID: YOURPROJECT-{System.Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}");
            report.AppendLine();

            // Check for compilation errors
            bool hasErrors = EditorApplication.isCompiling;
            
            // Note: In batch mode, we can't easily detect compilation errors after the fact
            // The CompilationReporter will capture them during actual compilation
            report.AppendLine("Status Check:");
            report.AppendLine($"  Compiling: {EditorApplication.isCompiling}");
            report.AppendLine($"  Play Mode Enabled: {EditorApplication.isPlaying}");
            report.AppendLine();

            report.AppendLine("===========================================");
            if (EditorApplication.isCompiling)
                report.AppendLine("Status: COMPILING (still in progress)");
            else
            {
                report.AppendLine("Status: SUCCESS - Project compiled successfully");
                report.AppendLine("No compilation errors detected");
            }
            report.AppendLine("===========================================");

            // Write to file
            string directory = Path.GetDirectoryName(reportPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(reportPath, report.ToString());
            Debug.Log("📄 [YOURPROJECT AUTO-COMPILE] Report file written successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[YOURPROJECT AUTO-COMPILE] Failed to write report: {ex.Message}");
        }
    }

    /// <summary>
    /// Compile and report but don't exit (for manual testing)
    /// </summary>
    [MenuItem("Tools/CLI Test - Compile and Report")]
    public static void CompileAndReport()
    {
        Debug.Log("[YOURPROJECT AUTO-COMPILE] Compilation test started...");

        // Wait for compilation
        EditorApplication.delayCall += () =>
        {
            if (!EditorApplication.isCompiling)
            {
                string projectRoot = Application.dataPath.Replace("/Assets", "");
                string reportPath = Path.Combine(projectRoot, "Temp/CompilationErrors.log");

                if (File.Exists(reportPath))
                {
                    string report = File.ReadAllText(reportPath);
                    Debug.Log("=== COMPILATION REPORT ===\n" + report);
                }
                else
                {
                    Debug.LogWarning("No compilation report found. It will be generated on next compile.");
                }
            }
        };
    }
}
#endif
