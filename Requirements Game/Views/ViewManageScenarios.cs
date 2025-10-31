using System.Windows.Forms;
using System.Drawing;
using System;
using Requirements_Game;
using System.Diagnostics;
using System.Collections.Generic;

/// <summary>
/// View for displaying and managing all available scenarios
/// Dynamically rebuilds whenever the scenario list changes
/// </summary>
public class ViewManageScenarios : View
{

    public ViewManageScenarios()
    {

        // -- Base Layout Setup --

        ViewTableLayoutPanel.Dock = DockStyle.Top;
        ViewTableLayoutPanel.AutoSize = true;

        ViewTableLayoutPanel.ColumnCount = 3;
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // Left padding
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 800f)); // Scenario cards column
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // Right padding

        // -- Build Initial Scenario List --

        RebuildView();

        // -- Event Subscription --
        // Automatically rebuild when the global scenario list changes

        Scenarios.ScenariosChanged += Scenarios_ScenariosChanged;

    }

    /// <summary>
    /// Event that is triggered when the global scenario list changes
    /// </summary>
    public void Scenarios_ScenariosChanged(object sender, EventArgs e)
    {

        RebuildView();

    }

    // -- Build Scenario Cards Stack --
    /// <summary>
    /// Method that clears and rebuilds all scenario cards within the layout panel
    /// </summary>
    public void RebuildView()
    {

        // Clear previous content and reset row configuration

        ViewTableLayoutPanel.Controls.Clear();
        ViewTableLayoutPanel.RowStyles.Clear();
        ViewTableLayoutPanel.RowCount = 0;

        // Add a ScenarioCard for each scenario in the list

        foreach (Scenario scenario in Scenarios.GetScenarios())
        {

            ViewTableLayoutPanel.RowCount += 2;
            ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
            ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 104f));

            ScenarioCard scenarioCard = new ScenarioCard(scenario);
            scenarioCard.Dock = DockStyle.Fill;

            ViewTableLayoutPanel.Controls.Add(scenarioCard, 1, ViewTableLayoutPanel.RowCount - 1);

        }

        // Add final bottom spacing row

        ViewTableLayoutPanel.RowCount += 1;
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));

    }

    /// <summary>
    /// Represents a visual card component used to display a single scenario entry 
    /// within the Manage Scenarios view. 
    /// Each card shows the scenario’s name, description, and action buttons 
    /// for editing, exporting, and deleting the scenario
    /// </summary>
    private class ScenarioCard : CustomTableLayoutPanel
    {

        private Scenario Scenario;

        public ScenarioCard(Scenario scenario)
        {

            this.Scenario = scenario;

            // -- Base Panel Setup --
            this.BackColor = GlobalVariables.ColorLight;
            this.CornerRadius = 15;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0);
            this.Margin = new Padding(0);

            // -- Layout Configuration --
            this.ColumnCount = 7;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10f)); // Left padding
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60f)); // Button column
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10f)); // Spacing
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60f)); // Button column
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // Description area
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60f)); // Button column
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10f)); // Right padding

            this.RowCount = 5;
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f)); // Title row
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 2f)); // Divider line
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // Description row
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f)); // Button row
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 10f)); // Bottom padding

            // -- Scenario Title --

            Label scenarioNameLabel = new Label();
            scenarioNameLabel.Dock = DockStyle.Left;
            scenarioNameLabel.Text = scenario.Name;
            scenarioNameLabel.Font = new Font(GlobalVariables.AppFontName, 16, FontStyle.Bold);
            scenarioNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            scenarioNameLabel.AutoSize = true;
            scenarioNameLabel.Padding = new Padding(0);
            scenarioNameLabel.Margin = new Padding(0);
            this.Controls.Add(scenarioNameLabel, 1, 0);
            this.SetColumnSpan(scenarioNameLabel, 5);

            // -- Title Divider Line --

            Panel nameUnderscore = new Panel();
            nameUnderscore.BackColor = GlobalVariables.ColorMedium;
            nameUnderscore.Size = new Size(300, 2);
            nameUnderscore.Dock = DockStyle.Left;
            nameUnderscore.Padding = new Padding(0);
            nameUnderscore.Margin = new Padding(5, 0, 0, 0);
            this.Controls.Add(nameUnderscore, 1, 1);
            this.SetColumnSpan(nameUnderscore, 5);

            // -- Scenario Description --

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

            // -- Edit Button --

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
            editButton.Margin = new Padding(0, 3, 0, 0);
            this.Controls.Add(editButton, 1, 3);

            editButton.MouseClick += Button_MouseClick;

            // -- Export Button --

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
            exportButton.Margin = new Padding(0, 3, 0, 0);
            this.Controls.Add(exportButton, 3, 3);

            exportButton.MouseClick += Button_MouseClick;

            // -- Delete Button --

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
        /// Handles mouse click events for scenario-specific buttons: Edit, Export, and Delete
        /// </summary>
        private void Button_MouseClick(object sender, MouseEventArgs e)
        {

            // -- Get Clicked Button --

            CustomTextButton textButton = (CustomTextButton)sender; // Cast sender to CustomTextButton to access its label and context

            // If edit button - Navigate to the Edit Scenario view

            if (textButton.Text == "Edit")
            {

                Form1 form1 = (Form1)this.FindForm();
                form1.ChangeView("Edit Scenario", Scenario);

            }

            // If export button - Export the current scenario to a user-selected JSON file

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

            // If delete button - Confirm and delete the current scenario from the list

            if (textButton.Text == "Delete")
            {

                var confirm = MessageBox.Show(
                    $"Are you sure you want to delete '{Scenario.Name}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes) Scenarios.Remove(Scenario);

            }
        }
    }
}