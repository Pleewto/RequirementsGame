using System.Drawing;
using System;

/// <summary>
/// Provides methods for lightening and darkening colours
/// </summary>
class ColorManager {

    /// <summary>
    /// Returns a lighter version of the provided colour by the specified factor
    /// </summary>
    public static Color LightenColor(Color color, double LightenFactor) {
        
        if (LightenFactor < 0 || LightenFactor > 1) throw new Exception("LightenFactor must be between 0 and 1");

        // Increase each RGB component toward 255 (white) by the given factor

        int red = (int)(color.R + (255 - color.R) * LightenFactor);
        int green = (int)(color.G + (255 - color.G) * LightenFactor);
        int blue = (int)(color.B + (255 - color.B) * LightenFactor);

        return Color.FromArgb(color.A, red, green, blue);
    }


    /// <summary>
    /// Returns a darker version of the provided colour by the specified factor
    /// </summary>
    public static Color DarkenColor(Color color, double DarkenFactor) {
        
        if (DarkenFactor < 0 || DarkenFactor > 1) throw new ArgumentOutOfRangeException("DarkenFactor must be between 0 and 1");

        // Decrease each RGB component toward 0 (black) by the given factor

        int red = (int)(color.R * (1 - DarkenFactor));
        int green = (int)(color.G * (1 - DarkenFactor));
        int blue = (int)(color.B * (1 - DarkenFactor));

        return Color.FromArgb(color.A, red, green, blue);
    }

}