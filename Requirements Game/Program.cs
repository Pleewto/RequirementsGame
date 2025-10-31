using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Requirements_Game
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Load data needed at startup
            Scenarios.LoadFromFile(FileSystem.ScenariosFilePath);
            Debug.WriteLine($"[Startup] Loaded {Scenarios.GetScenarios().Length} scenarios.");

            Application.ApplicationExit += (s, e) =>
            {
                LLMServerClient.Shutdown();
            };

            Application.Run(new Form1());
        }
    }
}