using InventorWpfMvvmTemplate.Common;

namespace InventorWpfMvvmTemplate.ExampleFeature.UI;

/// <summary>
/// Interaction logic for FeatureView.xaml
/// </summary>
public partial class FeatureView : System.Windows.Controls.UserControl
{
    public FeatureView()
    {
        InitializeComponent();

        // Apply theme colors according to active Inventor Theme (Light/Dark)
        ThemeUtils.ApplyTheme(this);
    }
}
