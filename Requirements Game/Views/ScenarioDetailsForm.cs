using Requirements_Game;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

class ScenarioDetailsForm : Form {

    Scenario scenario;

    public static void Show(Scenario scenario, Form mainForm) {

        using (var form = new ScenarioDetailsForm(scenario)) {

            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(mainForm);

        }

    }

    public ScenarioDetailsForm(Scenario scenario)
    {
        this.scenario = scenario;
        this.FormBorderStyle = FormBorderStyle.None;
        this.Size = new Size(600, 600);
        this.BackColor = Color.AliceBlue;
        this.TransparencyKey = Color.AliceBlue;

        // Main layout
        CustomTableLayoutPanel tableLayoutPanel = new CustomTableLayoutPanel();
        tableLayoutPanel.CornerRadius = 10;
        tableLayoutPanel.Dock = DockStyle.Fill;
        tableLayoutPanel.BackColor = GlobalVariables.ColorMedium;
        tableLayoutPanel.Padding = new Padding(10);

        tableLayoutPanel.RowCount = 3;
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80f)); // Header
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // Scrollable content
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f)); // Footer

        tableLayoutPanel.ColumnCount = 3;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f)); // Left padding
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // Main content
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f)); // Right padding

        this.Controls.Add(tableLayoutPanel);

        
        // title
        TableLayoutPanel headerPanel = new TableLayoutPanel();
        headerPanel.Dock = DockStyle.Fill;
        headerPanel.ColumnCount = 2;
        headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30f));

        Label nameLabel = new Label();
        nameLabel.Text = scenario.Name;
        nameLabel.Font = new Font(GlobalVariables.AppFontName, 20, FontStyle.Bold);
        nameLabel.ForeColor = Color.Black;
        nameLabel.Dock = DockStyle.Fill;
        nameLabel.TextAlign = ContentAlignment.MiddleLeft;
        headerPanel.Controls.Add(nameLabel, 0, 0);

        CustomPictureBox closeButton = new CustomPictureBox();
        closeButton.Image = (Image)Requirements_Game.Properties.Resources.ResourceManager.GetObject("close");
        closeButton.InteractionEffect = ButtonInteractionEffect.Lighten;
        closeButton.SizeMode = PictureBoxSizeMode.CenterImage;
        closeButton.Dock = DockStyle.Fill;
        closeButton.MouseClick += CloseButton_MouseClick;
        headerPanel.Controls.Add(closeButton, 1, 0);

        tableLayoutPanel.Controls.Add(headerPanel, 1, 0);

        // main description content
        Panel scrollPanel = new Panel();
        scrollPanel.Dock = DockStyle.Fill;
        scrollPanel.AutoScroll = true;
        scrollPanel.BackColor = GlobalVariables.ColorMedium;
        scrollPanel.Padding = new Padding(5);

        RichTextBox contentRichTextBox = new RichTextBox();
        contentRichTextBox.ForeColor = Color.Black;
        contentRichTextBox.Font = new Font(GlobalVariables.AppFontName, 11);
        contentRichTextBox.Dock = DockStyle.Fill;
        contentRichTextBox.BorderStyle = BorderStyle.None;
        contentRichTextBox.BackColor = GlobalVariables.ColorMedium;
        contentRichTextBox.ReadOnly = true;
        contentRichTextBox.TabStop = false;

        string content = $"{scenario.Description}\n\n" +
                         $"Senior Engineer:\n" +
                         $"- {Scenario.SeniorSoftwareEngineer.Name}\n" +
                         $"  Role: {Scenario.SeniorSoftwareEngineer.Role}\n" +
                         $"  Personality: {Scenario.SeniorSoftwareEngineer.Personality}\n\n" +
                         $"Stakeholders:\n" +
                         string.Join("\n", scenario.GetStakeholders().Select(s =>
                             $"- {s.Name} ({s.Role}) — Personality: { s.Personality} "));

        contentRichTextBox.AppendText(content);
        scrollPanel.Controls.Add(contentRichTextBox);

        tableLayoutPanel.Controls.Add(scrollPanel, 1, 1);

        // begin interviewing button
        TableLayoutPanel footerPanel = new TableLayoutPanel();
        footerPanel.Dock = DockStyle.Fill;
        footerPanel.ColumnCount = 2;
        footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // filler
        footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160f)); // button width

        CustomTextButton beginButton = new CustomTextButton();
        beginButton.Text = "Begin Interviewing";
        beginButton.CornerRadius = 5;
        beginButton.TextAlign = ContentAlignment.MiddleCenter;
        beginButton.Padding = new Padding(0, 0, 0, 3);
        beginButton.Font = new Font(GlobalVariables.AppFontName, 12, FontStyle.Bold);
        beginButton.BackColor = Color.Black;
        beginButton.InteractionEffect = ButtonInteractionEffect.None;
        beginButton.ForeColor = Color.White;
        beginButton.Dock = DockStyle.Fill;
        beginButton.MouseClick += TestButton_MouseClick;

        footerPanel.Controls.Add(beginButton, 1, 0);
        tableLayoutPanel.Controls.Add(footerPanel, 1, 2);
    }

    private void CloseButton_MouseClick(object sender, MouseEventArgs e) {

        this.Close();

    }

    private void TestButton_MouseClick(object sender, MouseEventArgs e) {

        Form1 form1 = (Form1)this.Owner;
        form1.ChangeView("Chat");

        this.Close();

    }

}