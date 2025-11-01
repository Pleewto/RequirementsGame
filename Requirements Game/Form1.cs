using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Requirements_Game.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Requirements_Game
{

    /// <summary>
    /// The main application window for the *Requirements Elicitation Game*.
    /// Manages layout, navigation, and dynamic view switching between different screens
    /// such as Home, Scenarios, Create/Edit Scenario, Chat, Help, and Credits.
    /// Also initializes the title bar, button icons, and handles imports/exports and manual access.
    /// </summary>
    public partial class Form1 : Form
    {

        public Form1() { InitializeComponent(); }

        private TableLayoutPanel MainTableLayoutPanel;
        private TableLayoutPanel TitleBarTableLayoutPanel;
        private Label TitleLabel;

        private string CurrentViewTitle;
        private string BackViewTitle;

        private Dictionary<string, Control> ViewDictionary;
        private Dictionary<string, CustomPictureBox> CustomPictureBoxDictionary;

        /// <summary>
        /// Handles all initialization logic when the main form loads.  
        /// Sets up global references, loads saved scenarios, creates the main UI layout  
        /// (title bar and content area), initializes navigation icons and view controls,  
        /// and displays the default "Home" view on startup.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {

            GlobalVariables.MainForm = this;

            // -- Form1
            // App specific properties and class initialisation

            this.Text = "Requirements Elicitation Game";
            this.Icon = Resources.AppIcon;
            this.BackColor = Color.White;
            this.CurrentViewTitle = "";
            this.BackViewTitle = "";

            // -- Load Scenarios
            // Load scenarios from application local computer storage

            Scenarios.LoadFromFile(FileSystem.ScenariosFilePath);

            // -- MainTableLayoutPanel
            // Split into two row parts, the title bar and the main sectin
            // Main section will be where the different views are added and removed

            MainTableLayoutPanel = new TableLayoutPanel();
            MainTableLayoutPanel.Dock = DockStyle.Fill;
            MainTableLayoutPanel.Padding = new Padding(0);
            MainTableLayoutPanel.ColumnCount = 1;
            MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            MainTableLayoutPanel.RowCount = 2;
            MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
            MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            Controls.Add(MainTableLayoutPanel);

            // -- TitleBarTableLayoutPanel
            // Except for the percent column, add columns to hold buttons, such as 'Back' button
            // Add the same column on the other side regardless of whether it is holding a button so
            // that the percent column (holds the view's title) is always centre

            TitleBarTableLayoutPanel = new TableLayoutPanel();
            TitleBarTableLayoutPanel.Dock = DockStyle.Fill;
            TitleBarTableLayoutPanel.Padding = new Padding(0);
            TitleBarTableLayoutPanel.ColumnCount = 8;
            TitleBarTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12));
            TitleBarTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
            TitleBarTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
            TitleBarTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            TitleBarTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
            TitleBarTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
            TitleBarTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
            TitleBarTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12));
            TitleBarTableLayoutPanel.RowCount = 1;
            TitleBarTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            TitleBarTableLayoutPanel.BackColor = GlobalVariables.ColorLight;
            TitleBarTableLayoutPanel.Margin = new Padding(0);

            MainTableLayoutPanel.Controls.Add(TitleBarTableLayoutPanel, 0, 0);

            // -- TitleLabel

            TitleLabel = new Label();
            TitleLabel.Dock = DockStyle.Fill;
            TitleLabel.Font = new Font(GlobalVariables.AppFontName, 16, FontStyle.Bold);
            TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            TitleLabel.Text = "View Title";

            TitleBarTableLayoutPanel.Controls.Add(TitleLabel, 3, 0);

            // -- Picture Buttons
            // These buttons are created and added to a dictionary for quick access later.
            // They are not immediately displayed on the form but are dynamically added or
            // removed depending on the active view

            CustomPictureBoxDictionary = new Dictionary<string, CustomPictureBox>();

            foreach (string resourceName in new[] { "back", "create", "edit", "import", "export", "manual" })
            {
                Bitmap icon = (Bitmap)Resources.ResourceManager.GetObject(resourceName);

                CustomPictureBox CustomPictureBox = new CustomPictureBox();
                CustomPictureBox.Name = resourceName;
                CustomPictureBox.Dock = DockStyle.Fill;
                CustomPictureBox.InteractionEffect = ButtonInteractionEffect.Lighten;
                CustomPictureBox.Image = icon;
                CustomPictureBox.Cursor = Cursors.Hand;
                CustomPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                CustomPictureBox.MouseClick += CustomPictureBox_MouseClick;

                CustomPictureBoxDictionary.Add(resourceName, CustomPictureBox);
            }

            TitleBarTableLayoutPanel.Controls.Add(CustomPictureBoxDictionary["back"], 1, 0);

            // -- Views
            // This are created an added to a dictionary for faster access later

            ViewDictionary = new Dictionary<string, Control>();
            ViewDictionary.Add("Home", new ViewHome());
            ViewDictionary.Add("Scenarios", new ViewScenarios());
            ViewDictionary.Add("Manage Scenarios", new ViewManageScenarios());
            ViewDictionary.Add("Create Scenario", new ViewCreateScenario());
            ViewDictionary.Add("Edit Scenario", new ViewEditScenario());
            ViewDictionary.Add("Help", new ViewHelp());
            ViewDictionary.Add("Credits", new ViewCredits());

            // Display the 'Home' view

            ChangeView("Home");

        }

        /// <summary>
        /// Switches the main application view to the specified screen (e.g., Home, Scenarios, Chat, etc.).
        /// Handles dynamic navigation updates, UI freezing to prevent flicker, title bar visibility,
        /// and view-specific button configuration
        /// </summary>
        public void ChangeView(string newViewTitle, Scenario Scenario = null)
        {

            // Update back button navigation

            if (newViewTitle == "Scenarios") BackViewTitle = "Home";
            if (newViewTitle == "Manage Scenarios") BackViewTitle = "Scenarios";
            if (newViewTitle == "Edit Scenario" || newViewTitle == "Create Scenario") BackViewTitle = "Manage Scenarios";
            if (newViewTitle == "Chat") BackViewTitle = "Scenarios";
            if (newViewTitle == "Help") BackViewTitle = "Home";
            if (newViewTitle == "Credits") BackViewTitle = "Home";

            // Freeze UI so that the user doesn't see flicker and to slightly improve performance

            FreezeUi();

            // Allow Chat to be created on demand
            if (!ViewDictionary.ContainsKey(newViewTitle) && newViewTitle != "Chat")
                throw new Exception("View not found");

            // Remove current view

            if (!string.IsNullOrEmpty(CurrentViewTitle) && ViewDictionary.ContainsKey(CurrentViewTitle))
            {

                MainTableLayoutPanel.Controls.Remove(ViewDictionary[CurrentViewTitle]);

            }

            // Always recreate Chat so it uses the latest selected scenario

            if (newViewTitle == "Chat")
            {

                if (Scenario != null) GlobalVariables.CurrentScenario = Scenario;

                if (ViewDictionary.ContainsKey("Chat"))
                {

                    var old = ViewDictionary["Chat"];
                    old.Dispose();
                    ViewDictionary.Remove("Chat");

                }

                ViewDictionary["Chat"] = new ViewChat();

            }

            // Add new view to form

            CurrentViewTitle = newViewTitle;
            MainTableLayoutPanel.Controls.Add(ViewDictionary[newViewTitle], 0, 1);

            // Remove all icons expect the back button

            foreach (CustomPictureBox CustomPictureBox in CustomPictureBoxDictionary.Values)
            {

                if (CustomPictureBox.Name == "back" || CustomPictureBox.Parent == null) continue;

                TitleBarTableLayoutPanel.Controls.Remove(CustomPictureBox);

            }

            // Update title and hide the title bar if 'Home' view

            if (newViewTitle == "Home")
            {

                TitleBarVisible = false;

            }
            else
            {

                TitleBarVisible = true;
                TitleLabel.Text = newViewTitle;

            }

            // -- View specific button and setup logic
            // Dynamically configures title bar buttons and behavior based on the active view

            int lastColumn = TitleBarTableLayoutPanel.ColumnCount - 2;

            if (newViewTitle == "Scenarios")
            {

                TitleBarTableLayoutPanel.Controls.Add(CustomPictureBoxDictionary["edit"], lastColumn, 0);

            }
            else if (newViewTitle == "Manage Scenarios")
            {

                int exportColumn = lastColumn;
                int importColumn = exportColumn - 1;
                int createColumn = importColumn - 1;

                TitleBarTableLayoutPanel.Controls.Add(CustomPictureBoxDictionary["create"], createColumn, 0);
                TitleBarTableLayoutPanel.Controls.Add(CustomPictureBoxDictionary["import"], importColumn, 0);
                TitleBarTableLayoutPanel.Controls.Add(CustomPictureBoxDictionary["export"], exportColumn, 0);

            }
            else if (newViewTitle == "Edit Scenario")
            {

                ViewEditScenario editScenario = (ViewEditScenario)ViewDictionary["Edit Scenario"];

                editScenario.ChangeScenario(ref Scenario);

            }
            else if (newViewTitle == "Create Scenario")
            {

                ViewCreateScenario createScenario = (ViewCreateScenario)ViewDictionary["Create Scenario"];

                createScenario.Clear();

            }
            else if (newViewTitle == "Help")
            {

                TitleBarVisible = true;
                TitleLabel.Text = newViewTitle;

                TitleBarTableLayoutPanel.Controls.Add(CustomPictureBoxDictionary["manual"], 6, 0);
            }

            // Unfreeze Ui

            UnfreezeUi();

        }

        /// <summary>
        /// Provides methods to temporarily freeze the MainTableLayoutPanel
        /// to prevent UI redrawing or flickering during updates
        /// </summary>
        private void FreezeUi()
        {

            if (MainTableLayoutPanel == null) return;

            this.Cursor = Cursors.WaitCursor;

            Message message = Message.Create(MainTableLayoutPanel.Handle, 11, IntPtr.Zero, IntPtr.Zero);
            NativeWindow nativeWindow = NativeWindow.FromHandle(MainTableLayoutPanel.Handle);

            nativeWindow.DefWndProc(ref message);

        }

        /// <summary>
        /// Methods to unfreeze the MainTableLayoutPanel
        /// </summary>
        private void UnfreezeUi()
        {

            if (MainTableLayoutPanel == null) return;

            IntPtr wparam = new IntPtr(1);
            Message message = Message.Create(MainTableLayoutPanel.Handle, 11, wparam, IntPtr.Zero);
            NativeWindow nativeWindow = NativeWindow.FromHandle(MainTableLayoutPanel.Handle);

            nativeWindow.DefWndProc(ref message);

            MainTableLayoutPanel.Invalidate();
            MainTableLayoutPanel.Refresh();

            this.Cursor = Cursors.Default;

        }

        /// <summary>
        /// Hide/Unhides the title bar by adjusting its row height.
        /// </summary>
        private bool TitleBarVisible
        {

            set { MainTableLayoutPanel.RowStyles[0].Height = value ? 60 : 0; }

        }

        /// <summary>
        /// Handles mouse click events for all title bar <see cref="CustomPictureBox"/> buttons.
        /// Performs navigation and file-related actions such as importing, exporting, and opening the manual.
        /// </summary>
        private void CustomPictureBox_MouseClick(object sender, MouseEventArgs e)
        {

            // -- Verify Mouse Input --
            // Only continue if the user clicked the left mouse button

            if (e.Button != MouseButtons.Left) return;

            // -- Identify Clicked Button --
            // Cast the sender to a CustomPictureBox and retrieve its name for comparison

            CustomPictureBox CustomPictureBox = (CustomPictureBox)sender;
            string CustomPictureBoxName = CustomPictureBox.Name;

            if (CustomPictureBoxName == "back")
            {

                ChangeView(BackViewTitle);

            }
            else if (CustomPictureBoxName == "edit")
            {

                ChangeView("Manage Scenarios");

            }
            else if (CustomPictureBoxName == "create")
            {

                ChangeView("Create Scenario");

            }
            else if (CustomPictureBoxName == "import")
            {

                // Opens a dialog for importing scenarios from a JSON file

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {

                    openFileDialog.Filter = "JSON Files (*.json)|*.json";
                    openFileDialog.Title = "Import Scenarios";

                    if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                    string path = openFileDialog.FileName;
                    var importedScenarios = JsonFileManager.LoadScenarios(path);
                    var existingScenarios = Scenarios.GetScenarios();

                    foreach (var scenario in importedScenarios)
                    {

                        // -- Duplicate Check --
                        // Skips adding a scenario if an identical one already exists

                        bool isDuplicate = existingScenarios.Any(existing =>
                            existing.Name == scenario.Name &&
                            existing.Description == scenario.Description &&
                            existing.Prompt == scenario.Prompt &&
                            Enumerable.SequenceEqual(existing.FunctionalRequirements, scenario.FunctionalRequirements) &&
                            Enumerable.SequenceEqual(existing.NonFunctionalRequirements, scenario.NonFunctionalRequirements) &&
                            existing.ListStakeholders.Count == scenario.ListStakeholders.Count &&
                            !existing.ListStakeholders.Where((s, i) =>
                                s.Name != scenario.ListStakeholders[i].Name ||
                                s.Role != scenario.ListStakeholders[i].Role ||
                                s.Personality != scenario.ListStakeholders[i].Personality
                            ).Any()
                        );

                        if (!isDuplicate) Scenarios.Add(scenario);

                    }

                }

            }
            else if (CustomPictureBoxName == "export")
            {

                // Allows the user to export all scenarios to a JSON file

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {

                    saveFileDialog.Title = "Export Scenarios";
                    saveFileDialog.Filter = "JSON Files (*.json)|*.json";
                    saveFileDialog.FileName = "Scenarios_Requirements_Game.json";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {

                        string selectedPath = saveFileDialog.FileName;
                        Scenarios.SaveToFile(selectedPath, Scenarios.GetScenarios().ToList());

                    }
                }

            }
            else if (CustomPictureBoxName == "manual")
            {

                // Opens the embedded User Manual PDF using the system's default PDF viewer

                try
                {

                    string tempPath = Path.Combine(Path.GetTempPath(), "UserManual.pdf");
                    File.WriteAllBytes(tempPath, Resources.UserManual); // Embedded PDF as byte[]

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = tempPath,
                        UseShellExecute = true
                    });

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Unable to open manual: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }

        }
    }
}

