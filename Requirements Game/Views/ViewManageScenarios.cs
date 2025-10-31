using System.Windows.Forms;
using System.Drawing;
using System;
using Requirements_Game;
using System.Diagnostics;
using System.Collections.Generic;

public class ViewManageScenarios : View {

    public ViewManageScenarios() {

        ViewTableLayoutPanel.Dock = DockStyle.Top;
        ViewTableLayoutPanel.AutoSize = true;

        ViewTableLayoutPanel.ColumnCount = 3;
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 800f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        RebuildView();

        Scenarios.ScenariosChanged += Scenarios_ScenariosChanged;

    }

    public void Scenarios_ScenariosChanged(object sender, EventArgs e) {

        RebuildView();

    }

    public void RebuildView() {

        ViewTableLayoutPanel.Controls.Clear();
        ViewTableLayoutPanel.RowStyles.Clear();
        ViewTableLayoutPanel.RowCount = 0;

        foreach (Scenario scenario in Scenarios.GetScenarios()) {

            ViewTableLayoutPanel.RowCount += 2;
            ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
            ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 104f));

            ScenarioCard scenarioCard = new ScenarioCard(scenario);
            scenarioCard.Dock = DockStyle.Fill;

            ViewTableLayoutPanel.Controls.Add(scenarioCard, 1, ViewTableLayoutPanel.RowCount - 1);

        }

        ViewTableLayoutPanel.RowCount += 1;
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));

    }

    private class ScenarioCard : CustomTableLayoutPanel {

        private Scenario Scenario;

        public ScenarioCard(Scenario scenario) {

            this.Scenario = scenario;

            this.BackColor = GlobalVariables.ColorLight;
            this.CornerRadius = 15;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0);
            this.Margin = new Padding(0);

            this.ColumnCount = 7;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10f));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60f));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10f));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60f));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60f));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10f));
    
            this.RowCount = 5;
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 2f));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 10f));

            Label scenarioNameLabel = new Label();
            scenarioNameLabel.Dock = DockStyle.Left;
            scenarioNameLabel.Text = scenario.Name;
            scenarioNameLabel.Font = new Font(GlobalVariables.AppFontName, 16, FontStyle.Bold);
            scenarioNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            scenarioNameLabel.AutoSize = true;
            scenarioNameLabel.Padding = new Padding(0);
            scenarioNameLabel.Margin = new Padding(0);
            this.Controls.Add(scenarioNameLabel, 1,0);
            this.SetColumnSpan(scenarioNameLabel, 5);

            Panel nameUnderscore = new Panel();
            nameUnderscore.BackColor = GlobalVariables.ColorMedium;
            nameUnderscore.Size = new Size(300, 2);
            nameUnderscore.Dock = DockStyle.Left;
            nameUnderscore.Padding = new Padding(0);
            nameUnderscore.Margin = new Padding(5,0,0,0);
            this.Controls.Add(nameUnderscore, 1, 1);
            this.SetColumnSpan(nameUnderscore, 5);

            Label scenarioDescriptionLabel = new Label();
            scenarioDescriptionLabel.Dock = DockStyle.Fill;
            scenarioDescriptionLabel.Text = scenario.Description;
            scenarioDescriptionLabel.Font = new Font(GlobalVariables.AppFontName, 10, FontStyle.Italic);
            scenarioDescriptionLabel.TextAlign = ContentAlignment.MiddleLeft;
            scenarioDescriptionLabel.AutoSize = true;
            scenarioDescriptionLabel.Padding = new Padding(0);
            scenarioDescriptionLabel.Margin = new Padding(0);
            this.Controls.Add(scenarioDescriptionLabel, 1, 2);
            this.SetColumnSpan(scenarioDescriptionLabel, 5);

            CustomTextButton editButton = new CustomTextButton();
            editButton.CornerRadius = 8;
            editButton.Dock = DockStyle.Fill;
            editButton.Text = "Edit";
            editButton.Font = new Font(GlobalVariables.AppFontName, 10, FontStyle.Bold);
            editButton.ForeColor = Color.White;
            editButton.BackColor = GlobalVariables.ColorButtonBlack;
            editButton.InteractionEffect = ButtonInteractionEffect.Lighten;
            editButton.AutoSize = true;
            editButton.Padding = new Padding(0);
            editButton.Margin = new Padding(0,3,0,0);
            this.Controls.Add(editButton, 1, 3);

            editButton.MouseClick += Button_MouseClick;

            CustomTextButton exportButton = new CustomTextButton();
            exportButton.CornerRadius = 8;
            exportButton.Dock = DockStyle.Fill;
            exportButton.Text = "Export";
            exportButton.Font = new Font(GlobalVariables.AppFontName, 10, FontStyle.Bold);
            exportButton.ForeColor = Color.White;
            exportButton.BackColor = GlobalVariables.ColorButtonBlack;
            exportButton.InteractionEffect = ButtonInteractionEffect.Lighten;
            exportButton.AutoSize = true;
            exportButton.Padding = new Padding(0);
            exportButton.Margin = new Padding(0,3,0,0);
            this.Controls.Add(exportButton, 3, 3);

            exportButton.MouseClick += Button_MouseClick;

            CustomTextButton deleteButton = new CustomTextButton();
            deleteButton.CornerRadius = 8;
            deleteButton.Dock = DockStyle.Fill;
            deleteButton.Text = "Delete";
            deleteButton.Font = new Font(GlobalVariables.AppFontName, 10, FontStyle.Bold);
            deleteButton.ForeColor = Color.White;
            deleteButton.BackColor = Color.FromArgb(136, 0, 0);
            deleteButton.InteractionEffect = ButtonInteractionEffect.Lighten;
            deleteButton.AutoSize = true;
            deleteButton.Padding = new Padding(0);
            deleteButton.Margin = new Padding(0, 3, 0, 0);
            this.Controls.Add(deleteButton, 5, 3);

            deleteButton.MouseClick += Button_MouseClick;
        }

        /// <summary>
        /// Handles mouse click events for scenario-specific buttons: Edit, Export, and Delete.
        /// </summary>
        private void Button_MouseClick(object sender, MouseEventArgs e)
        {
            // Cast sender to CustomTextButton to access its label and context
            CustomTextButton textButton = (CustomTextButton)sender;
            Debug.WriteLine($"Clicked button: {textButton.Text}");

            // Navigate to the Edit Scenario view
            if (textButton.Text == "Edit")
            {
                Form1 form1 = (Form1)this.FindForm();
                form1.ChangeView("Edit Scenario", Scenario);
            }

            // Export the current scenario to a user-selected JSON file
            if (textButton.Text == "Export")
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Export Scenarios";
                    saveFileDialog.Filter = "JSON Files (*.json)|*.json";
                    saveFileDialog.FileName = $"{Scenario.Name}_Requirements.json";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedPath = saveFileDialog.FileName;
                        Scenarios.SaveToFile(selectedPath, new List<Scenario> { Scenario });
                    }
                }
            }

            // Confirm and delete the current scenario from the list
            if (textButton.Text == "Delete")
            {
                var confirm = MessageBox.Show(
                    $"Are you sure you want to delete '{Scenario.Name}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    Scenarios.Remove(Scenario);
                }
            }
        }
    }
}