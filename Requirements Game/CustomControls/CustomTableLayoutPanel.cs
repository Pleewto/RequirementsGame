using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class CustomTableLayoutPanel : TableLayoutPanel {

    public int CornerRadius { get; set; }

    public CustomTableLayoutPanel() {

        this.CornerRadius = 0;
        this.DoubleBuffered = true;

    }

    protected override void OnPaintBackground(PaintEventArgs e) {

        // Paint Background

        Color backColor = (this.Parent == null || CornerRadius == 0) ? this.BackColor : this.Parent.BackColor;
        SolidBrush backgroundBrush = new SolidBrush(backColor);

        e.Graphics.FillRectangle(backgroundBrush, new Rectangle(0, 0, this.Width, this.Height));

        if (CornerRadius <= 0) return;

        // Paint rectangle with rounded corners

        Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
        GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        int diameter = Math.Min(Math.Min(this.Width, this.Height), CornerRadius * 2);

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        SolidBrush brush = new SolidBrush(this.BackColor);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.FillPath(brush, path);

    }

}