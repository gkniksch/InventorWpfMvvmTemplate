using System.Windows;
using System.Windows.Media;
using Inventor;
using MediaColor = System.Windows.Media.Color;
using MediaBrush = System.Windows.Media.SolidColorBrush;

namespace InventorWpfMvvmTemplate.Common;

/// <summary>
/// Specifies the UI theme of Autodesk Inventor.
/// </summary>
public enum InventorTheme
{
    Light,
    Dark
}

/// <summary>
/// Utility class to detect Inventor's current theme and apply corresponding XAML colors and brushes.
/// </summary>
public static class ThemeUtils
{
    public static class ResourceKeys
    {
        public const string WindowBackgroundBrush = "WindowBackgroundBrush";
        public const string CardBackgroundBrush = "CardBackgroundBrush";
        public const string CardBorderBrush = "CardBorderBrush";
        public const string PrimaryTextBrush = "PrimaryTextBrush";
        public const string SecondaryTextBrush = "SecondaryTextBrush";
        public const string SeparatorBrush = "SeparatorBrush";
        public const string ButtonBackgroundBrush = "ButtonBackgroundBrush";
        public const string ButtonForegroundBrush = "ButtonForegroundBrush";
        public const string ControlBackgroundBrush = "ControlBackgroundBrush";
        public const string ControlBorderBrush = "ControlBorderBrush";
    }

    private static ApplicationEvents? _applicationEvents;
    private static readonly List<WeakReference<FrameworkElement>> _registeredElements = new();
    private static InventorTheme _lastKnownTheme = InventorTheme.Light;

