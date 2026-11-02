using System.Runtime.InteropServices;
using Inventor;
using InventorWpfMvvmTemplate.Common;
using InventorWpfMvvmTemplate.ExampleFeature;

namespace InventorWpfMvvmTemplate;

/// <summary>
/// Inventor Add-in entry point implementing ApplicationAddInServer.
/// </summary>
[Guid("36FCD227-1C39-449D-9A67-DE1F5CF2D0C4")]
[ComVisible(true)]
public class StandardAddInServer : ApplicationAddInServer
{
    public const string ClientId = "{36FCD227-1C39-449D-9A67-DE1F5CF2D0C4}";

    private ButtonDefinition? _featureButtonDef;
    private List<CommandControl> _commandControls = new();

    public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
    {
        // 1. Assign the global Inventor Application instance
        Globals.InvApp = addInSiteObject.Application;

        // 2. Load embedded PNG icons (or fallback to generated vector icons)
        var (smallIcon, largeIcon) = RibbonHelpers.GetOrGenerateIcons(
            smallPngName: "ExampleIcon16.png",
            largePngName: "ExampleIcon32.png",
            resourceNamespace: "Resources.Icons");

        // 3. Define the Ribbon button using RibbonHelpers with icons
        _featureButtonDef = RibbonHelpers.CreateButtonDefinition(
            displayName: "Show Feature Window",
            internalName: "InventorWpfMvvmTemplate.ShowFeatureWindow",
            clientId: ClientId,
            description: "Opens the example WPF MVVM dockable window.",
            tooltip: "Show Feature Window",
            standardIcon: smallIcon,
            largeIcon: largeIcon);

        _featureButtonDef.OnExecute += OnFeatureButtonExecute;

        // 4. Add the button to an Example Tab & Panel across target environments
        string[] targetRibbons = ["ZeroDoc", "Part", "Assembly"];
        _commandControls = RibbonHelpers.AddButtonToRibbons(
            ribbonNames: targetRibbons,
            tabDisplayName: "Example Tab",
            tabInternalName: "id_TabExample",
            panelDisplayName: "Example Panel",
            panelInternalName: "id_PanelExample",
            clientId: ClientId,
            buttonDef: _featureButtonDef,
            useLargeIcon: true);
    }

    public void Deactivate()
    {
        // Close dockable window if open
        FeatureDockableWindow.Close();

        // Clean up command controls
        RibbonHelpers.CleanUpCommandControls(_commandControls);
        _commandControls.Clear();

        // Unsubscribe and release button definition
        if (_featureButtonDef != null)
        {
            _featureButtonDef.OnExecute -= OnFeatureButtonExecute;
            Marshal.ReleaseComObject(_featureButtonDef);
            _featureButtonDef = null;
        }

        // Reset global application instance
        Globals.InvApp = null!;

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void ExecuteCommand(int commandId)
    {
        // Deprecated method required by ApplicationAddInServer interface
    }

    public object? Automation => null;

    private void OnFeatureButtonExecute(NameValueMap context)
    {
        Feature.Execute();
    }
}
