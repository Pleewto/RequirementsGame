using Requirements_Game;
using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Displays available scenarios in a responsive, multi-column grid and
/// rebuilds the layout when the scenario list changes
/// </summary>
public class ViewScenarios : View
{

    /// <summary>
    /// Initializes the scenarios view layout and subscribes to scenario change events
    /// </summary>
    public ViewScenarios()
    {

        ViewTableLayoutPanel.Dock = DockStyle.Top;
        ViewTableLayoutPanel.AutoSize = true;

        ViewTableLayoutPanel.ColumnCount = 7;
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300f));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        RebuildView();

        Scenarios.ScenariosChanged += Scenarios_ScenariosChanged;

    }

    /// <summary>
    /// Handles global scenario list updates by rebuilding the view
    /// </summary>
    public void Scenarios_ScenariosChanged(object sender, EventArgs e)
    {

        RebuildView();

    }

    /// <summary>
    /// Rebuilds the scenarios grid: clears existing rows/controls and lays out
    /// ScenarioCards across three fixed-width columns with spacing rows
    /// </summary>
    private void RebuildView()
    {

        // Clear all controls and TableLayoutPanel rows to be rebuilt

        ViewTableLayoutPanel.Controls.Clear();
        ViewTableLayoutPanel.RowStyles.Clear();
        ViewTableLayoutPanel.RowCount = 0;

        int columnIndex = 1;

        foreach (Scenario scenario in Scenarios.GetScenarios())
        {

            if (columnIndex == 1)
            {

                ViewTableLayoutPanel.RowCount += 2;
                ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
                ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 115f));

            }

            ScenarioCard scenarioCard = new ScenarioCard(scenario);

            ViewTableLayoutPanel.Controls.Add(scenarioCard, columnIndex, ViewTableLayoutPanel.RowCount - 1);

            columnIndex = (columnIndex == 5) ? 1 : columnIndex + 2;

        }

    }

    /// <summary>
    /// Visual card for a single scenario, showing its name and description with
    /// hover/press feedback and click-to-open details behavior.
    /// </summary>
    private class ScenarioCard : CustomTableLayoutPanel
    {

        private Scenario scenario;
        private bool IsMouseEnter;

        private CustomLabel ScenarioNameLabel;
        private CustomPanel NameUnderscore;
        private CustomLabel ScenarioDescriptionLabel;

        /// <summary>
        /// Configures the scenario card’s layout, binds scenario data, and wires up
        /// mouse interaction events
        /// </summary>
        public ScenarioCard(Scenario scenario)
        {

            // -- Card Layout

            this.scenario = scenario;
            this.BackColor = GlobalVariables.ColorLight;
            this.CornerRadius = 15;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0, 5, 0, 5);
            this.Margin = new Padding(0);
            this.IsMouseEnter = false;

            this.ColumnCount = 3;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10f));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10f));

            this.RowCount = 3;
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));
            this.RowStyles.Add(new RowStyle(SizeType.Absolute, 2f));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // -- ScenarioNameLabel

            ScenarioNameLabel = new CustomLabel();
            ScenarioNameLabel.Dock = DockStyle.Fill;
            ScenarioNameLabel.Text = scenario.Name;
            ScenarioNameLabel.BackColor = GlobalVariables.ColorLight;
            ScenarioNameLabel.Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Bold);
            ScenarioNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            ScenarioNameLabel.AutoSize = true;
            ScenarioNameLabel.Padding = new Padding(0);
            ScenarioNameLabel.Margin = new Padding(0);
            this.Controls.Add(ScenarioNameLabel, 1, 0);

            // -- NameUnderscore line

            NameUnderscore = new CustomPanel();
            NameUnderscore.BackColor = GlobalVariables.ColorMedium;
            NameUnderscore.Size = new Size(300, 2);
            NameUnderscore.Dock = DockStyle.Fill;
            NameUnderscore.Padding = new Padding(0);
            NameUnderscore.Margin = new Padding(5, 0, 0, 0);
            this.Controls.Add(NameUnderscore, 1, 1);

            // -- ScenarioDescriptionLabel

            ScenarioDescriptionLabel = new CustomLabel();
            ScenarioDescriptionLabel.Dock = DockStyle.Fill;
            ScenarioDescriptionLabel.BackColor = GlobalVariables.ColorLight;
            ScenarioDescriptionLabel.Text = scenario.Description;
            ScenarioDescriptionLabel.Font = new Font(GlobalVariables.AppFontName, 10, FontStyle.Italic);
            ScenarioDescriptionLabel.TextAlign = ContentAlignment.TopLeft;
            ScenarioDescriptionLabel.AutoSize = true;
            ScenarioDescriptionLabel.Padding = new Padding(0);
            ScenarioDescriptionLabel.Margin = new Padding(0, 8, 0, 0);
            this.Controls.Add(ScenarioDescriptionLabel, 1, 2);

            // Events

            this.MouseClick += Me_MouseClick;
            this.MouseDown += Me_MouseDown;
            this.MouseEnter += Me_MouseEnter;
            this.MouseLeave += Me_MouseLeave;
            this.MouseUp += Me_MouseUp;

            ScenarioNameLabel.MouseClick += Me_MouseClick;
            ScenarioNameLabel.MouseDown += Me_MouseDown;
            ScenarioNameLabel.MouseEnter += Me_MouseEnter;
            ScenarioNameLabel.MouseLeave += Me_MouseLeave;
            ScenarioNameLabel.MouseUp += Me_MouseUp;

            NameUnderscore.MouseClick += Me_MouseClick;
            NameUnderscore.MouseDown += Me_MouseDown;
            NameUnderscore.MouseEnter += Me_MouseEnter;
            NameUnderscore.MouseLeave += Me_MouseLeave;
            NameUnderscore.MouseUp += Me_MouseUp;

            ScenarioDescriptionLabel.MouseClick += Me_MouseClick;
            ScenarioDescriptionLabel.MouseDown += Me_MouseDown;
            ScenarioDescriptionLabel.MouseEnter += Me_MouseEnter;
            ScenarioDescriptionLabel.MouseLeave += Me_MouseLeave;
            ScenarioDescriptionLabel.MouseUp += Me_MouseUp;

        }

        /// <summary>
        /// On left mouse down, darkens the card and child regions to indicate press state
        /// </summary>
        private void Me_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {

                ControlFreezer.Freeze(this);

                ScenarioNameLabel.BackColor = ColorManager.DarkenColor(ScenarioNameLabel.BackColor, 0.1);
                NameUnderscore.BackColor = ColorManager.DarkenColor(NameUnderscore.BackColor, 0.1);
                ScenarioDescriptionLabel.BackColor = ColorManager.DarkenColor(ScenarioDescriptionLabel.BackColor, 0.1);
                this.BackColor = ColorManager.DarkenColor(this.BackColor, 0.1);

                ControlFreezer.Unfreeze(this);

            }

        }

        /// <summary>
        /// On first mouse enter, applies a subtle darken effect to highlight hover state
        /// </summary>
        private void Me_MouseEnter(object sender, EventArgs e)
        {

            if (IsMouseEnter == false)
            {

                IsMouseEnter = true;

                ControlFreezer.Freeze(this);

                ScenarioNameLabel.BackColor = ColorManager.DarkenColor(ScenarioNameLabel.BackColor, 0.1);
                NameUnderscore.BackColor = ColorManager.DarkenColor(NameUnderscore.BackColor, 0.1);
                ScenarioDescriptionLabel.BackColor = ColorManager.DarkenColor(ScenarioDescriptionLabel.BackColor, 0.1);
                this.BackColor = ColorManager.DarkenColor(this.BackColor, 0.1);

                ControlFreezer.Unfreeze(this);

            }

        }

        /// <summary>
        /// When the mouse leaves the card (and is no longer within bounds),
        /// restores original colours and clears hover state
        /// </summary>
        private void Me_MouseLeave(object sender, EventArgs e)
        {

            if (IsMouseEnter && this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition)) == false)
            {

                IsMouseEnter = false;

                ControlFreezer.Freeze(this);

                ScenarioNameLabel.BackColor = GlobalVariables.ColorLight;
                NameUnderscore.BackColor = GlobalVariables.ColorMedium;
                ScenarioDescriptionLabel.BackColor = GlobalVariables.ColorLight;
                this.BackColor = GlobalVariables.ColorLight;

                ControlFreezer.Unfreeze(this);

            }

        }

        /// <summary>
        /// On left mouse up, resets the card visuals to their default (non-pressed) colours
        /// </summary>
        private void Me_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {

                ScenarioNameLabel.BackColor = GlobalVariables.ColorLight;
                NameUnderscore.BackColor = GlobalVariables.ColorMedium;
                ScenarioDescriptionLabel.BackColor = GlobalVariables.ColorLight;
                this.BackColor = GlobalVariables.ColorLight;

            }

        }

        /// <summary>
        /// Selects this scenario, stores it as current, and opens the scenario details dialog
        /// </summary>
        private void Me_MouseClick(object sender, MouseEventArgs e)
        {

            Form1 form1 = (Form1)this.FindForm();

            GlobalVariables.CurrentScenario = scenario; // Remember which scenario was selected

            ScenarioDetailsForm.Show(scenario, form1);

        }

    }

}