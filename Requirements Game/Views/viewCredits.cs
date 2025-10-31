using System;
using System.Drawing;
using System.Windows.Forms;
using Requirements_Game;

public class ViewCredits : View
{
    public ViewCredits()
    {
        // Full layout fills available space
        ViewTableLayoutPanel.Dock = DockStyle.Fill;
        ViewTableLayoutPanel.Padding = new Padding(10);
        ViewTableLayoutPanel.BackColor = GlobalVariables.ColorPrimary;

        ViewTableLayoutPanel.ColumnCount = 3;
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 900f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        ViewTableLayoutPanel.RowCount = 3;
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45f));   // top spacer
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // main content
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 55f));   // footer

        // Main content panel (centered)
        var contentPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = GlobalVariables.ColorPrimary,
            AutoSize = true,
            Anchor = AnchorStyles.None
        };

        contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // "Developed by:"
        contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Courtney
        contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Jarron
        contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Cory
        contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Mai

        contentPanel.Controls.Add(CreateHeaderLabel("Developed by:"), 0, 0);
        contentPanel.Controls.Add(CreateNameWithLink("Courtney Hemmett", "https://github.com/Pleewto"), 0, 1);
        contentPanel.Controls.Add(CreateNameWithLink("Jarron Eckford", "https://github.com/Jeckford"), 0, 2);
        contentPanel.Controls.Add(CreateNameWithLink("Cory Crombie", "https://github.com/KorraOne"), 0, 3);
        contentPanel.Controls.Add(CreateNameWithLink("Mai Le", "https://github.com/ttle11"), 0, 4);

        ViewTableLayoutPanel.Controls.Add(contentPanel, 1, 1);

        // Footer panel pinned to bottom
        var footerPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = GlobalVariables.ColorPrimary,
            Padding = new Padding(0),
            Margin = new Padding(0),
            AutoSize = true
        };

        footerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        footerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        footerPanel.Controls.Add(CreateFooterLabel(DateTime.Now.ToString("dd MMMM yyyy") + " – ECU"), 0, 0);
        footerPanel.Controls.Add(CreateFooterLabel("Thank you to Martin and Luke"), 0, 1);

        ViewTableLayoutPanel.Controls.Add(footerPanel, 1, 2);
    }

    private Label CreateHeaderLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(GlobalVariables.AppFontName, 20, FontStyle.Bold),
            ForeColor = GlobalVariables.ColorDark,
            AutoSize = true
        };
    }

    private LinkLabel CreateNameWithLink(string name, string url)
    {
        string fullText = $"{name} — GitHub";
        int linkStart = fullText.IndexOf("GitHub");

        var linkLabel = new LinkLabel
        {
            Text = fullText,
            Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Regular),
            LinkColor = Color.Blue,
            ActiveLinkColor = Color.DarkBlue,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            AutoSize = true,
            BackColor = GlobalVariables.ColorPrimary,
            ForeColor = GlobalVariables.ColorDark
        };

        linkLabel.Links.Add(linkStart, "GitHub".Length, url);
        linkLabel.LinkClicked += (s, e) =>
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Link.LinkData.ToString(),
                UseShellExecute = true
            });
        };

        return linkLabel;
    }

    private Label CreateFooterLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(GlobalVariables.AppFontName, 10, FontStyle.Italic),
            ForeColor = Color.Gray,
            AutoSize = true
        };
    }
}