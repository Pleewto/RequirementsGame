using System.Drawing.Drawing2D;
using System.Drawing;
using System;
using System.Windows.Forms;

class CustomLabel : Label {

    /// <summary>
    /// Controls the roundness of the label’s corners (in pixels).
    /// A value of 0 means no rounded corners
    /// </summary>
    public int CornerRadius { get; set; }

    public CustomLabel() {

        // Default properties

        this.Font = new Font(GlobalVariables.AppFontName, 11, FontStyle.Regular);
        this.TextAlign = ContentAlignment.MiddleLeft;
        this.CornerRadius = 0;
        this.DoubleBuffered = true; // Enable double-buffering to reduce flicker when redrawing

    }

    /// <summary>
    /// Custom background painting logic to support rounded corners.
    /// Overrides default background painting
    /// </summary>
    protected override void OnPaintBackground(PaintEventArgs e) {

        // Fill the background to match the parent control. A rounded 
        // rectangle will then be drawn over this with the label's actual backcolor.
        // If there’s no parent or corner rounding is disabled, 
        // fill with the label’s own backcolor (non-rounded rectangle) and return

        var backColor = (this.Parent == null || CornerRadius == 0) ? this.BackColor : this.Parent.BackColor;
        var backgroundBrush = new SolidBrush(backColor);
       
        e.Graphics.FillRectangle(backgroundBrush, new Rectangle(0, 0, this.Width, this.Height));
        
        if (CornerRadius <= 0) return; // If rounding is not requested, exit early to avoid unnecessary calculations

        // Get the label's rectangle so that if can be used to calculate the full
        // rounded corner path. The corner diameter will be the smaller of the control’s width, height,
        // or twice the CornerRadius to ensure arcs fit cleanly within the label’s dimensions

        var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
        var path = new GraphicsPath();
        var diameter = Math.Min(Math.Min(this.Width, this.Height), CornerRadius * 2);

        // Build the rectangle path with the rounded corners

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90); // Top-left corner
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90); // Top-right corner
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90); // Bottom-right corner
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90); // Bottom-left corner
        path.CloseFigure();

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // Enable anti-aliasing for smoother curves
        e.Graphics.FillPath(new SolidBrush(this.BackColor), path); // Fill the rounded rectangle with the label's background color

    }

}