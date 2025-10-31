using System.Windows.Forms;
using System.Drawing;
using Requirements_Game;
using System.Linq;

public class ViewHome : CustomTableLayoutPanel {

    public ViewHome() {

        // View Properties

        this.Dock = DockStyle.Fill;
        this.Padding = new Padding(0);

        this.ColumnCount = 2;
        this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));    
        
        this.RowCount = 2;
        this.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
        this.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

        // Game Title

        Label gameTitle = new Label();
        gameTitle.Font = new Font(GlobalVariables.AppFontName, 30, FontStyle.Bold);
        gameTitle.Text = "Requirements Elicitation Game";
        gameTitle.Dock = DockStyle.Fill;
        gameTitle.AutoSize = true;
        gameTitle.TextAlign = ContentAlignment.BottomCenter;
        gameTitle.Padding = new Padding(0, 0, 0, 50);

        this.Controls.Add(gameTitle,0,0);
        this.SetColumnSpan(gameTitle, 2);

        // Play Button

        CustomTextButton playButton = new CustomTextButton();
        playButton.Name = "Play";
        playButton.Text = "Play";
        playButton.Dock = DockStyle.None;
        playButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        playButton.Margin = new Padding(0, 0, 20, 0);
        playButton.BackColor = GlobalVariables.ColorButtonBlack;
        playButton.InteractionEffect = ButtonInteractionEffect.Lighten;
        playButton.ForeColor = Color.White;
        this.Controls.Add(playButton, 0, 1);

        playButton.MouseClick += Button_MouseClick;

        // Help Button

        CustomTextButton helpButton = new CustomTextButton();
        helpButton.Name = "Help";
        helpButton.Text = "Help";
        helpButton.Dock = DockStyle.None;
        helpButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        helpButton.Margin = new Padding(20, 0, 0, 0);
        helpButton.BackColor = GlobalVariables.ColorButtonBlack;
        helpButton.InteractionEffect = ButtonInteractionEffect.Lighten;
        helpButton.ForeColor = Color.White;
        this.Controls.Add(helpButton, 1, 1);

        helpButton.MouseClick += Button_MouseClick;

        // Credits Button
        CustomTextButton creditsButton = new CustomTextButton();
        creditsButton.Name = "Credits";
        creditsButton.Text = "Credits";
        creditsButton.Dock = DockStyle.Fill;
        creditsButton.Anchor = AnchorStyles.None;
        creditsButton.Margin = new Padding(20, 10, 20, 10);
        creditsButton.BackColor = GlobalVariables.ColorButtonBlack;
        creditsButton.InteractionEffect = ButtonInteractionEffect.Lighten;
        creditsButton.ForeColor = Color.White;
        this.Controls.Add(creditsButton, 0, 2);
        this.SetColumnSpan(creditsButton, 2); 

        creditsButton.MouseClick += Button_MouseClick;

    }

    private void Button_MouseClick(object sender, MouseEventArgs e)
    {

        if (e.Button != MouseButtons.Left) return;

        CustomTextButton button = (CustomTextButton)sender;
        string buttonName = button.Name;

        if (buttonName == "Play")
        {

            Form1 form1 = (Form1)this.FindForm();
            form1.ChangeView("Scenarios");

        }
        else if (buttonName == "Help")
        {
            // Navigate to Help view (page with back button in the app title bar)
            Form1 form1 = (Form1)this.FindForm();
            form1.ChangeView("Help");
        }
        else if (buttonName == "Credits")
        {
            Form1 form1 = (Form1)this.FindForm();
            form1.ChangeView("Credits");
        }
    }

}