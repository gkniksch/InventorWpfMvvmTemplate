using System.Windows.Interop;
using Inventor;
using InventorWpfMvvmTemplate.Common;
using InventorWpfMvvmTemplate.ExampleFeature.UI;

namespace InventorWpfMvvmTemplate.ExampleFeature;

/// <summary>
/// Manages the lifecycle and display of the feature dockable window in Inventor.
/// </summary>
public class FeatureDockableWindow : DockableWindowBase
{
    private static FeatureViewModel? _viewModel;
    private static DockableWindow? _dockableWindow;
    private static HwndSource? _hwndSource;
    private static DockableWindowsEvents? _dockableWindowsEvents;

    private const string InternalName = "InventorWpfMvvmTemplate.FeatureDockableWindow";
    private const string Title = "Feature Dockable Window";
    private const int WindowWidth = 350;

    /// <summary>
    /// Creates (if necessary) and displays the dockable window, wiring up the WPF ViewModel and View,
    /// subscribing to DockableWindowsEvents so the window auto-closes when hidden.
    /// </summary>
    /// <param name="activeDocument">The currently active Inventor document, if any.</param>
    public static void CreateAndShow(Document? activeDocument)
    {
        // 1. Subscribe to DockableWindowsEvents so that hiding the window closes it automatically
        if (_dockableWindowsEvents == null)
        {
            _dockableWindowsEvents = Globals.InvApp.UserInterfaceManager.DockableWindows.Events;
            _dockableWindowsEvents.OnHide += OnDockableWindowHide;
        }

        // 2. Create a new FeatureViewModel if it doesn't already exist
        if (_viewModel == null)
        {
            _viewModel = new FeatureViewModel();
        }

        // Update active document information on the ViewModel
        _viewModel.UpdateActiveDocument(activeDocument);

        // 3. Always create a new FeatureView and apply the ViewModel as its DataContext
        var view = new FeatureView
        {
            DataContext = _viewModel
        };

        // 4. Set title, internal name, and window width, calling DockableWindowBase.CreateDockableWindow at the end
        string clientId = StandardAddInServer.ClientId;
        (_dockableWindow, _hwndSource) = CreateDockableWindow(clientId, InternalName, Title, WindowWidth, view);
    }

    private static void OnDockableWindowHide(
        DockableWindow dockableWindow,
        EventTimingEnum beforeOrAfter,
        NameValueMap context,
        out HandlingCodeEnum handlingCode)
    {
        handlingCode = HandlingCodeEnum.kEventNotHandled;

        if (beforeOrAfter == EventTimingEnum.kAfter && dockableWindow.InternalName == InternalName)
        {
            Close();
            handlingCode = HandlingCodeEnum.kEventHandled;
        }
    }

    /// <summary>
    /// Closes the dockable window, disposes its WPF hosting resources, and unsubscribes from events.
    /// </summary>
    public static void Close()
    {
        if (_dockableWindowsEvents != null)
        {
            try
            {
                _dockableWindowsEvents.OnHide -= OnDockableWindowHide;
            }
            catch
            {
            }
            _dockableWindowsEvents = null;
        }

        CloseDockableWindow(_dockableWindow, ref _hwndSource);
        _dockableWindow = null;
    }
}
