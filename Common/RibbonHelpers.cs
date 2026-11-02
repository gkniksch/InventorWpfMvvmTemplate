using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Inventor;

namespace InventorWpfMvvmTemplate.Common;

/// <summary>
/// Static helper class to simplify and standardize the creation of Ribbon tabs, panels,
/// button controls, and button icon conversion for Autodesk Inventor.
/// </summary>
public static class RibbonHelpers
{
    /// <summary>
    /// Internal converter subclassing AxHost to access the protected GetIPictureDispFromPicture method.
    /// </summary>
    private sealed class AxHostConverter : AxHost
    {
        private AxHostConverter() : base(string.Empty) { }

        public static object? ConvertImage(Image? image)
        {
            if (image == null) return null;
            return GetIPictureDispFromPicture(image);
        }
    }

    /// <summary>
    /// Converts a System.Drawing.Image to stdole.IPictureDisp object required by Inventor button definitions.
    /// </summary>
    public static object? ConvertToIPictureDisp(Image? image)
    {
        return AxHostConverter.ConvertImage(image);
    }

    /// <summary>
    /// Loads an embedded PNG image resource and converts it to IPictureDisp.
    /// </summary>
    /// <param name="imageFileName">The file name of the embedded icon (e.g. "ExampleIcon16.png").</param>
    /// <param name="resourceNamespace">Optional subfolder/namespace where the icon is located; defaults to "Resources.Icons".</param>
    /// <param name="targetAssembly">Optional assembly to load from; defaults to executing assembly.</param>
    public static object? LoadEmbeddedPngIcon(string imageFileName, string resourceNamespace = "Resources.Icons", Assembly? targetAssembly = null)
    {
        Assembly asm = targetAssembly ?? Assembly.GetExecutingAssembly();
        string fullName = $"{asm.GetName().Name}.{resourceNamespace}.{imageFileName}";
        try
        {
            using Stream? stream = asm.GetManifestResourceStream(fullName);
            if (stream != null)
            {
                using var bmp = new Bitmap(stream);
                return ConvertToIPictureDisp(bmp);
            }
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    /// Loads a PNG image from a physical disk file path and converts it to IPictureDisp.
    /// </summary>
    /// <param name="filePath">Full or relative path to the PNG image file on disk.</param>
    public static object? LoadFilePngIcon(string filePath)
    {
        try
        {
            if (System.IO.File.Exists(filePath))
            {
                using var bmp = new Bitmap(filePath);
                return ConvertToIPictureDisp(bmp);
            }
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    /// Gets a pair of standard (16x16) and large (32x32) icons, attempting to load embedded PNG files first,
    /// and falling back to programmatically generated vector icons if the files are not found.
    /// </summary>
    /// <param name="smallPngName">Embedded PNG resource name for 16x16 icon (e.g. "ExampleIcon16.png").</param>
    /// <param name="largePngName">Embedded PNG resource name for 32x32 icon (e.g. "ExampleIcon32.png").</param>
    /// <param name="resourceNamespace">Subfolder/namespace where the icons are located; defaults to "Resources.Icons".</param>
    public static (object? smallIcon, object? largeIcon) GetOrGenerateIcons(
        string smallPngName = "ExampleIcon16.png",
        string largePngName = "ExampleIcon32.png",
        string resourceNamespace = "Resources.Icons")
    {
        object? small = LoadEmbeddedPngIcon(smallPngName, resourceNamespace);
        object? large = LoadEmbeddedPngIcon(largePngName, resourceNamespace);

        if (small == null)
        {
            using var smallBmp = CreateExampleIconBitmap(16);
            small = ConvertToIPictureDisp(smallBmp);
        }

        if (large == null)
        {
            using var largeBmp = CreateExampleIconBitmap(32);
            large = ConvertToIPictureDisp(largeBmp);
        }

        return (small, large);
    }

    /// <summary>
    /// Programmatically generates a generic example/template icon with an isometric cube/template block and badge.
    /// </summary>
    public static Bitmap CreateExampleIconBitmap(int size)
    {
        var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.Clear(System.Drawing.Color.Transparent);

        float s = size / 32f;

        // Isometric 3D template block / cube representing a component or CAD feature
        // Top face points
        PointF pTop = new(16 * s, 3 * s);
        PointF pTopRight = new(27 * s, 9 * s);
        PointF pCenter = new(16 * s, 15 * s);
        PointF pTopLeft = new(5 * s, 9 * s);

        // Bottom corners
        PointF pBottomLeft = new(5 * s, 21 * s);
        PointF pBottomCenter = new(16 * s, 27 * s);
        PointF pBottomRight = new(27 * s, 21 * s);

        // 1. Draw Left face
        using (var leftBrush = new SolidBrush(System.Drawing.Color.FromArgb(59, 130, 246))) // Medium Blue #3B82F6
        {
            g.FillPolygon(leftBrush, new[] { pTopLeft, pCenter, pBottomCenter, pBottomLeft });
        }

        // 2. Draw Right face
        using (var rightBrush = new SolidBrush(System.Drawing.Color.FromArgb(29, 78, 216))) // Darker Blue #1D4ED8
        {
            g.FillPolygon(rightBrush, new[] { pCenter, pTopRight, pBottomRight, pBottomCenter });
        }

        // 3. Draw Top face
        using (var topBrush = new SolidBrush(System.Drawing.Color.FromArgb(147, 197, 253))) // Light Blue #93C5FD
        {
            g.FillPolygon(topBrush, new[] { pTop, pTopRight, pCenter, pTopLeft });
        }

        // 4. Draw Edges
        using (var edgePen = new Pen(System.Drawing.Color.FromArgb(30, 58, 138), Math.Max(1f, 1.2f * s))) // #1E3A8A
        {
            edgePen.LineJoin = LineJoin.Round;
            g.DrawPolygon(edgePen, new[] { pTop, pTopRight, pCenter, pTopLeft });
            g.DrawPolygon(edgePen, new[] { pTopLeft, pCenter, pBottomCenter, pBottomLeft });
            g.DrawPolygon(edgePen, new[] { pCenter, pTopRight, pBottomRight, pBottomCenter });
        }

        // 5. Example "star/sparkle" badge in top-right suggesting a template / sample
        float starCenterScaleX = 23 * s;
        float starCenterScaleY = 7 * s;
        float starR = 4.5f * s;

        using (var starBrush = new SolidBrush(System.Drawing.Color.FromArgb(250, 204, 21))) // Gold #FACC15
        {
            PointF[] star =
            {
                new(starCenterScaleX, starCenterScaleY - starR),
                new(starCenterScaleX + starR * 0.35f, starCenterScaleY - starR * 0.35f),
                new(starCenterScaleX + starR, starCenterScaleY),
                new(starCenterScaleX + starR * 0.35f, starCenterScaleY + starR * 0.35f),
                new(starCenterScaleX, starCenterScaleY + starR),
                new(starCenterScaleX - starR * 0.35f, starCenterScaleY + starR * 0.35f),
                new(starCenterScaleX - starR, starCenterScaleY),
                new(starCenterScaleX - starR * 0.35f, starCenterScaleY - starR * 0.35f),
            };
            g.FillPolygon(starBrush, star);
        }

        return bmp;
    }

    /// <summary>
    /// Gets programmatically generated standard 16x16 and 32x32 example icons as IPictureDisp objects.
    /// </summary>
    public static (object? smallIcon, object? largeIcon) CreateExampleIcons()
    {
        using var smallBmp = CreateExampleIconBitmap(16);
        using var largeBmp = CreateExampleIconBitmap(32);

        return (ConvertToIPictureDisp(smallBmp), ConvertToIPictureDisp(largeBmp));
    }

    /// <summary>
    /// Creates and registers a new ButtonDefinition with the Inventor CommandManager, supporting custom standard and large icons.
    /// </summary>
    public static ButtonDefinition CreateButtonDefinition(
        string displayName,
        string internalName,
        string clientId,
        string description = "",
        string tooltip = "",
        object? standardIcon = null,
        object? largeIcon = null,
        CommandTypesEnum classification = CommandTypesEnum.kQueryOnlyCmdType)
    {
        ControlDefinitions controlDefs = Globals.InvApp.CommandManager.ControlDefinitions;

        try
        {
            if (controlDefs[internalName] is ButtonDefinition existingBtn)
            {
                return existingBtn;
            }
        }
        catch
        {
            // Not found, proceed to add
        }

        return controlDefs.AddButtonDefinition(
            DisplayName: displayName,
            InternalName: internalName,
            Classification: classification,
            ClientId: clientId,
            DescriptionText: string.IsNullOrWhiteSpace(description) ? displayName : description,
            ToolTipText: string.IsNullOrWhiteSpace(tooltip) ? displayName : tooltip,
            StandardIcon: standardIcon ?? Type.Missing,
            LargeIcon: largeIcon ?? Type.Missing);
    }

    /// <summary>
    /// Gets an existing RibbonTab from the given Ribbon or creates it if it does not exist.
    /// </summary>
    public static RibbonTab GetOrCreateRibbonTab(
        Ribbon ribbon,
        string displayName,
        string internalName,
        string clientId)
    {
        try
        {
            return ribbon.RibbonTabs[internalName];
        }
        catch
        {
            return ribbon.RibbonTabs.Add(displayName, internalName, clientId);
        }
    }

    /// <summary>
    /// Gets an existing RibbonPanel from the given RibbonTab or creates it if it does not exist.
    /// </summary>
    public static RibbonPanel GetOrCreateRibbonPanel(
        RibbonTab tab,
        string displayName,
        string internalName,
        string clientId)
    {
        try
        {
            return tab.RibbonPanels[internalName];
        }
        catch
        {
            return tab.RibbonPanels.Add(displayName, internalName, clientId);
        }
    }

    /// <summary>
    /// Adds a ButtonDefinition to a RibbonTab and RibbonPanel across the specified ribbon environments.
    /// Returns the list of created CommandControl instances for easy cleanup during Deactivate.
    /// </summary>
    public static List<CommandControl> AddButtonToRibbons(
        string[] ribbonNames,
        string tabDisplayName,
        string tabInternalName,
        string panelDisplayName,
        string panelInternalName,
        string clientId,
        ButtonDefinition buttonDef,
        bool useLargeIcon = true)
    {
        var controls = new List<CommandControl>();
        UserInterfaceManager uim = Globals.InvApp.UserInterfaceManager;

        foreach (string ribbonName in ribbonNames)
        {
            try
            {
                Ribbon ribbon = uim.Ribbons[ribbonName];
                RibbonTab tab = GetOrCreateRibbonTab(ribbon, tabDisplayName, tabInternalName, clientId);
                RibbonPanel panel = GetOrCreateRibbonPanel(tab, panelDisplayName, panelInternalName, clientId);

                CommandControl control = panel.CommandControls.AddButton(buttonDef, UseLargeIcon: useLargeIcon);
                controls.Add(control);
            }
            catch
            {
                // Silently ignore environments that are unavailable or not loaded yet
            }
        }

        return controls;
    }

    /// <summary>
    /// Safely releases and removes a collection of CommandControl instances.
    /// </summary>
    public static void CleanUpCommandControls(IEnumerable<CommandControl> controls)
    {
        foreach (CommandControl control in controls)
        {
            try
            {
                control.Delete();
                Marshal.ReleaseComObject(control);
            }
            catch
            {
            }
        }
    }
}
