using System;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// Provides methods to temporarily freeze and unfreeze a control and all its child controls
/// to prevent UI redrawing or flickering during updates
/// </summary>
class ControlFreezer {

    /// <summary>
    /// Recursively freezes the specified control and all its child controls
    /// </summary>
    public static void Freeze(Control Control) {

        // Freeze all child controls first

        foreach (Control control in Control.Controls) {

            Freeze(control);

        }

        // Send WM_SETREDRAW message with wParam = 0 to suspend redrawing

        Message targetMessage = Message.Create(Control.Handle, 11, System.IntPtr.Zero, System.IntPtr.Zero);
        NativeWindow targetWindow = NativeWindow.FromHandle(Control.Handle);

        targetWindow.DefWndProc(ref targetMessage);

    }

    /// <summary>
    /// Recursively unfreezes the specified control and all its child controls
    /// </summary>

    public static void Unfreeze(Control Control) {

        // Unfreeze all child controls first

        foreach (Control control in Control.Controls) {

            Unfreeze(control);

        }

        // Send WM_SETREDRAW message with wParam = 1 to resume redrawing

        IntPtr wparam = new IntPtr(1);
        Message targetMessage = Message.Create(Control.Handle, 11, wparam, System.IntPtr.Zero);
        NativeWindow targetWindow = NativeWindow.FromHandle(Control.Handle);

        targetWindow.DefWndProc(ref targetMessage);

        // Force a redraw of the control after re-enabling updates

        Control.Invalidate();
        Control.Refresh();
      
    }

}