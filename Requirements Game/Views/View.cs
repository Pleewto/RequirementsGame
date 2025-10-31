using Requirements_Game;
using System.Windows.Forms;

/// <summary>
/// Base class for all application views.
/// Provides a consistent layout structure and common visual properties
/// </summary>
public class View : Panel
{

    /// <summary>
    /// The main layout panel used by all derived views to contain their UI elements.
    /// </summary>
    protected CustomTableLayoutPanel ViewTableLayoutPanel;


    /// <summary>
    /// Initializes the base view, setting standard properties and layout container.
    /// </summary>
    public View()
    {

        // Base panel settings
        this.Dock = DockStyle.Fill;
        this.AutoScroll = true; // Enable scrolling for overflow content
        this.DoubleBuffered = true; // Reduce flicker during redraws

        // ViewTableLayoutPanel

        ViewTableLayoutPanel = new CustomTableLayoutPanel();

        this.Controls.Add(ViewTableLayoutPanel);

    }

}
