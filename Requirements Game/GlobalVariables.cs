using System;
using System.Collections.Generic;
using System.Drawing;

class GlobalVariables {

    // Place holders, please change to what looks better

    static public Requirements_Game.Form1 MainForm { get; set; }

    static public string AppFontName { get => "Calibri"; }

    static public Color ColorPrimary { get => Color.White; }
    static public Color ColorLight { get => Color.FromArgb(235, 235, 235); }
    static public Color ColorMedium { get => Color.FromArgb(205, 205, 205); }
    static public Color ColorDark { get => Color.FromArgb(0, 0, 0); }
    static public Color ColorButtonBlack { get => Color.FromArgb(40, 40, 40); }

    public static Scenario CurrentScenario { get; set; }

    public static string PersonaKey(Scenario s, Stakeholder p)
        => $"{s?.Name ?? "(no-scenario)"}|{p?.Name ?? "(no-persona)"}";

    // UI transcript entry
    public class ChatMsg {
        public ChatLog.MessageActor Actor;
        public string Text;
        public ChatMsg(ChatLog.MessageActor actor, string text) { Actor = actor; Text = text; }
    }

    // Per-persona chat history (personaKey -> ordered list of messages)
    public static Dictionary<string, List<ChatMsg>> PersonaChatLogs
        = new Dictionary<string, List<ChatMsg>>();

    // Per-persona live streaming buffer (personaKey -> latest partial reply)
    public static Dictionary<string, string> PersonaLiveReply
        = new Dictionary<string, string>();

    // personaKey -> list of (Type, Text) tuples for the student's draft requirements
    public static Dictionary<string, List<(string Type, string Text)>> PersonaRequirements
        = new Dictionary<string, List<(string Type, string Text)>>();


}

// Specifies how a button visually responds when interacted with (hover and click)
public enum ButtonInteractionEffect {
    Lighten,
    Darken,
    None
}