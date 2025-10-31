using System;
using System.IO;

/// <summary>
/// Handles setup and directory management for the application,
/// including creating required folders and copying default resources on first installation
/// </summary>
class FileSystem {

    public static string InstallDirectory => AppDomain.CurrentDomain.BaseDirectory;
    public static string AppFolderPath { get; }
    public static string ModelsFolderPath { get; }
    public static string ScenariosFilePath { get; }

    static FileSystem() {

        // Define application data folder structure

        AppFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Requirement Game");
        ModelsFolderPath = Path.Combine(AppFolderPath, "Models");
        ScenariosFilePath = Path.Combine(AppFolderPath, "Scenarios.json");

        // Ensure application directories exist

        Directory.CreateDirectory(AppFolderPath);
        Directory.CreateDirectory(ModelsFolderPath);

        // Define bundled resource paths located in the installation directory

        string bundledScenarioPath = Path.Combine(InstallDirectory, "Resources", "DefaultScenarios.json");
        string bundledModelsPath = Path.Combine(InstallDirectory, "Models");

        // Exit early if not first installation (scenarios file already exists)

        if (File.Exists(ScenariosFilePath)) return;

        // Copy the bundled scenario file if available

        if (File.Exists(bundledScenarioPath)) File.Copy(bundledScenarioPath, ScenariosFilePath);

        // Copy each model file from installer to app location if folder exists

        if (!Directory.Exists(bundledModelsPath)) return;

        foreach (string modelFile in Directory.GetFiles(bundledModelsPath)) {

            string fileName = Path.GetFileName(modelFile);
            string destPath = Path.Combine(ModelsFolderPath, fileName);

            // Only copy if the file doesn’t already exist in the destination

            if (!File.Exists(destPath)) File.Copy(modelFile, destPath);

        }

    }

}