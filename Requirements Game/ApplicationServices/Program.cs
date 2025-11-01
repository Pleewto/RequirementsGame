using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Requirements_Game
{

    /// <summary>
    /// Application entry point for the Requirements Elicitation Game.
    /// Handles startup initialization, data loading, and shutdown
    /// </summary>
    internal static class Program
    {

        [STAThread]
        static void Main()
        {

            // -- Initialize Application Settings --
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // -- Load Scenario Data --
            // Loads all scenario files needed before the main form starts
            Scenarios.LoadFromFile(FileSystem.ScenariosFilePath);

            // -- Application Exit Handling --
            // Ensures the local LLM server is stopped when the application closes
            Application.ApplicationExit += (s, e) => {

                try { LLMServerClient.StopServer(); }
                catch (Exception ex) { Debug.WriteLine("Error shutting down LLM server: " + ex.Message); }

            };

            // -- Start Main Form --
            Application.Run(new Form1());

        }

    }

}