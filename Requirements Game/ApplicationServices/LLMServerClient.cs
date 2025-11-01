using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// Manages all communication between the application and the locally hosted LLM server.
/// Handles model selection based on available system memory, starts and stops the server process,
/// and maintains multi-persona conversation histories.  
///  
/// Provides asynchronous chat functionality with streaming partial responses,  
/// allowing each persona (stakeholder) to maintain independent context across interactions.  
/// </summary>
public class LLMServerClient
{

    private static LLMServerClient ServerClient; // Singleton instance of the LLMServerClient
    private Process llamaProcess; // Reference to the running llama-server process

    private StringBuilder conversationHistory; // Maintains the ongoing chat log for the active persona
    private readonly Dictionary<string, StringBuilder> histories = new Dictionary<string, StringBuilder>(); // Stores conversation history per persona
    private string activeKey = null; // Key identifying the current active persona context

    public static bool IsBusy { get; private set; } // Indicates if the LLM is currently processing a request

    // -- Static Constructor --
    // Initializes the singleton instance and sets the busy flag default.
    static LLMServerClient()
    {

        ServerClient = new LLMServerClient();
        IsBusy = false;

    }

    /// <summary>
    /// Method that exposes the ServerClinent.StopServer method so that it
    /// can be called by ..
    /// <summary>
    public static void StopServer()
    {

        ServerClient.StopServer_Internal();

    }

    /// <summary>
    // Creates a new LLMServerClient with default parameters:
    // 6 processing threads and a 1024 token context size
    /// <summary>
    public LLMServerClient() : this(6, 1024)
    {
    }

    /// <summary>
    // Initializes the local LLM server based on available system memory,
    // automatically selecting an appropriate model size and launching
    // the llama-server process if not already running
    /// <summary>
    public LLMServerClient(int threads, int tokenSize)
    {

        // -- Available Memory --

        double totalRam = ComputerInfo.GetTotalMemory();
        double availableRamMB = ComputerInfo.GetAvailableMemory();

        // -- Model Selection --

        string modelPath = "";

        if (availableRamMB < 4096)
        { // 4GB

            // Show message to user if there memory is less than 4gb, as they may lack the required resources

            VisualMessageManager.ShowMessage(
                "Your computer currently has less than 4 GB of available RAM, which may affect the LLM’s performance. " +
                "If your system has more total RAM than what is currently available, try closing other applications " +
                "and restarting this program to improve performance");

            modelPath = $"{FileSystem.ModelsFolderPath}\\gemma-3-1b-it-QAT-Q4_0.gguf";

        }
        else if (availableRamMB < 8192)
        { // 8GB

            modelPath = $"{FileSystem.ModelsFolderPath}\\gemma-3-4b-it-Q4_K_M.gguf";

        }
        else
        { // if (availableRamMB < 16384) { // 16GB

            modelPath = $"{FileSystem.ModelsFolderPath}\\gemma-3-12b-it-Q4_K_M.gguf";

        }

        Debug.WriteLine($"[LLM] RAM avail MB = {availableRamMB}, modelPath = {modelPath}");

        // Throw error if model is not found in folder

        if (!File.Exists(modelPath)) throw new FileNotFoundException($"Model not found at: {modelPath}");

        // -- Existing Server Check --
        // Prevents launching multiple instances of the LLM server.

        var existingProcesses = Process.GetProcessesByName("llama-server");

        if (existingProcesses.Length > 0)
        {

            llamaProcess = existingProcesses[0];
            return;

        }

        // -- Launch Llama Server --

        string serverPath = Path.Combine(Application.StartupPath, "llama-b5995-bin-win-cpu-x64", "llama-server.exe");
        string arguments = $"--model \"{modelPath}\" --threads {threads} --ctx-size {tokenSize}";

        var psi = new ProcessStartInfo();
        psi.FileName = serverPath;
        psi.Arguments = arguments;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        llamaProcess = Process.Start(psi);

    }

    /// <summary>
    /// Activates a persona context for conversation.  
    /// If the specified persona does not already exist, a new entry is created
    /// and initialized with the provided system prompt.  
    /// </summary>
    public static void ActivatePersona(string personaKey, string systemPrompt)
    {

        ServerClient.ActivatePersonaInternal(personaKey, systemPrompt);

    }

    /// <summary>
    /// Internal handler for activating or initializing a persona context.  
    /// Ensures the persona exists in the conversation history dictionary
    /// and assigns it as the active context.
    /// </summary>
    private void ActivatePersonaInternal(string personaKey, string systemPrompt)
    {

        if (string.IsNullOrWhiteSpace(personaKey)) personaKey = "default";
        activeKey = personaKey;

        if (!histories.TryGetValue(activeKey, out var _))
            histories[activeKey] = new StringBuilder($"<|system|> {systemPrompt} ");

    }

