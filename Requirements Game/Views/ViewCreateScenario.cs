using Requirements_Game;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Diagnostics;
using Requirements_Game.Properties;

public class ViewCreateScenario : View
{
    protected Scenario referenceScenario = null;
    protected Scenario editingScenario = null; // Holds a copy of referenceScenario. Allows editing without modifying the original until the changes are explicitly saved or committed
    
    private Dictionary<string, CustomLabelledRichTextBox> inputFields = new Dictionary<string, CustomLabelledRichTextBox>();
    
    public ViewCreateScenario()
    {

        editingScenario = new Scenario(); // Assign editingScenario with an empty instance. This removes the need for 'isEditMode ?' checks
        referenceScenario = null;

        ViewTableLayoutPanel.Dock = DockStyle.Top;
        ViewTableLayoutPanel.AutoSize = true;

        ViewTableLayoutPanel.ColumnCount = 3;
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 800));
        ViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        ViewTableLayoutPanel.RowCount = 0;
        RebuildView();

    }

    public void Clear() {

        editingScenario = new Scenario();

        RebuildView();

    }

    public void RebuildView() {

        ControlFreezer.Freeze(this);

        inputFields.Clear();
        ViewTableLayoutPanel.Controls.Clear();
        ViewTableLayoutPanel.RowStyles.Clear();
        ViewTableLayoutPanel.RowCount = 0;

        var name = editingScenario.Name;
        var description = editingScenario.Description;
        var stakeholders = editingScenario.GetStakeholders().ToList();
        var stakeholderCount = stakeholders.Count;
        var frText = string.Join("\n", editingScenario.FunctionalRequirements);
        var nfrText = string.Join("\n", editingScenario.NonFunctionalRequirements);

        // Scenario Info Block
        var scenarioBlock = CreateSectionBlock();
        RebuildView_LabelledRichTextBox(ref scenarioBlock, "Scenario Name", name);
        RebuildView_LabelledRichTextBox(ref scenarioBlock, "Description", description, 6);
        ViewTableLayoutPanel.Controls.Add(scenarioBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Stakeholders Block
        var stakeholderBlock = CreateSectionBlock();
        string[] personalityOptions = new string[]
        {
            "Neutral",
            "Friendly",
            "Formal",
            "Challenging",
            "Skeptical",
            "Enthusiastic",
            "Detail-Oriented"
        };
        RebuildView_Label(ref stakeholderBlock, "Stakeholders");
        for (int i = 0; i < stakeholders.Count; i++) {
            int index = i + 1;

            RebuildView_StackholderLabelWithClose(ref stakeholderBlock, index);
            RebuildView_LabelledRichTextBox(ref stakeholderBlock, $"Name_{index}", stakeholders[i].Name);
            RebuildView_LabelledRichTextBox(ref stakeholderBlock, $"Role_{index}", stakeholders[i].Role);
            RebuildView_LabelledComboBox(ref stakeholderBlock, $"Personality_{index}", stakeholders[i].Personality, personalityOptions);

        }

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

        addStakeholderButton.MouseClick += (s, e) => {

            editingScenario = this.GetScenario();
            editingScenario.AddStakeholder(new Stakeholder());

            int scrollPosition = this.VerticalScroll.Value;

            RebuildView();

            this.VerticalScroll.Value = scrollPosition;

        };

        stakeholderBlock.RowCount += 1;
        stakeholderBlock.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        stakeholderBlock.Controls.Add(addStakeholderButton, 1, stakeholderBlock.RowCount - 1);

        ViewTableLayoutPanel.Controls.Add(stakeholderBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Requirements Block
        var requirementsBlock = CreateSectionBlock();
        RebuildView_Label(ref requirementsBlock, "Requirements (Optional)");
        RebuildView_LabelledRichTextBox(ref requirementsBlock, "Functional Requirements", frText, 6);
        RebuildView_LabelledRichTextBox(ref requirementsBlock, "Non-Functional Requirements", nfrText, 6);
        ViewTableLayoutPanel.Controls.Add(requirementsBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Create/Save Button
        var buttonBlock = CreateSectionBlock();
        CustomTextButton createButton = new CustomTextButton();
        createButton.Text = referenceScenario is null ? "Create" : "Save";
        createButton.Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Bold);
        createButton.ForeColor = Color.White;
        createButton.BackColor = Color.FromArgb(0, 136, 5);
        createButton.InteractionEffect = ButtonInteractionEffect.Darken;
        createButton.TextAlign = ContentAlignment.MiddleCenter;
        createButton.CornerRadius = 5;
        createButton.Size = new Size(100, 30);
        createButton.Anchor = AnchorStyles.Right;

        createButton.MouseClick += CreateButton_MouseClick;
        buttonBlock.RowCount += 1;
        buttonBlock.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        buttonBlock.Controls.Add(createButton, 1, buttonBlock.RowCount - 1);

        ViewTableLayoutPanel.Controls.Add(buttonBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        ControlFreezer.Unfreeze(this);

    }

    public void RebuildView_Store()
    {

        bool isEditMode = false;
        ControlFreezer.Freeze(this);

        inputFields.Clear();
        ViewTableLayoutPanel.Controls.Clear();
        ViewTableLayoutPanel.RowStyles.Clear();
        ViewTableLayoutPanel.RowCount = 0;

        string name = isEditMode ? editingScenario.Name : "";
        string description = isEditMode ? editingScenario.Description : "";
        
        List<Stakeholder> stakeholders = isEditMode ? editingScenario.GetStakeholders().ToList() : new List<Stakeholder> { new Stakeholder() };
        var stakeholderCount = stakeholders.Count;

        string frText = isEditMode ? string.Join("\n", editingScenario.FunctionalRequirements) : "";
        string nfrText = isEditMode ? string.Join("\n", editingScenario.NonFunctionalRequirements) : "";

        // Scenario Info Block
        var scenarioBlock = CreateSectionBlock();
        RebuildView_LabelledRichTextBox(ref scenarioBlock, "Scenario Name", name);
        RebuildView_LabelledRichTextBox(ref scenarioBlock, "Description", description, 6);
        ViewTableLayoutPanel.Controls.Add(scenarioBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
               
        // Stakeholders Block
        var stakeholderBlock = CreateSectionBlock();
        string[] personalityOptions = new string[]
        {
            "Neutral",
            "Friendly",
            "Formal",
            "Challenging",
            "Skeptical"
        };
        RebuildView_Label(ref stakeholderBlock, "Stakeholders");
        for (int i = 0; i < stakeholders.Count; i++)
        {
            int index = i + 1;
            RebuildView_LabelledRichTextBox(ref stakeholderBlock, $"Name_{index}", stakeholders[i].Name);
            RebuildView_LabelledRichTextBox(ref stakeholderBlock, $"Role_{index}", stakeholders[i].Role);
            RebuildView_LabelledComboBox(ref stakeholderBlock, $"Personality_{index}", stakeholders[i].Personality, personalityOptions);
        }

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

        addStakeholderButton.MouseClick += (s, e) =>
        {          
            editingScenario = this.GetScenario();
            editingScenario.AddStakeholder(new Stakeholder());

            int scrollPosition = this.VerticalScroll.Value;

            RebuildView();

            this.VerticalScroll.Value = scrollPosition;

        };

        stakeholderBlock.RowCount += 1;
        stakeholderBlock.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        stakeholderBlock.Controls.Add(addStakeholderButton, 1, stakeholderBlock.RowCount - 1);

        ViewTableLayoutPanel.Controls.Add(stakeholderBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Requirements Block
        var requirementsBlock = CreateSectionBlock();
        RebuildView_Label(ref requirementsBlock, "Requirements (Optional)");
        RebuildView_LabelledRichTextBox(ref requirementsBlock, "Functional Requirements", frText, 6);
        RebuildView_LabelledRichTextBox(ref requirementsBlock, "Non-Functional Requirements", nfrText, 6);
        ViewTableLayoutPanel.Controls.Add(requirementsBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Create Button
        var buttonBlock = CreateSectionBlock();
        CustomTextButton createButton = new CustomTextButton();
        createButton.Text = isEditMode ? "Save" : "Create";
        createButton.Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Bold);
        createButton.ForeColor = Color.White;
        createButton.BackColor = Color.FromArgb(0, 136, 5);
        createButton.InteractionEffect = ButtonInteractionEffect.Darken;
        createButton.TextAlign = ContentAlignment.MiddleCenter;
        createButton.CornerRadius = 5;
        createButton.Size = new Size(100, 30);
        createButton.Anchor = AnchorStyles.Right;

        createButton.MouseClick += CreateButton_MouseClick;
        buttonBlock.RowCount += 1;
        buttonBlock.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        buttonBlock.Controls.Add(createButton, 1, buttonBlock.RowCount - 1);

        ViewTableLayoutPanel.Controls.Add(buttonBlock, 1, ViewTableLayoutPanel.RowCount++);
        ViewTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        ControlFreezer.Unfreeze(this);

    }

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
        inputFields[LabelText] = richTextBox;
    }

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

        // Store in inputFields for consistency
        inputFields[LabelText] = new CustomLabelledRichTextBox
        {
            LabelText = LabelText,
            TextboxText = comboBox.SelectedItem?.ToString() ?? ""
        };

        // Update binding on selection
        comboBox.SelectedIndexChanged += (s, e) =>
        {
            inputFields[LabelText].TextboxText = comboBox.SelectedItem?.ToString() ?? "";
        };
    }

    public void RebuildView_StackholderLabelWithClose(ref CustomTableLayoutPanel SubTableLayoutPanel, int StakeholderNumber) {

        string stakeholderText = $"Stakeholder {StakeholderNumber}";

        SubTableLayoutPanel.RowCount += 1;
        SubTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        CustomLabel label = new CustomLabel();
        label.Text = stakeholderText;
        label.Dock = DockStyle.Top;
        label.Font = new Font(GlobalVariables.AppFontName, 14, FontStyle.Italic | FontStyle.Bold);
        label.Margin = new Padding(0,20,0,0);
        label.AutoSize = true;

        var closeIcon = (Bitmap)Resources.ResourceManager.GetObject("close_thick");
        closeIcon = new Bitmap(closeIcon, 18, 18);

        var closeButton = new CustomPictureBox();
        closeButton.Dock = DockStyle.Right;
        closeButton.Size = new Size(20, 20);
        closeButton.Padding = new Padding(0,5,0,0);
        closeButton.SizeMode = PictureBoxSizeMode.CenterImage;
        closeButton.Image = closeIcon;
        closeButton.InteractionEffect = ButtonInteractionEffect.Lighten;
        label.Controls.Add(closeButton);

        closeButton.Click += (sender, e) => {

            DialogResult result = MessageBox.Show($"Are you sure you want to delete '{stakeholderText}'?", "Delete Stakeholder", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes) {

                editingScenario = this.GetScenario();
                editingScenario.DeleteStakeHolderByIndex(StakeholderNumber - 1);

                int scrollPosition = this.VerticalScroll.Value;

                RebuildView();

                this.VerticalScroll.Value = scrollPosition;

            }

        };

        SubTableLayoutPanel.Controls.Add(label, 1, SubTableLayoutPanel.RowCount - 1);

    }

    public void RebuildView_Label(ref CustomTableLayoutPanel SubTableLayoutPanel, string LabelText) {

        SubTableLayoutPanel.RowCount += 1;
        SubTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        CustomLabel label = new CustomLabel();       
        label.Text = LabelText;
        label.Dock = DockStyle.Top;
        label.Font = new Font(GlobalVariables.AppFontName, 16, FontStyle.Bold);
        label.Margin = new Padding(0,5,0,0);
        label.AutoSize = true;

        SubTableLayoutPanel.Controls.Add(label, 1, SubTableLayoutPanel.RowCount - 1);

    }

    // Builds and returns a Scenario object based on the current UI input fields
    private Scenario GetScenario() {

        Scenario target = new Scenario();

        target.Name = inputFields["Scenario Name"].TextboxText;
        target.Description = inputFields["Description"].TextboxText;

        for (int i = 1; i <= editingScenario.ListStakeholders.Count; i++) {

            if (inputFields.ContainsKey($"Name_{i}")) {

                var stakeholder = new Stakeholder {
                    Name = inputFields[$"Name_{i}"].TextboxText,
                    Role = inputFields[$"Role_{i}"].TextboxText,
                    Personality = inputFields[$"Personality_{i}"].TextboxText
                };

                target.AddStakeholder(stakeholder);

            }

        }

        // Parse Functional Requirements
        string[] frLines = inputFields["Functional Requirements"].TextboxText
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        target.FunctionalRequirements = frLines.ToList();

        // Parse Non-Functional Requirements
        string[] nfrLines = inputFields["Non-Functional Requirements"].TextboxText
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        target.NonFunctionalRequirements = nfrLines.ToList();

        return target;

    }

    private void CreateButton_MouseClick(object sender, MouseEventArgs e) {

        Scenario target = GetScenario(); // Scenario as per current UI inputs

        if (target.ValidateScenario() == "Scenario is valid") {

            if (referenceScenario is null) {

                Scenarios.Add(target);

            } else {

                Scenarios.ReplaceScenario(ref referenceScenario, ref target);
                editingScenario = target;

            }

            Form1 form1 = (Form1)this.FindForm();
            form1.ChangeView("Manage Scenarios");

        } else {
            
            MessageBox.Show(target.ValidateScenario(), "Scenario Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

    }

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

