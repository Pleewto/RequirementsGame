using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// A custom picturebox button that supports
/// hover/click visual effects (darken or lighten) when interacted with.
/// </summary>
class CustomPictureBox : PictureBox {

    private Image BaseImage;
    private Image EnterImage { get; set; }
    private Image DownImage { get; set; }

    private ButtonInteractionEffect _InteractionEffect; // Stores the type of interaction effect to apply (Darken, Lighten, or None)

    public CustomPictureBox() {

        this.SizeMode = PictureBoxSizeMode.CenterImage;
        this.BackColor = Color.Transparent;
        this._InteractionEffect = ButtonInteractionEffect.None;

        this.BaseImage = null; 
        this.EnterImage = null; 
        this.DownImage = null;

        this.MouseDown += Me_MouseDown;
        this.MouseEnter += Me_MouseEnter;
        this.MouseLeave += Me_MouseLeave;
        this.MouseUp += Me_MouseUp;

    }

    /// <summary>
    /// Gets or sets the visual effect applied when the mouse interacts with the button.
    /// Throws an exception if an undefined enum value is assigned
    /// </summary>
    public ButtonInteractionEffect InteractionEffect {

        get => _InteractionEffect;

        set
        {

            // Validate that the assigned value is a defined enum member

            if (!Enum.IsDefined(typeof(ButtonInteractionEffect), value)) {

                throw new Exception("Invalid ButtonInteractionEffect value");

            }

            _InteractionEffect = value;

            UpdateImages();

        }

    }

    /// <summary>
    /// Hides the inherited Image property to store the base image internally.
    /// This allows temporarily changing the button image on hover/click without losing the original value
    /// </summary>
    public new Image Image {

        get => BaseImage;

        set
        {

            base.Image = value; // Update the actual background
            BaseImage = value; // Store the original color for interaction colors to be calculated and to restore back to original after interaction is complete

            UpdateImages();

        }

    }

    /// <summary>
    /// Generates lighter and darker image variants from the base image 
    /// for hover and click effects
    /// </summary>
    private void UpdateImages() {

        EnterImage = UpdateImages_GetImageVarient(0.25);
        DownImage = UpdateImages_GetImageVarient(0.40);

    }

    /// <summary>
    /// Creates a new image derived from the base image, adjusted by a specified lightening or darkening factor
    /// </summary>
    private Image UpdateImages_GetImageVarient(double Factor) {

        if (BaseImage == null || InteractionEffect == ButtonInteractionEffect.None) { return BaseImage; }

        // BaseImage is cloned and stored as newImage
        // Each pixel is then read and written back to the new bitmap
        // in either a lighter or darker state  

        var newImage = new Bitmap(BaseImage);

        for (int x = 0; x < newImage.Width - 1; x++) {

            for (int y = 0; y < newImage.Height - 1; y++) {

                // Read and adjust pixel colour based on the interaction effect

                var pixelColor = newImage.GetPixel(x, y);

                if (InteractionEffect == ButtonInteractionEffect.Darken) {

                    pixelColor = ColorManager.DarkenColor(pixelColor, Factor);

                } else if (InteractionEffect == ButtonInteractionEffect.Lighten) {

                    pixelColor = ColorManager.LightenColor(pixelColor, Factor);

                }

                newImage.SetPixel(x, y, pixelColor);

            }

        }

        return newImage;

    }

    // --------------------------------------------------------------
    // Mouse Event Handlers
    // --------------------------------------------------------------

    /// <summary>
    /// Change the Picturebox image over to the darker or lighter version when the mouse button is held down
    /// </summary>
    private void Me_MouseDown(object sender, MouseEventArgs e) {

        if (e.Button == MouseButtons.Left) {

            base.Image = DownImage == null ? BaseImage : DownImage;
            this.Refresh();

        }

    }

    /// <summary>
    /// Change the Picturebox image over to the darker or lighter version when the mouse enters the button
    /// </summary>
    private void Me_MouseEnter(object sender, EventArgs e) {

        base.Image = EnterImage == null ? BaseImage : EnterImage;

    }

    /// <summary>
    /// Restores the original color when the mouse leaves the button
    /// </summary>
    private void Me_MouseLeave(object sender, EventArgs e) {

        base.Image = BaseImage;

    }

    /// <summary>
    /// Restores the mouse enter image when the mouse button is released.
    /// If mouse cursor is no longer over button on release, the MouseLeave event will trigger
    /// returning it back to the original image
    /// </summary>
    private void Me_MouseUp(object sender, MouseEventArgs e) {

        base.Image = EnterImage == null ? BaseImage : EnterImage;

    }

}