    /// <summary>
    /// Resets the conversation history for a specific persona, without
    /// affecting other personas.  
    /// The persona is reinitialized with the provided system prompt.
    /// </summary>
    public static void ResetConversationForPersona(string personaKey, string systemPrompt)
    {

        ServerClient.ResetConversationInternal(personaKey, systemPrompt);

    }

    /// <summary>
    /// Internal method that clears and reinitializes the specified persona’s conversation history.  
    /// Sets the given persona as the active conversation context.
    /// </summary>
    private void ResetConversationInternal(string personaKey, string systemPrompt)
    {

        if (string.IsNullOrWhiteSpace(personaKey)) personaKey = "default";
        histories[personaKey] = new StringBuilder($"<|system|> {systemPrompt} ");
        activeKey = personaKey;

    }

    /// <summary>
    /// Sends a message to the active LLM persona and streams back partial responses in real time.  
    /// Ensures that only one request is processed at a time and updates the persona’s live reply buffer.  
    /// </summary>
    public static async void SendMessage(string personaKey, string question)
    {

        // -- Busy Check --
        // Prevents multiple overlapping requests to the LLM

        if (IsBusy) throw new Exception("LLM is busy");

        IsBusy = true;

        // -- Validate Persona Key --
        // Set as default if one does not exist

        if (string.IsNullOrWhiteSpace(personaKey)) personaKey = "default";

        // -- Activate Persona --
        // Ensures this persona context exists; if not, initializes with a default system prompt

        ServerClient.ActivatePersonaInternal(personaKey, "You are a helpful assistant.");

        // -- Reset Live Reply Buffer --
        // Clears any previous streamed response text for this persona

        GlobalVariables.PersonaLiveReply[personaKey] = "";

        // -- Send Message and Stream Partial Responses --

        await ServerClient.Chat(question, partial => {

            // Initialize buffer if missing (safety check)

            if (!GlobalVariables.PersonaLiveReply.ContainsKey(personaKey))
                GlobalVariables.PersonaLiveReply[personaKey] = "";

            // Append streamed content to live buffer

            GlobalVariables.PersonaLiveReply[personaKey] += partial;

        });

        // -- Reset Busy Flag --

        IsBusy = false;

    }

    /// <summary>
    /// Sends a chat request to the locally hosted LLM server and streams back the response asynchronously.  
    /// Maintains conversation continuity for the active persona and updates the conversation history
    /// with both the user’s prompt and the assistant’s reply.  
    /// Supports real-time streaming of partial responses through a callback delegate
    /// </summary>
    /// <param name="question">The user’s message or prompt to send to the model</param>
    /// <param name="onPartialResponse">Optional callback invoked for each partial text fragment received from the LLM stream</param>
    /// <returns>The full assistant response once streaming is complete</returns>
    public async Task<string> Chat(string question, Action<string> onPartialResponse = null)
    {

        // -- Ensure Active Persona --
        // Create a default persona if none is currently active

        if (string.IsNullOrEmpty(activeKey))
            ActivatePersonaInternal("default", "You are a helpful assistant.");

        // Append question to conversation history

        var conversationHistory = histories[activeKey];
        conversationHistory.Append($"<|user|> {question} ");

        // -- Define Request Endpoint --

        string url = "http://localhost:8080/completion";

        // -- Build JSON Request Body --

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

        // -- Send Request --

        try
        {

            // -- Send Request to Local Server --

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

            // Use streaming mode to process responses as they arrive

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);

            string fullResponse = "";

            while (!reader.EndOfStream)
            {

                var line = await reader.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(line))
                {

                    try
                    {

                        var json = JsonDocument.Parse(line.Replace("data: ", "")); // Parse JSON lines and extract assistant content tokens

                        if (json.RootElement.TryGetProperty("content", out var contentProp))
                        {

                            string partial = contentProp.GetString();

                            fullResponse += partial;

                            onPartialResponse?.Invoke(partial); // Invoke callback for real-time UI updates

                        }

                    }
                    catch (Exception ex)
                    {

                        Debug.Print(ex.Message);

                    }
                }
            }

            // -- Append Full Response to Conversation --

            conversationHistory.Append($" <|assistant|> {fullResponse} ");

            return fullResponse;

        }
        catch (Exception ex)
        {

            return $"Error: {ex.Message}";

        }

    }

    /// <summary>
    /// Terminates the running LLM server process and releases its system resources.
    /// </summary>
    public void StopServer_Internal()
    {

        llamaProcess.Kill();
        llamaProcess.Dispose();

    }

}