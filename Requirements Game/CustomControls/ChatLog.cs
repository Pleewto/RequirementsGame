using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Scrollable chat log that displays message bubbles for two actors
/// </summary>
public class ChatLog : Panel {

    public enum MessageActor { User, System } // Using an Enum to force only two types of actors

    // Tracks the most recent ChatPanel and its MessageBubble.
    // Used to position the next ChatPanel and decide whether to append or start a new MessageBubble
    private Panel ChatPanel;
    private ChatMessageBubble MessageBubble;

    public ChatLog() {

        this.Margin = new Padding(0);
        this.AutoScroll = true;
        this.HorizontalScroll.Maximum = 0;
        this.HorizontalScroll.Visible = false;
        this.AutoScrollMinSize = new Size(0, this.AutoScrollMinSize.Height);

        this.SizeChanged += ChatMessageBubble_SizeChanged;

    }

    /// <summary>
    /// Appends a message to the chat log, grouping by actor and aligning appropriately
    /// </summary>
    public void SendMessage(string message, MessageActor actor) {

        // Remove whitespace and random characters from start of message

        int messageStartIndex = 0;

        foreach (char character in message.ToCharArray())
        {

            if (char.IsLetterOrDigit(character)) break;

            messageStartIndex++;

        }

        message = message.Substring(messageStartIndex).TrimEnd();

        if (message.Length == 0) message = "...";

        string actorName = actor == MessageActor.System ? "System" : "User";

        // Start a new bubble group if this is the first message or the actor changed

        if (MessageBubble == null || MessageBubble.Name != actorName) {

            int panelLocationY = ChatPanel == null ? 20 : ChatPanel.Location.Y + ChatPanel.Height + 20; // Add spacing after previous group

            MessageBubble = new ChatMessageBubble();
            MessageBubble.Name = actorName;
            MessageBubble.Dock = actor == MessageActor.System ? DockStyle.Left : DockStyle.Right;
            MessageBubble.BackColor = actor == MessageActor.System ? Color.FromArgb(224, 224, 224) : Color.Black;
            MessageBubble.ForeColor = actor == MessageActor.System ? Color.Black : Color.White;

            // A ChatPanel is used per MessageBubble to allow for the MessageBubble
            // to align left or right. This is not possible is the MessageBubble is added
            // straight onto this class control

            ChatPanel = new Panel();
            ChatPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ChatPanel.Size = new Size(this.Width, MessageBubble.Height + 10);
            ChatPanel.Location = new Point(20, panelLocationY);

            this.Controls.Add(ChatPanel);
            ChatPanel.Controls.Add(MessageBubble);

            MessageBubble.SizeChanged += ChatMessageBubble_SizeChanged;
        }

        // Add the message to the chat bubble. If the actor has not changed,
        // the current bubble will be added to, giving the illusion of a single
        // dynamically growing chat bubble

        MessageBubble.Text = message;

    }

    /// <summary>
    /// Clears all chat content and resets scroll position
    /// </summary>
    public void Clear() {

        this.Controls.Clear();
        ChatPanel = null;
        MessageBubble = null;
        this.AutoScrollPosition = new Point(0, 0);

    }

    /// <summary>
    /// Adjust ChatPanel widths to match the controls new size,
    /// which then auto aligns the chat bubbles to the correct new positions
    /// </summary>
    private void ChatMessageBubble_SizeChanged(object sender, EventArgs e) {

        // Match group panel widths to the control width

        foreach (Control control in this.Controls) {

            if (control is Panel panel && panel.Controls.Count > 0) {

                panel.Size = new Size(this.Width - 50, panel.Controls[0].Size.Height);

            }

        }

        // Keep view pinned to the latest message when vertical scroll is visible

        if (this.VerticalScroll.Visible) {

            this.AutoScrollPosition = new Point(0, this.VerticalScroll.Maximum);

        }

    }

    /// <summary>
    /// Internal message bubble class, used to apply defaults properties
    /// </summary>
    private class ChatMessageBubble : CustomLabel {

        public ChatMessageBubble() {

            this.AutoSize = true;
            this.Padding = new Padding(5);
            this.MaximumSize = new Size(400, 0);
            this.Font = new Font("Calibri", 12, FontStyle.Regular);
            this.CornerRadius = 15;

        }

    }
}