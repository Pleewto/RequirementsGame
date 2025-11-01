using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

public class ProfileLabel : UserControl {

    private PictureBox ProfilePictureBox;
    private Label ProfileNameLabel;
    private Label ProfileShortDescriptionLabel;

    public ProfileLabel() {

        this.Size = new Size(200, 80);
        this.Margin = new Padding(0);

        ProfilePictureBox = new PictureBox {
            SizeMode = PictureBoxSizeMode.Zoom
        };
        this.Controls.Add(ProfilePictureBox);

        ProfileImage = null;

        ProfileNameLabel = new Label {
            Text = "Helena Hills",
            TextAlign = ContentAlignment.BottomLeft,
            Padding = new Padding(5, 0, 0, 1),
            Font = new Font("Calibri", 12, FontStyle.Bold)
        };
        this.Controls.Add(ProfileNameLabel);

        ProfileShortDescriptionLabel = new Label {
            Text = "Restaurant Owner",
            Padding = new Padding(5, 1, 0, 0),
            Font = new Font("Calibri", 10, FontStyle.Italic)
        };
        this.Controls.Add(ProfileShortDescriptionLabel);

        UpdateUi();

        // Events

        this.SizeChanged += ProfileLabel_SizeChanged;

        this.MouseEnter += ProfileLabel_MouseEnter;
        this.MouseLeave += ProfileLabel_MouseLeave;

        ProfilePictureBox.MouseEnter += ProfileLabel_MouseEnter;
        ProfilePictureBox.MouseLeave += ProfileLabel_MouseLeave;

        ProfileNameLabel.MouseEnter += ProfileLabel_MouseEnter;
        ProfileNameLabel.MouseLeave += ProfileLabel_MouseLeave;

        ProfileShortDescriptionLabel.MouseEnter += ProfileLabel_MouseEnter;
        ProfileShortDescriptionLabel.MouseLeave += ProfileLabel_MouseLeave;

    }

    public string ProfileName {

        get => ProfileNameLabel.Text;
        set => ProfileNameLabel.Text = value;

    }

    public string ProfileShortDescription {

        get => ProfileShortDescriptionLabel.Text;
        set => ProfileShortDescriptionLabel.Text = value;

    }

    private void UpdateUi() {

        ProfilePictureBox.Size = new Size(this.Height - 20, this.Height - 20);
        ProfilePictureBox.Location = new Point(10, 10);

        ProfileNameLabel.Location = new Point(10 + ProfilePictureBox.Width, 0);

        if (string.IsNullOrEmpty(ProfileShortDescriptionLabel.Text)) {

            ProfileNameLabel.Size = new Size(this.Width - ProfilePictureBox.Width - 10, this.Height);
            ProfileNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            ProfileShortDescriptionLabel.Visible = false;

        } else {

            ProfileNameLabel.Size = new Size(this.Width - ProfilePictureBox.Width - 10, this.Height / 2);
            ProfileShortDescriptionLabel.Visible = true;
            ProfileShortDescriptionLabel.Size = new Size(this.Width - ProfilePictureBox.Width - 10, this.Height / 2);
            ProfileShortDescriptionLabel.Location = new Point(10 + ProfilePictureBox.Width, this.Height / 2);
        }

    }

    private void ProfileLabel_SizeChanged(object sender, EventArgs e) {

        UpdateUi();

    }

    private void ProfileLabel_MouseEnter(object sender, EventArgs e) {

        this.BackColor = Color.FromArgb(240, 240, 240);

    }

    private void ProfileLabel_MouseLeave(object sender, EventArgs e) {

        this.BackColor = Color.White;

    }

    public Bitmap ProfileImage {

        set
        {

            if (value == null) {

                value = new Bitmap(200, 200);

                using (Graphics tempGraphics = Graphics.FromImage(value)) {

                    tempGraphics.Clear(Color.FromArgb(200, 200, 200));

                }

            }

            int side = Math.Min(value.Width, value.Height);
            Rectangle cropArea = new Rectangle((value.Width - side) / 2, (value.Height - side) / 2, side, side);
            Bitmap squareBitmap = value.Clone(cropArea, PixelFormat.Format32bppArgb);
            Bitmap resizedBitmap = new Bitmap(squareBitmap, 100, 100);

            int diameter = resizedBitmap.Width;
            Bitmap circleBitmap = new Bitmap(diameter, diameter, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(circleBitmap)) {

                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(Color.Black), new Rectangle(0, 0, diameter - 1, diameter - 1));

            }

            for (int x = 0; x < resizedBitmap.Width; x++) {

                for (int y = 0; y < resizedBitmap.Width; y++) {

                    Color currentPixel = resizedBitmap.GetPixel(x, y);
                    int alpha = circleBitmap.GetPixel(x, y).A;

                    resizedBitmap.SetPixel(x, y, Color.FromArgb(alpha, currentPixel));

                }
            }

            ProfilePictureBox.Image = resizedBitmap;

        }
    }
}
