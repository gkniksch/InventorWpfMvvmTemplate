namespace InventorWpfMvvmTemplate.Common;

/// <summary>
/// Provides global access to the Inventor Application instance.
/// </summary>
public static class Globals
{
    /// <summary>
    /// Gets or sets the top-level Inventor Application object, initialized when the add-in activates.
    /// </summary>
    public static Inventor.Application InvApp { get; set; } = null!;
}