    /// <summary>
    /// Gets the currently active Inventor theme (Light or Dark).
    /// </summary>
    public static InventorTheme GetCurrentTheme()
    {
        try
        {
            if (Globals.InvApp?.ThemeManager?.ActiveTheme != null)
            {
                string themeName = Globals.InvApp.ThemeManager.ActiveTheme.Name;
                if (!string.IsNullOrEmpty(themeName) &&
                    themeName.IndexOf("Dark", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return InventorTheme.Dark;
                }
            }
        }
        catch
        {
            // Default to Light if Inventor or ThemeManager is not ready
        }

        return InventorTheme.Light;
    }

    /// <summary>
    /// Returns true if the active Inventor theme is Dark theme.
    /// </summary>
    public static bool IsDarkTheme() => GetCurrentTheme() == InventorTheme.Dark;

    /// <summary>
    /// Applies theme color brushes to the specified FrameworkElement's Resources.
    /// Also registers the element to dynamically update if the user changes theme in Inventor options.
    /// </summary>
    /// <param name="element">The WPF FrameworkElement to style.</param>
    /// <param name="autoUpdateOnThemeChange">Whether to subscribe to Inventor application option events to auto-update coloring.</param>
    public static void ApplyTheme(FrameworkElement element, bool autoUpdateOnThemeChange = true)
    {
        if (element == null) return;

        InventorTheme currentTheme = GetCurrentTheme();
        ApplyThemeResources(element.Resources, currentTheme);

        if (autoUpdateOnThemeChange)
        {
            RegisterForThemeChanges(element);
        }
    }

    /// <summary>
    /// Applies theme color brushes into the specified ResourceDictionary.
    /// </summary>
    public static void ApplyThemeResources(ResourceDictionary resources, InventorTheme theme)
    {
        if (resources == null) return;

        bool isDark = theme == InventorTheme.Dark;

        SetBrush(resources, ResourceKeys.WindowBackgroundBrush, isDark ? "#1E1E1E" : "#F8FAFC");
        SetBrush(resources, ResourceKeys.CardBackgroundBrush, isDark ? "#252526" : "#FFFFFF");
        SetBrush(resources, ResourceKeys.CardBorderBrush, isDark ? "#3E3E42" : "#E2E8F0");
        SetBrush(resources, ResourceKeys.PrimaryTextBrush, isDark ? "#F1F5F9" : "#0F172A");
        SetBrush(resources, ResourceKeys.SecondaryTextBrush, isDark ? "#94A3B8" : "#64748B");
        SetBrush(resources, ResourceKeys.SeparatorBrush, isDark ? "#334155" : "#E2E8F0");
        SetBrush(resources, ResourceKeys.ButtonBackgroundBrush, isDark ? "#3B82F6" : "#2563EB");
        SetBrush(resources, ResourceKeys.ButtonForegroundBrush, "#FFFFFF");
        SetBrush(resources, ResourceKeys.ControlBackgroundBrush, isDark ? "#2D2D30" : "#FFFFFF");
        SetBrush(resources, ResourceKeys.ControlBorderBrush, isDark ? "#475569" : "#CBD5E1");
    }

    /// <summary>
    /// Attempts to retrieve a native Inventor theme color for a specific UI component.
    /// </summary>
    public static MediaColor? TryGetComponentThemeColor(string componentName)
    {
        try
        {
            if (Globals.InvApp?.ThemeManager != null)
            {
                Inventor.Color invColor = Globals.InvApp.ThemeManager.GetComponentThemeColor(componentName);
                if (invColor != null)
                {
                    return MediaColor.FromArgb(255, (byte)invColor.Red, (byte)invColor.Green, (byte)invColor.Blue);
                }
            }
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    /// Registers a FrameworkElement to receive automatic color updates when Inventor's theme changes.
    /// </summary>
    public static void RegisterForThemeChanges(FrameworkElement element)
    {
        lock (_registeredElements)
        {
            // Remove garbage-collected or duplicate entries
            _registeredElements.RemoveAll(r => !r.TryGetTarget(out var target) || ReferenceEquals(target, element));
            _registeredElements.Add(new WeakReference<FrameworkElement>(element));

            EnsureApplicationEventsSubscribed();
        }
    }

    private static void EnsureApplicationEventsSubscribed()
    {
        if (_applicationEvents == null && Globals.InvApp != null)
        {
            try
            {
                _applicationEvents = Globals.InvApp.ApplicationEvents;
                _applicationEvents.OnApplicationOptionChange += OnApplicationOptionChange;
                _lastKnownTheme = GetCurrentTheme();
            }
            catch
            {
            }
        }
    }

    private static void OnApplicationOptionChange(
        EventTimingEnum beforeOrAfter,
        NameValueMap context,
        out HandlingCodeEnum handlingCode)
    {
        handlingCode = HandlingCodeEnum.kEventNotHandled;

        if (beforeOrAfter == EventTimingEnum.kAfter)
        {
            InventorTheme newTheme = GetCurrentTheme();
            if (newTheme != _lastKnownTheme)
            {
                _lastKnownTheme = newTheme;
                RefreshRegisteredElements();
                handlingCode = HandlingCodeEnum.kEventHandled;
            }
        }
    }

    /// <summary>
    /// Re-evaluates and applies the current theme across all registered elements.
    /// </summary>
    public static void RefreshRegisteredElements()
    {
        lock (_registeredElements)
        {
            InventorTheme currentTheme = GetCurrentTheme();

            for (int i = _registeredElements.Count - 1; i >= 0; i--)
            {
                if (_registeredElements[i].TryGetTarget(out var element))
                {
                    if (element.Dispatcher.CheckAccess())
                    {
                        ApplyThemeResources(element.Resources, currentTheme);
                    }
                    else
                    {
                        element.Dispatcher.Invoke(() => ApplyThemeResources(element.Resources, currentTheme));
                    }
                }
                else
                {
                    _registeredElements.RemoveAt(i);
                }
            }
        }
    }

    private static void SetBrush(ResourceDictionary resources, string key, string hexColor)
    {
        var mediaColor = (MediaColor)System.Windows.Media.ColorConverter.ConvertFromString(hexColor);
        var brush = new MediaBrush(mediaColor);
        brush.Freeze();
        resources[key] = brush;
    }
}
