using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// A composite control that pairs a titled Label with a bordered RichTextBox.
/// The control itself is a CustomTableLayoutPanel so it can be added directly onto forms or other controls
/// </summary>
class CustomLabelledRichTextBox : CustomTableLayoutPanel {

    private CustomLabel NameLabel;
    private RichTextBox TextBox;

    public CustomLabelledRichTextBox() {

        this.Margin = new Padding(0);
        this.Padding = new Padding(0);
        this.AutoSize = true;

        this.ColumnCount = 1;
        this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        this.RowCount = 2;
        this.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Label row
        this.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // RichTextBox row

        // Title label

        NameLabel = new CustomLabel();
        NameLabel.Text = "Text";
        NameLabel.Font = new Font(GlobalVariables.AppFontName, 12, FontStyle.Bold);
        NameLabel.Dock = DockStyle.Top;
        NameLabel.AutoSize = true;
        NameLabel.Padding = new Padding(0,8,0,3);
        NameLabel.Margin = new Padding(0);
        this.Controls.Add(NameLabel, 0, 0);

        // Outer border panel
        // RichTextBox has limited border colour options; this panel provides a custom-coloured border

        Panel borderPanel = new Panel();
        borderPanel.Dock = DockStyle.Fill;
        borderPanel.BackColor = GlobalVariables.ColorMedium;
        borderPanel.Padding = new Padding(2); // Border thickness
        borderPanel.Margin = new Padding(0);
        borderPanel.AutoSize = true;
        borderPanel.AutoScroll = false;
        this.Controls.Add(borderPanel, 0, 1);

        // Inner panel (text padding)
        // RichTextBox draws text hard against its edges; this panel simulates "padding" for the text

        Panel innerBorderPanel = new Panel();
        innerBorderPanel.Dock = DockStyle.Fill;
        innerBorderPanel.BackColor = GlobalVariables.ColorPrimary;
        innerBorderPanel.Padding = new Padding(3); // Text padding
        innerBorderPanel.Margin = new Padding(0);
        innerBorderPanel.AutoSize = true;
        innerBorderPanel.AutoScroll = false;
        borderPanel.Controls.Add(innerBorderPanel);

        // Richtextbox

        TextBox = new RichTextBox();
        TextBox.BorderStyle = BorderStyle.None;
        TextBox.Dock = DockStyle.Top;
        TextBox.Margin = new Padding(3);
        TextBox.Multiline = false;
        TextBox.Font = new Font(GlobalVariables.AppFontName, 12, FontStyle.Regular);
        TextBox.Margin = new Padding(0, 0, 0, 2);
        innerBorderPanel.Controls.Add(TextBox);

        // Give the richtextbox a default height of a single line

        this.TextBoxRowCount = 1;

    }

    /// <summary>
    /// Write-only: sets the desired number of text rows for the RichTextBox
    /// by multiplying a single-line height by the specified value.
    /// </summary>
    public int TextBoxRowCount {

        set { TextBox.Height = TextRenderer.MeasureText("Ag", TextBox.Font).Height * value; }

    }

    /// <summary>
    /// Controls whether the underlying RichTextBox supports multiple lines
    /// </summary>
    public bool Multiline {

        get { return TextBox.Multiline; }
        set { TextBox.Multiline = value; }

    }

    /// <summary>
    /// Gets or sets the title displayed above the text box
    /// </summary>
    public string LabelText {
    
        get { return NameLabel.Text; }
        set { NameLabel.Text = value; }
    
    }

    /// <summary>
    /// Gets or sets the text content of the underlying RichTextBox
    /// </summary>
    public string TextboxText {

        get { return TextBox.Text; }
        set { TextBox.Text = value; }

    }

}
