using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// A custom text button that supports rounded corners, that has
/// hover/click visual effects (darken or lighten) when interacted with.
/// Inherits behavior from CustomLabel, which creates the rounded corners
/// </summary>
class CustomTextButton : CustomLabel {

    private Color _BaseColor; // Stores the original background color to restore after interactions
    private ButtonInteractionEffect _InteractionEffect; // Stores the type of interaction effect to apply (Darken, Lighten, or None)

    public CustomTextButton() {

        // Default properties

        this.CornerRadius = 12;
        this.TextAlign = ContentAlignment.MiddleCenter;
        this.BackColor = Color.Gray;
        this._InteractionEffect = ButtonInteractionEffect.None;

        // Mouse events for visual interaction feedback

        this.MouseDown += Me_MouseDown;
        this.MouseEnter += Me_MouseEnter;
        this.MouseUp += Me_MouseUp;
        this.MouseLeave += Me_MouseLeave;

    }

    /// <summary>
    /// Gets or sets the visual effect applied when the mouse interacts with the button.
    /// Throws an exception if an undefined enum value is assigned
    /// </summary>
    public ButtonInteractionEffect InteractionEffect {
        
        get => _InteractionEffect;

        set {

            // Validate that the assigned value is a defined enum member

            if (!Enum.IsDefined(typeof(ButtonInteractionEffect), value)) {
                
                throw new Exception("Invalid ButtonInteractionEffect value");

            }

            _InteractionEffect = value;

        }
    }

    /// <summary>
    /// Hides the inherited BackColor property to store the base color internally.
    /// This allows temporarily changing the button color on hover/click without losing the original value
    /// </summary>
    public new Color BackColor { 
        
        get => _BaseColor;

        set {

            base.BackColor = value; // Update the actual background
            _BaseColor = value; // Store the original color for interaction colors to be calculated and to restore back to original after interaction is complete

        }
    
    }

    // --------------------------------------------------------------
    // Mouse Event Handlers
    // --------------------------------------------------------------

    /// <summary>
    /// Darkens or lightens the button while the left mouse button is held down
    /// </summary>
    private void Me_MouseDown(object sender, MouseEventArgs e) {

        if (e.Button == MouseButtons.Left && InteractionEffect != ButtonInteractionEffect.None) {

            if (InteractionEffect == ButtonInteractionEffect.Darken) {

                base.BackColor = ColorManager.DarkenColor(_BaseColor, 0.30);

            } else if (InteractionEffect == ButtonInteractionEffect.Lighten) {

                base.BackColor = ColorManager.LightenColor(_BaseColor, 0.30);

            }

            this.Refresh(); // Force a repaint to show the effect immediately

        }

    }

    /// <summary>
    /// Darkens or lightens the button when the mouse enters the button
    /// </summary>
    private void Me_MouseEnter(object sender, EventArgs e) {

        if (InteractionEffect == ButtonInteractionEffect.Darken) {

            base.BackColor = ColorManager.DarkenColor(_BaseColor, 0.15);

        } else if (InteractionEffect == ButtonInteractionEffect.Lighten) {

            base.BackColor = ColorManager.LightenColor(_BaseColor, 0.15);

        }

        this.Refresh(); // Force a repaint to show the effect immediately

    }

    /// <summary>
    /// Restores the original color when the mouse leaves the button
    /// </summary>
    private void Me_MouseLeave(object sender, EventArgs e) {

        base.BackColor = _BaseColor;

    }

    /// <summary>
    /// Restores the mouse enter color when the mouse button is released.
    /// If mouse cursor is no longer over button on release, the MouseLeave event will trigger
    /// returning it back to the original color
    /// </summary>
    private void Me_MouseUp(object sender, MouseEventArgs e) {

        if (InteractionEffect == ButtonInteractionEffect.Darken) {

            base.BackColor = ColorManager.DarkenColor(_BaseColor, 0.15);

        } else if (InteractionEffect == ButtonInteractionEffect.Darken) {

            base.BackColor = ColorManager.LightenColor(_BaseColor, 0.15);

        }

        this.Refresh(); // Force a repaint to show the effect immediately

    }

}
