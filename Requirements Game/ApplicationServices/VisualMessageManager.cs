using System.Windows.Forms;
using System.Drawing;
using Requirements_Game.Properties;

/// <summary>
/// Displays stacked notifications at the bottom of the main form
/// </summary>
class VisualMessageManager {

    // Borderless overlay form that hosts all message panels
    private static Form MessageForm;

    static VisualMessageManager() {

        MessageForm = new Form();
        MessageForm.FormBorderStyle = FormBorderStyle.None;
        MessageForm.BackColor = Color.AliceBlue;
        MessageForm.TransparencyKey = Color.AliceBlue; // Make AliceBlue (form color) transparent
        MessageForm.Owner = GlobalVariables.MainForm;
        MessageForm.AutoScroll = false;
        MessageForm.Show();

        UpdateMessageFormPosition();

        // Keep overlay anchored to bottom-center of the main form
        GlobalVariables.MainForm.Move += (s, e) => UpdateMessageFormPosition();
        GlobalVariables.MainForm.Resize += (s, e) => UpdateMessageFormPosition();

    }

    /// <summary>
    /// Sizes and positions the overlay form to be bottom-centered over the main form
    /// and sets its height to fit all child message panels
    /// </summary>
    static void UpdateMessageFormPosition() {

        // Width = two-thirds of main form

        MessageForm.Width = GlobalVariables.MainForm.Width / 3 * 2;

        var requiredHeight = 0;

        // Compute required height from children (panels + their margins)

        foreach (Control control in MessageForm.Controls) {

            requiredHeight += control.Height;
            requiredHeight += control.Margin.Top;
            requiredHeight += control.Margin.Bottom;

        }

        MessageForm.Height = requiredHeight;

        // Bottom-center the overlay with a small gap

        MessageForm.Location = new Point(
            GlobalVariables.MainForm.Left + (GlobalVariables.MainForm.Width - MessageForm.Width) / 2,
            GlobalVariables.MainForm.Bottom - MessageForm.Height - 10
        );

    }

    /// <summary>
    /// Adds a new message panel to the overlay; can auto-close after a delay
    /// </summary>
    public static void ShowMessage(string Message, bool AutoClose = false) {

        // Rounded TableLayoutPanel to structure the new message
        // 1 row, 3 columns: [message | close button | right padding]

        var messageTableLayoutPanel = new CustomTableLayoutPanel();
        messageTableLayoutPanel.CornerRadius = 10;
        messageTableLayoutPanel.Margin = new Padding(0);
        messageTableLayoutPanel.Dock = DockStyle.Bottom;
        messageTableLayoutPanel.BackColor = GlobalVariables.ColorMedium;
        messageTableLayoutPanel.AutoSize = true;

        messageTableLayoutPanel.RowCount = 1;
        messageTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        messageTableLayoutPanel.ColumnCount = 3;
        messageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        messageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
        messageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

        MessageForm.Controls.Add(messageTableLayoutPanel);

        // Add a panel to create space between stacked messages vertically

        var spacer = new Panel();
        spacer.BackColor = Color.AliceBlue;
        spacer.Size = new Size(5, 5);
        spacer.Margin = new Padding(0);
        spacer.Dock = DockStyle.Bottom;
        MessageForm.Controls.Add(spacer);

        // MessageLabel

        var messageLabel = new Label();
        messageLabel.Text = Message;
        messageLabel.Font = new Font(GlobalVariables.AppFontName, 11, FontStyle.Regular);
        messageLabel.TextAlign = ContentAlignment.MiddleLeft;
        messageLabel.Dock = DockStyle.Fill;
        messageLabel.Margin = new Padding(10);
        messageLabel.AutoSize = true;
        messageTableLayoutPanel.Controls.Add(messageLabel, 0, 0);

        // Close button (optional when not auto-closing)

        var closeIcon = (Bitmap)Resources.ResourceManager.GetObject("close");
        closeIcon = new Bitmap(closeIcon, 20, 20); // Shrinking the image size

        var closeButton = new CustomPictureBox();
        closeButton.Dock = DockStyle.Fill;
        closeButton.SizeMode = PictureBoxSizeMode.CenterImage;
        closeButton.Image = closeIcon;
        closeButton.InteractionEffect = ButtonInteractionEffect.Lighten;
    
        closeButton.Click += (sender, e) => { // Remove the message panel when user clicks the close button

            messageTableLayoutPanel.Dispose();
            UpdateMessageFormPosition();

        };

        messageTableLayoutPanel.Controls.Add(closeButton, 1, 0);

        // Auto-close logic

        if (AutoClose) {

            closeButton.Dispose(); // Hide the manual close button for auto-closing messages

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;

            // Tick event which will fire after 3 seconds
            // which will dispose (close) the message

            timer.Tick += (sender, e) => {

                timer.Stop();
                timer.Dispose();

                if (!messageTableLayoutPanel.IsDisposed) {

                    messageTableLayoutPanel.Dispose();
                    UpdateMessageFormPosition();

                }

            };

            timer.Start();

        }

        // Call to account for the new message

        UpdateMessageFormPosition();

    }

}
