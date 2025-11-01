using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

public class LLMServerClient {

    private static LLMServerClient ServerClient;

    public static bool IsBusy { get; private set; }

    static LLMServerClient() {

        ServerClient = new LLMServerClient();
        IsBusy = false;

    }

    public static void Shutdown()
    {
        try
        {
            ServerClient.StopServer();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error shutting down LLM server: " + ex.Message);
        }
    }

    // Selects (or creates) the active persona and ensures it has a system prompt
    // personaKey example: $"{scenarioName}|{personaName}"
    public static void ActivatePersona(string personaKey, string systemPrompt)
    {
        ServerClient.ActivatePersonaInternal(personaKey, systemPrompt);
    }

    private void ActivatePersonaInternal(string personaKey, string systemPrompt)
    {
        if (string.IsNullOrWhiteSpace(personaKey)) personaKey = "default";
        activeKey = personaKey;

        if (!histories.TryGetValue(activeKey, out var _))
            histories[activeKey] = new StringBuilder($"<|system|> {systemPrompt} ");
    }

    // Hard reset only that persona's conversation (keeps others)
    public static void ResetConversationForPersona(string personaKey, string systemPrompt)
    {
        ServerClient.ResetConversationInternal(personaKey, systemPrompt);
    }

    private void ResetConversationInternal(string personaKey, string systemPrompt)
    {
        if (string.IsNullOrWhiteSpace(personaKey)) personaKey = "default";
        histories[personaKey] = new StringBuilder($"<|system|> {systemPrompt} ");
        activeKey = personaKey;
    }


    public static async void SendMessage(string personaKey, string question) {

        if (IsBusy) throw new Exception("LLM is busy");

        IsBusy = true;

        if (string.IsNullOrWhiteSpace(personaKey)) personaKey = "default";

        // Use the stored prompt for this persona if we have it
        // otherwise, fall back to the generic
        ServerClient.ActivatePersonaInternal(personaKey, "You are a helpful assistant.");

        // Reset only this persona’s live buffer
        GlobalVariables.PersonaLiveReply[personaKey] = "";

        await ServerClient.Chat(question, partial => {
            if (!GlobalVariables.PersonaLiveReply.ContainsKey(personaKey))
                GlobalVariables.PersonaLiveReply[personaKey] = "";
            GlobalVariables.PersonaLiveReply[personaKey] += partial;
        });

        IsBusy = false;

    }

    
    private Process llamaProcess;
    private StringBuilder conversationHistory;

    // Persona aware conversation state
    private readonly Dictionary<string, StringBuilder> histories = new Dictionary<string, StringBuilder>();
    private string activeKey = null;

    public LLMServerClient() : this(6, 1024) {
    }

    public LLMServerClient(int threads, int tokenSize) {

        // Initialise conversation        

        // Get model based on available RAM

        double totalRam = ComputerInfo.GetTotalMemory();
        double availableRamMB = ComputerInfo.GetAvailableMemory();
        string modelPath = "";

        if (availableRamMB < 4096) { // 4GB

            VisualMessageManager.ShowMessage("Your computer currently has less than 4 GB of available RAM, which may affect the LLM’s performance. If your system has more total RAM than what is currently available, try closing other applications and restarting this program to improve performance");

            modelPath = $"{FileSystem.ModelsFolderPath}\\gemma-3-4b-it-QAT-Q4_0.gguf";

        } else if (availableRamMB < 8192) { // 8GB

            modelPath = $"{FileSystem.ModelsFolderPath}\\gemma-3-4b-it-Q4_K_M.gguf";

        } else { // if (availableRamMB < 16384) { // 16GB

            modelPath = $"{FileSystem.ModelsFolderPath}\\gemma-3-12b-it-Q4_K_M.gguf";

        } /* else { // 16GB or more

            modelPath = $"{FileSystem.ModelsFolderPath}\\gemma-3-27B-it-QAT-Q4_0.gguf";

        } */

        Debug.WriteLine($"[LLM] RAM avail MB = {availableRamMB}, modelPath = {modelPath}");

        if (string.IsNullOrWhiteSpace(modelPath))
            throw new InvalidOperationException(
                $"Model path was not selected. Available RAM (MB): {availableRamMB}");

        if (!File.Exists(modelPath))
            throw new FileNotFoundException($"Model not found at: {modelPath}");


        // Check if server is already running, exit if so

        var existingProcesses = Process.GetProcessesByName("llama-server");

        if (existingProcesses.Length > 0) {

            llamaProcess = existingProcesses[0];
            return;

        }


        // Start server

        string serverPath = Path.Combine(Application.StartupPath, "llama-b5995-bin-win-cpu-x64", "llama-server.exe");
        string arguments = $"--model \"{modelPath}\" --threads {threads} --ctx-size {tokenSize}";

        var psi = new ProcessStartInfo {

            FileName = serverPath,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true

        };

        llamaProcess = Process.Start(psi);

    }

    public void StopServer() {

        llamaProcess.Kill();
        llamaProcess.Dispose();

    }

    public async Task<string> Chat(string question, Action<string> onPartialResponse = null) {

        // ensure an active persona exists (fallback "default")
        if (string.IsNullOrEmpty(activeKey))
            ActivatePersonaInternal("default", "You are a helpful assistant.");

        var conversationHistory = histories[activeKey];

        conversationHistory.Append($"<|user|> {question} ");

        string url = "http://localhost:8080/completion";

        var jsonBody = new JsonBuilder();
        jsonBody.Items.Add("prompt", $"{conversationHistory} <|assistant|>");
        jsonBody.Items.Add("n_predict", 2048);
        jsonBody.Items.Add("temperature", 0.2);
        jsonBody.Items.Add("top_k", 20);
        jsonBody.Items.Add("top_p", 0.8);
        jsonBody.Items.Add("stop", new string[] { "<|user|>", "<|system|>", "<|assistant|>" });
        jsonBody.Items.Add("stream", true);
        jsonBody.Items.Add("repeat_penalty", 1.1);

        var content = new StringContent(jsonBody.ToString(), Encoding.UTF8, "application/json");

        try {

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);

            string fullResponse = "";

            while (!reader.EndOfStream) {

                var line = await reader.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(line)) {

                    try {

                        var json = JsonDocument.Parse(line.Replace("data: ", ""));

                        if (json.RootElement.TryGetProperty("content", out var contentProp)) {

                            string partial = contentProp.GetString();

                            fullResponse += partial;
                                                     

                            onPartialResponse?.Invoke(partial);

                        }

                    } catch (Exception ex) {

                        Debug.Print(ex.Message);

                    }
                }
            }

            conversationHistory.Append($" <|assistant|> {fullResponse} ");

            return fullResponse;

        } catch (Exception ex) {

            return $"Error: {ex.Message}";

        }

    }

}