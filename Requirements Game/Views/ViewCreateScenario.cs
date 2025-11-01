using Requirements_Game;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Requirements_Game.Properties;

/// <summary>
/// View for creating (or editing) a Scenario.  
/// Dynamically builds input sections (scenario info, stakeholders, requirements),
/// and commits changes via Create/Save.
/// </summary>
public class ViewCreateScenario : View
{
    protected Scenario ReferenceScenario = null;
    protected Scenario EditingScenario = null; // Holds a copy of ReferenceScenario. Allows editing without modifying the original until the changes are explicitly saved or committed

    private Dictionary<string, CustomLabelledRichTextBox> InputFields = new Dictionary<string, CustomLabelledRichTextBox>();

    /// <summary>
    /// Initializes the view with an empty editable Scenario and constructs
    /// the layout and input sections.
    /// </summary>
    public ViewCreateScenario()
    {

        EditingScenario = new Scenario(); // Assign EditingScenario with an empty instance
        ReferenceScenario = null;

        ViewTableLayoutPanel.Dock = DockStyle.Top;
        ViewTableLayoutPanel.AutoSize = true;

        ViewTableLayoutPanel.ColumnCount = 3;
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 800));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        ViewTableLayoutPanel.RowCount = 0;
        RebuildView();

    }

    /// <summary>
    /// Resets the form to a fresh, empty Scenario and rebuilds the UI.
    /// </summary>
    public void Clear()
    {

        EditingScenario = new Scenario();

        RebuildView();

    }

    /// <summary>
    /// Rebuilds the entire form from the current EditingScenario:
    /// scenario info, stakeholders (with add/remove), requirements, and the Create/Save button.
    /// Preserves state by reading from the working copy.
    /// </summary>
    public void RebuildView()
    {

        // -- Freeze UI to prevent flicker or unnecessary redraws during rebuild

        ControlFreezer.Freeze(this);

        // -- Clear existing input fields and layout before reconstruction

        InputFields.Clear();
        ViewTableLayoutPanel.Controls.Clear();
        ViewTableLayoutPanel.RowStyles.Clear();
        ViewTableLayoutPanel.RowCount = 0;

        // -- Retrieve current scenario data to repopulate fields

        var name = EditingScenario.Name;
        var description = EditingScenario.Description;
        var stakeholders = EditingScenario.GetStakeholders().ToList();
        var stakeholderCount = stakeholders.Count;
        var frText = string.Join("\n", EditingScenario.FunctionalRequirements);
        var nfrText = string.Join("\n", EditingScenario.NonFunctionalRequirements);

        // -- Scenario Info Block --
        // Create and populate the section containing the scenario name and description

        var scenarioBlock = CreateSectionBlock();
        RebuildView_LabelledRichTextBox(ref scenarioBlock, "Scenario Name", name);
        RebuildView_LabelledRichTextBox(ref scenarioBlock, "Description", description, 6);
        ViewTableLayoutPanel.Controls.Add(scenarioBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // -- Stakeholders Block --
        // Create section to display and manage all stakeholder entries

        var stakeholderBlock = CreateSectionBlock();

        // Define available personality options for stakeholder selection

        string[] personalityOptions = new string[] {
            "Neutral",
            "Friendly",
            "Formal",
            "Challenging",
            "Skeptical",
            "Enthusiastic",
            "Detail-Oriented"
        };

        // Add header label for the stakeholder section

        RebuildView_Label(ref stakeholderBlock, "Stakeholders");

        // -- Loop through each stakeholder and create corresponding input fields

        for (int i = 0; i < stakeholders.Count; i++)
        {
            int index = i + 1;

            // Add label and close icon for the stakeholder

            RebuildView_StackholderLabelWithClose(ref stakeholderBlock, index);

            // Add name, role, and personality selection fields

            RebuildView_LabelledRichTextBox(ref stakeholderBlock, $"Name_{index}", stakeholders[i].Name);
            RebuildView_LabelledRichTextBox(ref stakeholderBlock, $"Role_{index}", stakeholders[i].Role);
            RebuildView_LabelledComboBox(ref stakeholderBlock, $"Personality_{index}", stakeholders[i].Personality, personalityOptions);

        }

        // -- Add Stakeholder Button --
        // Creates a button to allow users to dynamically add new stakeholders

        CustomTextButton addStakeholderButton = new CustomTextButton();
        addStakeholderButton.Text = "+ Add Stakeholder";
        addStakeholderButton.Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Bold);
        addStakeholderButton.ForeColor = Color.White;
        addStakeholderButton.BackColor = GlobalVariables.ColorButtonBlack;
        addStakeholderButton.InteractionEffect = ButtonInteractionEffect.Lighten;
        addStakeholderButton.TextAlign = ContentAlignment.MiddleCenter;
        addStakeholderButton.CornerRadius = 5;
        addStakeholderButton.Size = new Size(170, 30);
        addStakeholderButton.Anchor = AnchorStyles.None;

        // -- Event: Add new stakeholder when clicked

        addStakeholderButton.MouseClick += (s, e) => {

            // Save current state before rebuilding

            EditingScenario = this.GetScenario();

            // Append a new empty stakeholder

            EditingScenario.AddStakeholder(new Stakeholder());

            // Preserve scroll position after rebuild

            int scrollPosition = this.VerticalScroll.Value;

            RebuildView();

            this.VerticalScroll.Value = scrollPosition;

        };

        // -- Add button to stakeholder section

        stakeholderBlock.RowCount += 1;
        stakeholderBlock.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        stakeholderBlock.Controls.Add(addStakeholderButton, 1, stakeholderBlock.RowCount - 1);

        // Add stakeholder section to the main view

        ViewTableLayoutPanel.Controls.Add(stakeholderBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // -- Requirements Block --
        // Adds a section for optional functional and non-functional requirements

        var requirementsBlock = CreateSectionBlock();
        RebuildView_Label(ref requirementsBlock, "Requirements (Optional)");
        RebuildView_LabelledRichTextBox(ref requirementsBlock, "Functional Requirements", frText, 6);
        RebuildView_LabelledRichTextBox(ref requirementsBlock, "Non-Functional Requirements", nfrText, 6);
        ViewTableLayoutPanel.Controls.Add(requirementsBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // -- Create/Save Button Block --
        // Add the main button that creates or saves the scenario

        var buttonBlock = CreateSectionBlock();
        CustomTextButton createButton = new CustomTextButton();
        createButton.Text = ReferenceScenario is null ? "Create" : "Save";
        createButton.Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Bold);
        createButton.ForeColor = Color.White;
        createButton.BackColor = Color.FromArgb(0, 136, 5);
        createButton.InteractionEffect = ButtonInteractionEffect.Darken;
        createButton.TextAlign = ContentAlignment.MiddleCenter;
        createButton.CornerRadius = 5;
        createButton.Size = new Size(100, 30);
        createButton.Anchor = AnchorStyles.Right;

        // Attach event to handle click (create/save logic)

        createButton.MouseClick += CreateButton_MouseClick;

        // Add Create button to layout

        buttonBlock.RowCount += 1;
        buttonBlock.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        buttonBlock.Controls.Add(createButton, 1, buttonBlock.RowCount - 1);

        ViewTableLayoutPanel.Controls.Add(buttonBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // -- Unfreeze UI after rebuild complete

        ControlFreezer.Unfreeze(this);

    }

    /// <summary>
    /// Adds a labelled, single- or multi-line text input to a section panel,
    /// registers it in InputFields, and optionally sets a row height for multi-line.
    /// </summary>
    public void RebuildView_LabelledRichTextBox(ref CustomTableLayoutPanel SubTableLayoutPanel, string LabelText, string TextboxText, int RowCount = 1)
    {

        SubTableLayoutPanel.RowCount += 1;
        SubTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        CustomLabelledRichTextBox richTextBox = new CustomLabelledRichTextBox();
        richTextBox.Name = LabelText;
        richTextBox.LabelText = LabelText.Split('_')[0];
        richTextBox.Dock = DockStyle.Top;

        if (RowCount > 1)
        {

            richTextBox.Multiline = true;
            richTextBox.TextBoxRowCount = RowCount;
            richTextBox.Height = RowCount * 20;

        }

        richTextBox.TextboxText = TextboxText;
        SubTableLayoutPanel.Controls.Add(richTextBox, 1, SubTableLayoutPanel.RowCount - 1);
        InputFields[LabelText] = richTextBox;

    }

    /// <summary>
    /// Adds a labelled ComboBox (with fixed options) to a section panel.  
    /// Binds the selected value into InputFields for consistent retrieval
    /// </summary>
    public void RebuildView_LabelledComboBox(ref CustomTableLayoutPanel SubTableLayoutPanel, string LabelText, string selectedValue, string[] options)
    {

        SubTableLayoutPanel.RowCount += 1;
        SubTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Label

        CustomLabel label = new CustomLabel();
        label.Name = LabelText;
        label.Text = LabelText.Split('_')[0];
        label.Dock = DockStyle.Top;
        label.Font = new Font(GlobalVariables.AppFontName, 12, FontStyle.Bold);
        label.AutoSize = true;

        // ComboBox

        ComboBox comboBox = new ComboBox();
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.Items.AddRange(options);
        comboBox.SelectedItem = selectedValue ?? options.FirstOrDefault();
        comboBox.Dock = DockStyle.Top;

        // Wrap into a panel for consistency

        Panel panel = new Panel { Dock = DockStyle.Top, AutoSize = true };
        panel.Controls.Add(comboBox);
        panel.Controls.Add(label);

        SubTableLayoutPanel.Controls.Add(panel, 1, SubTableLayoutPanel.RowCount - 1);

        // Store in InputFields for consistency

        InputFields[LabelText] = new CustomLabelledRichTextBox
        {

            LabelText = LabelText,
            TextboxText = comboBox.SelectedItem?.ToString() ?? ""

        };

        // Update binding on selection

        comboBox.SelectedIndexChanged += (s, e) => {

            InputFields[LabelText].TextboxText = comboBox.SelectedItem?.ToString() ?? "";

        };

    }

    /// <summary>
    /// Inserts a "Stakeholder N" header with an inline close button that
    /// prompts for deletion and rebuilds the view on confirm.
    /// </summary>
    public void RebuildView_StackholderLabelWithClose(ref CustomTableLayoutPanel SubTableLayoutPanel, int StakeholderNumber)
    {

        // -- Create Stakeholder Header Label --
        // Display a title label for each stakeholder (e.g., "Stakeholder 1")

        string stakeholderText = $"Stakeholder {StakeholderNumber}";

        SubTableLayoutPanel.RowCount += 1;
        SubTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        CustomLabel label = new CustomLabel();
        label.Text = stakeholderText;
        label.Dock = DockStyle.Top;
        label.Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Italic | FontStyle.Bold);
        label.Margin = new Padding(0, 20, 0, 0);
        label.AutoSize = true;

        // -- Load and Prepare Close Icon --
        // Retrieve and resize the close icon from resources

        var closeIcon = (Bitmap)Resources.ResourceManager.GetObject("close_thick");
        closeIcon = new Bitmap(closeIcon, 18, 18);

        // -- Create Close Button --
        // A small clickable button to delete this stakeholder entry

        var closeButton = new CustomPictureBox();
        closeButton.Dock = DockStyle.Right;
        closeButton.Size = new Size(20, 20);
        closeButton.Padding = new Padding(0, 5, 0, 0);
        closeButton.SizeMode = PictureBoxSizeMode.CenterImage;
        closeButton.Image = closeIcon;
        closeButton.InteractionEffect = ButtonInteractionEffect.Lighten;
        label.Controls.Add(closeButton);

        // -- Close Button Click Event --
        // When clicked, confirm deletion and rebuild the view if confirmed

        closeButton.Click += (sender, e) => {

            // Ask user to confirm deletion

            DialogResult result = MessageBox.Show($"Are you sure you want to delete '{stakeholderText}'?", "Delete Stakeholder", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            // -- Delete Stakeholder --
            // If confirmed, rebuild the view with stakeholder removed

            if (result == DialogResult.Yes)
            {

                // Capture current state and remove the selected stakeholder

                EditingScenario = this.GetScenario();
                EditingScenario.DeleteStakeHolderByIndex(StakeholderNumber - 1);

                // Preserve current scroll position after rebuild

                int scrollPosition = this.VerticalScroll.Value;

                RebuildView();

                // Restore previous scroll position

                this.VerticalScroll.Value = scrollPosition;

            }

        };

        // -- Add Label to Layout --
        // Add the label (with close button) to the stakeholder section

        SubTableLayoutPanel.Controls.Add(label, 1, SubTableLayoutPanel.RowCount - 1);

    }

    /// <summary>
    /// Adds a bold section title label to the target section panel
    /// </summary>
    public void RebuildView_Label(ref CustomTableLayoutPanel SubTableLayoutPanel, string LabelText)
    {

        SubTableLayoutPanel.RowCount += 1;
        SubTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        CustomLabel label = new CustomLabel();
        label.Text = LabelText;
        label.Dock = DockStyle.Top;
        label.Font = new Font(GlobalVariables.AppFontName, 16, FontStyle.Bold);
        label.Margin = new Padding(0, 5, 0, 0);
        label.AutoSize = true;

        SubTableLayoutPanel.Controls.Add(label, 1, SubTableLayoutPanel.RowCount - 1);

    }

    /// <summary>
    // Builds and returns a Scenario object based on the current UI input fields
    /// </summary>
    private Scenario GetScenario()
    {

        // -- Assign Basic Information --
        // Populate the scenario name and description from the text fields

        Scenario target = new Scenario();

        target.Name = InputFields["Scenario Name"].TextboxText;
        target.Description = InputFields["Description"].TextboxText;

        // -- Add Stakeholders --
        // Iterate through each stakeholder entry and construct Stakeholder objects

        for (int i = 1; i <= EditingScenario.ListStakeholders.Count; i++)
        {

            if (InputFields.ContainsKey($"Name_{i}"))
            {

                var stakeholder = new Stakeholder
                {
                    Name = InputFields[$"Name_{i}"].TextboxText,
                    Role = InputFields[$"Role_{i}"].TextboxText,
                    Personality = InputFields[$"Personality_{i}"].TextboxText
                };

                target.AddStakeholder(stakeholder);

            }

        }

        // -- Parse Functional Requirements --
        // Split multiline text into individual requirement lines and store in list

        string[] frLines = InputFields["Functional Requirements"].TextboxText
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        target.FunctionalRequirements = frLines.ToList();

        // -- Parse Non-Functional Requirements --
        // Same as above but for non-functional requirements

        string[] nfrLines = InputFields["Non-Functional Requirements"].TextboxText
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        target.NonFunctionalRequirements = nfrLines.ToList();

        // -- Return Completed Scenario --
        // Return the newly assembled scenario containing all UI input data

        return target;

    }

    /// <summary>
    /// Validates and commits the current Scenario.  
    /// Adds a new Scenario or replaces the referenced one, then navigates back to Manage Scenarios.  
    /// Shows an error dialog if validation fails.
    /// </summary>
    private void CreateButton_MouseClick(object sender, MouseEventArgs e)
    {

        // -- Build Scenario from Inputs --
        // Construct a Scenario object from the current UI input fields

        Scenario target = GetScenario(); // Scenario as per current UI inputs

        // -- Validate Scenario Data --
        // Ensure all required fields are filled and data meets validation criteria

        if (target.ValidateScenario() == "Scenario is valid")
        {

            // -- Determine Mode (Create or Edit) --

            if (ReferenceScenario is null)
            { // If creating a new scenario

                Scenarios.Add(target);

            }
            else
            { // If editing an existing scenario, replace it with updated values

                Scenarios.ReplaceScenario(ref ReferenceScenario, ref target);
                EditingScenario = target;

            }

            // -- Navigate Back to Manage Scenarios View --

            Form1 form1 = (Form1)this.FindForm();
            form1.ChangeView("Manage Scenarios");

        }
        else
        { // -- Validation Failed --

            // Show error message with validation feedback

            MessageBox.Show(target.ValidateScenario(), "Scenario Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

    }

    /// <summary>
    /// Creates a rounded, padded section container used to group related inputs.
    /// </summary>
    private CustomTableLayoutPanel CreateSectionBlock()
    {

        var panel = new CustomTableLayoutPanel();
        panel.CornerRadius = 10;
        panel.Padding = new Padding(10);
        panel.Dock = DockStyle.Top;
        panel.BackColor = GlobalVariables.ColorLight;
        panel.AutoSize = true;

        panel.ColumnCount = 3;
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30f));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30f));

        return panel;

    }

}

