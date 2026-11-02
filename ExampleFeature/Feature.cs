using Inventor;
using InventorWpfMvvmTemplate.Common;

namespace InventorWpfMvvmTemplate.ExampleFeature;

/// <summary>
/// Feature execution entry point, called from the Inventor Ribbon button.
/// </summary>
public static class Feature
{
    /// <summary>
    /// Executes the feature command, capturing the active document and opening the dockable window.
    /// </summary>
    public static void Execute()
    {
        try
        {
            Document? activeDocument = null;
            try
            {
                activeDocument = Globals.InvApp.ActiveDocument;
            }
            catch
            {
                // ActiveDocument throws or is null when Inventor is in the ZeroDoc state
            }

            FeatureDockableWindow.CreateAndShow(activeDocument);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"An error occurred while executing the feature:\n\n{ex.Message}",
                "Feature Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }
}
