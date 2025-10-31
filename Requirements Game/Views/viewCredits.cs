using System;
using System.Drawing;
using System.Windows.Forms;
using Requirements_Game;

/// <summary>
/// Credits view — displays developer names, GitHub links, and acknowledgments
/// Inherits from View
/// </summary>
public class ViewCredits : View
{

    public ViewCredits()
    {

        // -- Base Layout Setup --
        // Layout: Main Content and Footer

        ViewTableLayoutPanel.Dock = DockStyle.Fill;
        ViewTableLayoutPanel.BackColor = GlobalVariables.ColorPrimary;

        ViewTableLayoutPanel.ColumnCount = 3;
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f)); // Left padding
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Center content
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f)); // Right padding

        ViewTableLayoutPanel.RowCount = 2;
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90f)); // Main content
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10f)); // Footer

        // -- Content Section --
        // Five rows to holder developer names and github links

        var contentPanel = new TableLayoutPanel();
        contentPanel.ColumnCount = 1;
        contentPanel.RowCount = 5;
        contentPanel.AutoSize = true;
        contentPanel.BackColor = GlobalVariables.ColorPrimary;
        contentPanel.Anchor = AnchorStyles.None;

        contentPanel.Controls.Add(CreateHeaderLabel("Developed by:"), 0, 0);
        contentPanel.Controls.Add(CreateNameWithLink("Courtney Hemmett", "https://github.com/Pleewto"), 0, 1);
        contentPanel.Controls.Add(CreateNameWithLink("Jarron Eckford", "https://github.com/Jeckford"), 0, 2);
        contentPanel.Controls.Add(CreateNameWithLink("Cory Crombie", "https://github.com/KorraOne"), 0, 3);
        contentPanel.Controls.Add(CreateNameWithLink("Mai Le", "https://github.com/ttle11"), 0, 4);

        ViewTableLayoutPanel.Controls.Add(contentPanel, 1, 0);

        // -- Footer Section --

        var footerPanel = new TableLayoutPanel();

        footerPanel.ColumnCount = 1;
        footerPanel.RowCount = 2;
        footerPanel.AutoSize = true;
        footerPanel.BackColor = GlobalVariables.ColorPrimary;
        footerPanel.Anchor = AnchorStyles.None;

        footerPanel.Controls.Add(CreateFooterLabel("29/10/2025 – ECU"), 0, 0);
        footerPanel.Controls.Add(CreateFooterLabel("Thank you to Martin and Luke"), 0, 1);

        ViewTableLayoutPanel.Controls.Add(footerPanel, 1, 1);

    }

    /// <summary>
    /// Function to create and return Header label
    /// </summary>
    private Label CreateHeaderLabel(string text)
    {

        return new Label
        {
            Text = text,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(GlobalVariables.AppFontName, 20, FontStyle.Bold),
            ForeColor = GlobalVariables.ColorDark,
            AutoSize = true,
            Anchor = AnchorStyles.None
        };
    }

    /// <summary>
    /// Function to create and return Name with Link label
    /// </summary>
    private LinkLabel CreateNameWithLink(string name, string url)
    {

        var linkLabel = new LinkLabel
        {
            Text = name,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Regular),
            LinkColor = GlobalVariables.ColorDark,
            ActiveLinkColor = GlobalVariables.ColorDark,
            LinkBehavior = LinkBehavior.NeverUnderline,
            BackColor = GlobalVariables.ColorPrimary,
            ForeColor = GlobalVariables.ColorDark,
            Cursor = Cursors.Hand,
            AutoSize = true,
            Anchor = AnchorStyles.None
        };

        linkLabel.Links.Add(0, name.Length, url);

        // Tooltip

        var tooltip = new ToolTip();
        tooltip.SetToolTip(linkLabel, "View GitHub profile");

        // Open link in default browser when clicked

        linkLabel.LinkClicked += (s, e) => {

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {

                FileName = e.Link.LinkData.ToString(),
                UseShellExecute = true

            });

        };

        return linkLabel;
    }

    /// <summary>
    /// Function to create and return Footer label
    /// </summary>
    private Label CreateFooterLabel(string text)
    {
        return new Label
        {
            Text = text,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(GlobalVariables.AppFontName, 10, FontStyle.Italic),
            ForeColor = Color.Gray,
            AutoSize = true,
            Anchor = AnchorStyles.None
        };
    }
}