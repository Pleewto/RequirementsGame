using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

/// <summary>
/// A custom user control that displays a user’s profile information,
/// including a circular profile image, name, and short description.
/// 
/// The control automatically adjusts layout and visual appearance based
/// on size changes, and provides hover effects for better interactivity.
/// If no image is provided, a default placeholder is rendered.
/// </summary>
public class ProfileLabel : UserControl
{

    private PictureBox ProfilePictureBox;
    private Label ProfileNameLabel;
    private Label ProfileShortDescriptionLabel;

    public ProfileLabel()
    {

        // -- UserControl settings

        this.Size = new Size(200, 80);
        this.Margin = new Padding(0);

        // -- ProfilePictureBox --

        ProfilePictureBox = new PictureBox();
        ProfilePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        this.Controls.Add(ProfilePictureBox);

        this.ProfileImage = null;

        // -- ProfilePictureBox --

        ProfileNameLabel = new Label();
        ProfileNameLabel.Text = "Helena Hills";
        ProfileNameLabel.TextAlign = ContentAlignment.BottomLeft;
        ProfileNameLabel.Padding = new Padding(5, 0, 0, 1);
        ProfileNameLabel.Font = new Font("Calibri", 12, FontStyle.Bold);

        this.Controls.Add(ProfileNameLabel);

        // -- ProfileShortDescriptionLabel --

        ProfileShortDescriptionLabel = new Label();
        ProfileShortDescriptionLabel.Padding = new Padding(5, 1, 0, 0);
        ProfileShortDescriptionLabel.Font = new Font("Calibri", 10, FontStyle.Italic);
        this.Controls.Add(ProfileShortDescriptionLabel);

        // -- UpdateUI --
        // Update the UI after the required controls are added,
        // this will resize and place them in the correct positions

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

    /// <summary>
    /// Gets or sets the profile’s display name shown in bold beside the image
    /// </summary>
    public string ProfileName
    {

        get => ProfileNameLabel.Text;
        set => ProfileNameLabel.Text = value;

    }

    /// <summary>
    /// Gets or sets the profile’s short descriptive text displayed below the name
    /// </summary>
    public string ProfileShortDescription
    {

        get => ProfileShortDescriptionLabel.Text;
        set => ProfileShortDescriptionLabel.Text = value;

    }

    /// <summary>
    /// Dynamically updates the layout and positioning of all profile components
    /// (image, name, and description) whenever the control’s size or content changes.
    /// Ensures that the profile picture remains aligned on the left, and the text
    /// elements adjust appropriately depending on whether a short description exists.
    /// </summary>
    private void UpdateUi()
    {

        ProfilePictureBox.Size = new Size(this.Height - 20, this.Height - 20);
        ProfilePictureBox.Location = new Point(10, 10);

        ProfileNameLabel.Location = new Point(10 + ProfilePictureBox.Width, 0);

        if (string.IsNullOrEmpty(ProfileShortDescriptionLabel.Text))
        {

            ProfileNameLabel.Size = new Size(this.Width - ProfilePictureBox.Width - 10, this.Height);
            ProfileNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            ProfileShortDescriptionLabel.Visible = false;

        }
        else
        {

            ProfileNameLabel.Size = new Size(this.Width - ProfilePictureBox.Width - 10, this.Height / 2);
            ProfileShortDescriptionLabel.Visible = true;
            ProfileShortDescriptionLabel.Size = new Size(this.Width - ProfilePictureBox.Width - 10, this.Height / 2);
            ProfileShortDescriptionLabel.Location = new Point(10 + ProfilePictureBox.Width, this.Height / 2);
        }

    }

    /// <summary>
    /// Sets and formats the profile image for display.
    /// The image is cropped to a centered square, resized to a consistent 100×100 dimension,
    /// and masked into a circular shape for a clean avatar appearance.
    /// If no image is provided, a default grey placeholder is generated.
    /// </summary>
    public Bitmap ProfileImage
    {

        set
        {

            // -- Fallback Placeholder --
            // If no image is supplied, generate a 200×200 grey bitmap as a placeholder.

            if (value == null)
            {

                value = new Bitmap(200, 200);

                using (Graphics tempGraphics = Graphics.FromImage(value))
                {

                    tempGraphics.Clear(Color.FromArgb(200, 200, 200));

                }

            }

            // -- Crop to Square --
            // Crop the image to a centered square to maintain consistent proportions

            int side = Math.Min(value.Width, value.Height);
            Rectangle cropArea = new Rectangle((value.Width - side) / 2, (value.Height - side) / 2, side, side);
            Bitmap squareBitmap = value.Clone(cropArea, PixelFormat.Format32bppArgb);

            // -- Resize --
            // Resize the cropped image to 100×100 pixels for uniform display across all profiles

            Bitmap resizedBitmap = new Bitmap(squareBitmap, 100, 100);

            // -- Create Circular Mask --
            // Create a circular mask bitmap with anti-aliasing for smooth edges.

            int diameter = resizedBitmap.Width;
            Bitmap circleBitmap = new Bitmap(diameter, diameter, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(circleBitmap))
            {

                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(new SolidBrush(Color.Black), new Rectangle(0, 0, diameter - 1, diameter - 1));

            }

            // -- Apply Mask --
            // Apply the circular mask by copying only the pixels that fall within the circular area

            for (int x = 0; x < resizedBitmap.Width; x++)
            {

                for (int y = 0; y < resizedBitmap.Width; y++)
                {

                    Color currentPixel = resizedBitmap.GetPixel(x, y);
                    int alpha = circleBitmap.GetPixel(x, y).A;

                    resizedBitmap.SetPixel(x, y, Color.FromArgb(alpha, currentPixel));

                }
            }


            // -- Assign to PictureBox --
            // Display the processed circular image in the profile PictureBox control

            ProfilePictureBox.Image = resizedBitmap;

        }

    }

    /// <summary>
    /// Triggered when the control is resized; updates layout dynamically
    /// </summary>
    private void ProfileLabel_SizeChanged(object sender, EventArgs e)
    {

        UpdateUi();

    }


    /// <summary>
    /// Triggered when the mouse enters the control; applies hover highlight
    /// </summary>
    private void ProfileLabel_MouseEnter(object sender, EventArgs e)
    {

        this.BackColor = Color.FromArgb(240, 240, 240);

    }

    /// <summary>
    /// Triggered when the mouse leaves the control; restores default background
    /// </summary>
    private void ProfileLabel_MouseLeave(object sender, EventArgs e)
    {

        this.BackColor = Color.White;

    }

}