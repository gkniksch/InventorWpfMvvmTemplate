using System.Windows.Input;
using Inventor;
using InventorWpfMvvmTemplate.Common;

namespace InventorWpfMvvmTemplate.ExampleFeature.UI;

/// <summary>
/// ViewModel for the example feature dockable window.
/// </summary>
public class FeatureViewModel : ViewModelBase
{
    private string _activeDocumentName = "No active document";
    private string _statusMessage = "Ready";
    private int _clickCount = 0;

    public FeatureViewModel()
    {
        ExampleButtonCommand = new RelayCommand(OnExampleButtonClicked);
    }

    /// <summary>
    /// Gets or sets the display name of the active Inventor document.
    /// </summary>
    public string ActiveDocumentName
    {
        get => _activeDocumentName;
        set => SetProperty(ref _activeDocumentName, value);
    }

    /// <summary>
    /// Gets or sets the status message displayed to the user.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// Command triggered by the example button.
    /// </summary>
    public ICommand ExampleButtonCommand { get; }

    /// <summary>
    /// Updates the ViewModel with the current active document context.
    /// </summary>
    public void UpdateActiveDocument(Document? activeDocument)
    {
        ActiveDocumentName = activeDocument != null
            ? activeDocument.DisplayName
            : "No active document";
    }

    private void OnExampleButtonClicked()
    {
        _clickCount++;
        StatusMessage = $"Example button clicked {_clickCount} time(s)!";
    }
}
