using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

/// <summary>
/// Handles reading and writing scenario data to the JSON file stored in the Data folder
/// </summary>
public static class JsonFileManager {

    private static readonly string filePath = FileSystem.ScenariosFilePath;

    /// <summary>
    /// Loads and deserializes the list of scenarios from the specified JSON file
    /// </summary>
    public static List<Scenario> LoadScenarios(string filePath) {

        if (!File.Exists(filePath)) return new List<Scenario>();

        try {

            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<Scenario>>(json, new JsonSerializerOptions {

                PropertyNameCaseInsensitive = true

            }) ?? new List<Scenario>();

        } catch (Exception ex)  {

            // Log error and return an empty list if deserialization fails
            
            Console.WriteLine($"Error loading scenarios: {ex.Message}");
            
            return new List<Scenario>();

        }
    }

    /// <summary>
    /// Serializes and saves the list of scenarios to the specified JSON file
    /// </summary>
    public static bool SaveScenarios(List<Scenario> scenarios, string filePath) {
        
        try {

            // Convert the scenario list to formatted JSON and write it to disk

            string json = JsonSerializer.Serialize(scenarios, new JsonSerializerOptions {WriteIndented = true});

            File.WriteAllText(filePath, json);

            return true;

        } catch (Exception ex) {

            // Log error and return false if saving fails

            Console.WriteLine($"Error saving scenarios: {ex.Message}");
            
            return false;

        }

    }

}