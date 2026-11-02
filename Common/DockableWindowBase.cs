using System.Windows;
using System.Windows.Interop;
using Inventor;

namespace InventorWpfMvvmTemplate.Common;

/// <summary>
/// Base class providing utilities to create and manage Inventor dockable windows hosting WPF content.
/// </summary>
public abstract class DockableWindowBase
{
    /// <summary>
    /// Creates or retrieves a dockable window in Inventor, embeds the given WPF content via HwndSource,
    /// sets the initial docking state and width, and ensures the window is visible.
    /// </summary>
    /// <param name="clientId">The client/add-in GUID string.</param>
    /// <param name="internalName">The unique internal name of the dockable window.</param>
    /// <param name="title">The caption/title shown on the dockable window.</param>
    /// <param name="width">The initial width in pixels for the dockable window.</param>
    /// <param name="content">The WPF FrameworkElement (e.g. UserControl) to host.</param>
    /// <returns>A tuple containing the Inventor DockableWindow and the created HwndSource host.</returns>
    protected static (DockableWindow dockableWindow, HwndSource hwndSource) CreateDockableWindow(
        string clientId,
        string internalName,
        string title,
        int width,
        FrameworkElement content)
    {
        DockableWindows dockableWindows = Globals.InvApp.UserInterfaceManager.DockableWindows;
        DockableWindow dockableWindow;

        try
        {
            dockableWindow = dockableWindows[internalName];
        }
        catch
        {
            dockableWindow = dockableWindows.Add(clientId, internalName, title);
        }

        // Configure docking state and size if it hasn't been customized previously by the user
        if (!dockableWindow.IsCustomized)
        {
            dockableWindow.SetDockingState(DockingStateEnum.kDockRight);
            if (width > 0)
            {
                dockableWindow.Width = width;
            }
        }

        // Host the WPF element inside a Win32 child window parented to the Inventor DockableWindow HWND
        var parameters = new HwndSourceParameters(title)
        {
            PositionX = 0,
            PositionY = 0,
            Width = width > 0 ? width : 300,
            Height = dockableWindow.Height > 0 ? dockableWindow.Height : 600,
            ParentWindow = (nint)dockableWindow.HWND,
            WindowStyle = 0x10000000 | 0x40000000 // WS_VISIBLE | WS_CHILD
        };

        var hwndSource = new HwndSource(parameters)
        {
            RootVisual = content
        };

        dockableWindow.AddChild((nint)hwndSource.Handle);
        dockableWindow.Visible = true;

        return (dockableWindow, hwndSource);
    }

    /// <summary>
    /// Safely clears and deletes the dockable window and disposes its HwndSource host.
    /// </summary>
    protected static void CloseDockableWindow(DockableWindow? dockableWindow, ref HwndSource? hwndSource)
    {
        if (dockableWindow != null)
        {
            try
            {
                if (dockableWindow.Visible)
                {
                    dockableWindow.Visible = false;
                }

                dockableWindow.Clear();
                dockableWindow.Delete();
            }
            catch
            {
                // Silently ignore if already destroyed or not present
            }
        }

        if (hwndSource != null)
        {
            try
            {
                hwndSource.Dispose();
            }
            catch
            {
            }
            hwndSource = null;
        }
    }
}
