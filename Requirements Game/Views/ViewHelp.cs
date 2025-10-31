using Requirements_Game;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class ViewHelp : View
{
    private List<Image> images;
    private int currentIndex;
    private CustomPictureBox mainPicture;
    private FlowLayoutPanel thumbnailPanel;
    private Label pageLabel;

    public ViewHelp()
    {
        // View layout consistent with other views
        ViewTableLayoutPanel.Dock = DockStyle.Top;
        ViewTableLayoutPanel.AutoSize = true;
        ViewTableLayoutPanel.Padding = new Padding(10);

        ViewTableLayoutPanel.ColumnCount = 3;
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 900f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        ViewTableLayoutPanel.RowCount = 3;
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 520f)); // larger to give image focus
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));  // page label row

        images = new List<Image>();

        // Hardcoded ordered filenames, include file extension
        string[] orderedFileNames = new[] {
            "Help_TitlePage.png", // 1
            "Help_ScenarioSelectionPage.png",
            "Help_ScenarioDetailsPage.png",
            "Help_manageScenarioPage.png",
            "Help_CreateScenarioPage.png",
            "Help_EditScenarioPage.png",
            "Help_ChatPage_Left.png",
            "Help_ChatPage_Top.png",
            "Help_ChatPage_bottom.png"

        };

        string[] candidateDirs = new[]
        {
            Path.Combine(FileSystem.InstallDirectory, "resources"),
            Path.Combine(FileSystem.InstallDirectory, "Resources")
        };

        foreach (var dir in candidateDirs)
        {
            if (!Directory.Exists(dir)) continue;

            foreach (var fileName in orderedFileNames)
            {
                string filePath = Path.Combine(dir, fileName);
                if (!File.Exists(filePath)) continue;

                try
                {
                    // Load into memory copy so file handle can be released
                    using (var fs = File.OpenRead(filePath))
                    {
                        var img = Image.FromStream(fs);
                        images.Add(new Bitmap(img));
                    }
                }
                catch
                {
                    // skip invalid/unreadable files silently
                }
            }

            if (images.Count > 0) break;
        }

        // If no PNG images found, show instruction label
        if (images.Count == 0)
        {
            Label noImages = new Label
            {
                Text = "No help PNG images found in the 'resources' folder.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(GlobalVariables.AppFontName, 12),
                ForeColor = Color.Black
            };
            ViewTableLayoutPanel.Controls.Add(noImages, 1, 1);
            return;
        }

        // Build the central area: left arrow | image border panel | right arrow
        var centerPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Padding = new Padding(0),
        };
        centerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f)); // left arrow
        centerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // image area (with border)
        centerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f)); // right arrow

        // Left arrow button
        CustomTextButton leftButton = new CustomTextButton
        {
            Text = "<",
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            ForeColor = Color.Black,
            InteractionEffect = ButtonInteractionEffect.None,
            Font = new Font(GlobalVariables.AppFontName, 18, FontStyle.Bold),
            CornerRadius = 6
        };
        leftButton.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) { int prev = (currentIndex - 1 + images.Count) % images.Count; ShowImage(prev); } };
        centerPanel.Controls.Add(leftButton, 0, 0);

        // Image border panel (provides visible border around the image)
        var imageBorderPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(6), // border thickness
            BackColor = Color.Black // border color
        };

        // Inner panel to host the picture and give a background
        var innerImagePanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White // image background inside border
        };

        // Main picture area
        mainPicture = new CustomPictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = images[0],
            Margin = new Padding(0),
            BackColor = Color.White
        };
        mainPicture.MouseClick += MainPicture_MouseClick;

        // Compose border -> inner -> picture
        innerImagePanel.Controls.Add(mainPicture);
        imageBorderPanel.Controls.Add(innerImagePanel);
        centerPanel.Controls.Add(imageBorderPanel, 1, 0);

        // Right arrow button
        CustomTextButton rightButton = new CustomTextButton
        {
            Text = ">",
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            ForeColor = Color.Black,
            InteractionEffect = ButtonInteractionEffect.None,
            Font = new Font(GlobalVariables.AppFontName, 18, FontStyle.Bold),
            CornerRadius = 6
        };
        rightButton.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) { int next = (currentIndex + 1) % images.Count; ShowImage(next); } };
        centerPanel.Controls.Add(rightButton, 2, 0);

        ViewTableLayoutPanel.Controls.Add(centerPanel, 1, 1);

        // Page label at bottom (centered)
        pageLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(GlobalVariables.AppFontName, 10, FontStyle.Bold),
            ForeColor = Color.Black
        };
        ViewTableLayoutPanel.Controls.Add(pageLabel, 1, 2);

        // Start at first image
        currentIndex = 0;
        ShowImage(0);
    }

    private void MainPicture_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        // advance to next image
        int next = (currentIndex + 1) % images.Count;
        ShowImage(next);
    }

    private void ShowImage(int index)
    {
        if (index < 0 || index >= images.Count) return;
        currentIndex = index;
        mainPicture.Image = images[index];

        // update page label
        pageLabel.Text = string.Format("{0} / {1}", currentIndex + 1, images.Count);
    }
}