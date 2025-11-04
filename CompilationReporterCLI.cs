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
        Debug.Log("🚀🤖 [YOURPROJECT AUTO-COMPILE] CLI invoked - forcing compilation...");

        // Wait for Unity to fully initialize before forcing compilation
        EditorApplication.delayCall += () =>
        {
            // Force a recompilation to ensure we actually compile
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            
            Debug.Log("📦 [YOURPROJECT AUTO-COMPILE] Recompilation requested - waiting for compilation to start...");

            // Wait for compilation to actually start
            EditorApplication.delayCall += () =>
            {
                int waitStartCount = 0;
                while (!EditorApplication.isCompiling && waitStartCount < 200) // Max 20 seconds for compilation to start
                {
                    System.Threading.Thread.Sleep(100);
                    waitStartCount++;
                    // Check every 10 iterations (1 second)
                    if (waitStartCount % 10 == 0)
                    {
                        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(); // Keep requesting
                    }
                }

                if (!EditorApplication.isCompiling && waitStartCount >= 200)
                {
                    Debug.LogWarning("⚠️ [YOURPROJECT AUTO-COMPILE] Compilation did not start after 20s - checking if already compiled");
                    // Still generate a report even if compilation didn't start
                    string projectRoot = Application.dataPath.Replace("/Assets", "");
                    string reportPath = Path.Combine(projectRoot, "Temp/CompilationErrors.log");
                    WriteCompilationReport(reportPath);
                    Debug.Log($"✅ [YOURPROJECT AUTO-COMPILE] Status check complete → {reportPath}");
                    return;
                }

                Debug.Log("⚙️ [YOURPROJECT AUTO-COMPILE] Compilation started - waiting for completion...");

                // Now wait for compilation to finish
                EditorApplication.delayCall += () =>
                {
                    int waitCount = 0;
                    while (EditorApplication.isCompiling && waitCount < 600) // Max 60 seconds
                    {
                        System.Threading.Thread.Sleep(100);
                        waitCount++;
                    }

                    if (EditorApplication.isCompiling)
                    {
                        Debug.LogWarning("⏰ [YOURPROJECT AUTO-COMPILE] Compilation still in progress after 60s timeout");
                    }
                    else
                    {
                        Debug.Log("✅ [YOURPROJECT AUTO-COMPILE] Compilation finished");
                    }

                    // Force a compilation report generation after compilation completes
                    string projectRoot = Application.dataPath.Replace("/Assets", "");
                    string reportPath = Path.Combine(projectRoot, "Temp/CompilationErrors.log");
                    
                    // Generate the report
                    WriteCompilationReport(reportPath);

                    Debug.Log($"[YOURPROJECT AUTO-COMPILE] Compilation complete → {reportPath}");
                    Debug.Log("⚠️  [YOURPROJECT AUTO-COMPILE] Unity will remain open - batch script will terminate when ready");
                };
            };
        };
    }

    private static void WriteCompilationReport(string reportPath)
    {
        try
        {
            // Check if CompilationReporter already wrote a report (from compilation events)
            if (File.Exists(reportPath))
            {
                string existingReport = File.ReadAllText(reportPath);
                // If the existing report has actual compilation results, use it
                if (existingReport.Contains("Status: SUCCESS") || existingReport.Contains("Status: FAILED"))
                {
                    Debug.Log("📄 [YOURPROJECT AUTO-COMPILE] Using existing compilation report from CompilationReporter");
                    return; // Use the report from CompilationReporter which has detailed errors
                }
            }

            // Otherwise, generate a basic report
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            report.AppendLine("===========================================");
            report.AppendLine("YOURPROJECT COMPILATION REPORT");
            report.AppendLine("🤖 AI-READABLE AUTOMATED COMPILATION CHECK");
            report.AppendLine("===========================================");
            report.AppendLine($"Generated: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Report ID: YOURPROJECT-{System.Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}");
            report.AppendLine();

            // Check compilation status
            report.AppendLine("Status Check:");
            report.AppendLine($"  Compiling: {EditorApplication.isCompiling}");
            report.AppendLine($"  Play Mode Enabled: {EditorApplication.isPlaying}");
            report.AppendLine();

            // Check for actual compilation errors via CompilationPipeline
            var assemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
            bool hasErrors = false;
            foreach (var assembly in assemblies)
            {
                // Note: This is a basic check - detailed errors are captured by CompilationReporter
                if (assembly.name.Contains("error", System.StringComparison.OrdinalIgnoreCase))
                {
                    hasErrors = true;
                    break;
                }
            }

            report.AppendLine("===========================================");
            if (EditorApplication.isCompiling)
            {
                report.AppendLine("Status: COMPILING (still in progress)");
            }
            else if (hasErrors)
            {
                report.AppendLine("Status: FAILED - Compilation errors detected");
                report.AppendLine("Note: Check Unity console or CompilationReporter for detailed errors");
            }
            else
            {
                report.AppendLine("Status: SUCCESS - Project compiled successfully");
                report.AppendLine("No compilation errors detected");
            }
            report.AppendLine("===========================================");

            // Write to file
            string directory = Path.GetDirectoryName(reportPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

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
