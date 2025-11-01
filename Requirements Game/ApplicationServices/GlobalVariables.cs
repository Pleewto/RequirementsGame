using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// Provides globally accessible variables, shared data structures, and configuration settings
/// used throughout the application.
/// Acts as a centralised point for managing global context between different views and components.
/// </summary>
class GlobalVariables
{

    // Reference to the main form instance

    static public Requirements_Game.Form1 MainForm { get; set; }

    // -- Global UI Settings --

    static public string AppFontName { get => "Calibri"; }
    static public Color ColorPrimary { get => Color.White; }
    static public Color ColorLight { get => Color.FromArgb(235, 235, 235); }
    static public Color ColorMedium { get => Color.FromArgb(205, 205, 205); }
    static public Color ColorDark { get => Color.FromArgb(0, 0, 0); }
    static public Color ColorButtonBlack { get => Color.FromArgb(40, 40, 40); }

    // -- Active Scenario --

    public static Scenario CurrentScenario { get; set; }


    // -- Persona Key Generator --
    // Creates a unique identifier string for persona-specific data mappings

    public static string PersonaKey(Scenario s, Stakeholder p)
        => $"{s?.Name ?? "(no-scenario)"}|{p?.Name ?? "(no-persona)"}";

    // -- Chat Message Class --
    // Represents a single chat entry for display or logging

    public class ChatMsg
    {
        public ChatLog.MessageActor Actor;
        public string Text;
        public ChatMsg(ChatLog.MessageActor actor, string text) { Actor = actor; Text = text; }
    }

    // -- Persona Chat Logs --
    // Stores chat history per persona (personaKey → list of messages)

    public static Dictionary<string, List<ChatMsg>> PersonaChatLogs
        = new Dictionary<string, List<ChatMsg>>();

    // -- Live Streaming Replies --
    // Buffers ongoing partial responses for each persona (personaKey → text buffer)

    public static Dictionary<string, string> PersonaLiveReply
        = new Dictionary<string, string>();

    // -- Persona Draft Requirements --
    // Stores requirement entries being drafted during interviews
    // (personaKey → list of requirement type/text tuples)

    public static Dictionary<string, List<(string Type, string Text)>> PersonaRequirements
        = new Dictionary<string, List<(string Type, string Text)>>();

}

// -- Button Interaction Effect --
// Defines how a button visually responds to user interactions
public enum ButtonInteractionEffect
{
    Lighten,
    Darken,
    None
}