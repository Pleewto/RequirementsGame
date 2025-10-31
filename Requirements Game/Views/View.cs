using Requirements_Game;
using System.Windows.Forms;

public class View : Panel {

    protected CustomTableLayoutPanel ViewTableLayoutPanel;

    public View() {

        this.Dock = DockStyle.Fill;
        this.AutoScroll = true;
        this.DoubleBuffered = true;
        
        // ViewTableLayoutPanel

        ViewTableLayoutPanel = new CustomTableLayoutPanel();
        
        this.Controls.Add(ViewTableLayoutPanel);

    }

}
