# Inventor WPF MVVM Add-in Template

[![.NET 8.0 / .NET 10](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0--windows-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Autodesk Inventor](https://img.shields.io/badge/Autodesk%20Inventor-2025%2B-E51A24?logo=autodesk)](https://www.autodesk.com/products/inventor)
[![Architecture](https://img.shields.io/badge/Architecture-WPF%20%7C%20MVVM-blue)](#architecture-overview)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A production-ready **.NET 8 & .NET 10 / C#** add-in template for **Autodesk Inventor 2025 and beyond**. 

Starting with Inventor 2025, Autodesk migrated Inventor from .NET Framework 4.8 to .NET 8 (Core CLR). This repository provides a modern, clean, and extensible starter template designed specifically for Core CLR runtime versions, featuring a WPF dockable window, MVVM architecture, automatic Inventor theme adaptation (Light & Dark), and one-click F5 Visual Studio debugging.

---

## Key Features

- **.NET 8 & .NET 10 Ready**: Targets `net8.0-windows` out of the box with built-in support for upgrading to `net10.0-windows`, along with `<RollForward>Major</RollForward>` runtime flexibility.
- **Inventor 2025+ Forward Compatibility**: Add-in manifest is configured with `<SupportedSoftwareVersionGreaterThan>28..</SupportedSoftwareVersionGreaterThan>` without an upper cap, ensuring the add-in loads in Inventor 2025 (v29), 2026 (v30), and subsequent releases.
- **Automatic Interop Resolution**: Resolves `Autodesk.Inventor.Interop.dll` whether Autodesk places it under `Bin\Public Assemblies\` or `Bin\`.
- **WPF + MVVM Pattern**: Complete separation of UI ([`FeatureView.xaml`](ExampleFeature/UI/FeatureView.xaml)), ViewModel ([`FeatureViewModel.cs`](ExampleFeature/UI/FeatureViewModel.cs)), and Inventor COM interop.
- **Native Dockable Window Hosting**: Uses Win32 `HwndSource` inside [`DockableWindowBase.cs`](Common/DockableWindowBase.cs) to embed WPF UserControls directly into native Inventor dockable windows.
- **Automatic Lifecycle Management**: Subscribes to Inventor's `DockableWindowsEvents.OnHide` so closing/hiding the window automatically disposes WPF handles and cleans up COM resources.
- **Dynamic Inventor Theming**: [`ThemeUtils.cs`](Common/ThemeUtils.cs) detects Inventor's active theme (Light vs. Dark), applies matching WPF brush resources, and listens to `OnApplicationOptionChange` to re-theme in real-time when the user switches themes in Inventor settings.
- **Ribbon & Icon Utilities**: [`RibbonHelpers.cs`](Common/RibbonHelpers.cs) standardizes tab/panel/button creation and supports both embedded PNG icons and programmatic vector fallback drawing.
- **One-Click F5 Debugging**: Preconfigured with `Properties/launchSettings.json` to launch `Inventor.exe` under the debugger when you press **F5** (the "Play" button) in Visual Studio.
- **Automated Deployment**: Building the project automatically copies binaries and updates the `.addin` manifest in `%APPDATA%\Autodesk\Inventor <Version>\Addins\`.
- **Visual Studio & CLI Template Ready**: Packaged with `.template.config/template.json` for `dotnet new` and Visual Studio's "Create a new project" gallery.

---

## Architecture Overview

The codebase is organized into cleanly decoupled folder spaces:

```
InventorWpfMvvmTemplate/
├── .template.config/
│   └── template.json                # Template definition for Visual Studio & dotnet new (with Framework choices)
├── Common/                          # Shared infrastructure & utilities
│   ├── Globals.cs                   # Global static InvApp variable initialized on startup
│   ├── ViewModelBase.cs             # INotifyPropertyChanged base implementation
│   ├── CommandHelpers.cs            # RelayCommand and RelayCommand<T> for MVVM commands
│   ├── DockableWindowBase.cs        # Manages Inventor DockableWindow & WPF HwndSource hosting
│   ├── RibbonHelpers.cs             # Helper methods for tabs, panels, buttons, and icons
│   └── ThemeUtils.cs                # Theme detection & real-time XAML coloring (Light/Dark)
├── ExampleFeature/                  # Isolated feature space
│   ├── Feature.cs                   # public static Feature class called by the Ribbon button
│   ├── FeatureDockableWindow.cs     # Lifecycle manager for this feature's dockable window
│   └── UI/                          # WPF MVVM presentation layer
│       ├── FeatureViewModel.cs      # ViewModel with example command & state binding
│       ├── FeatureView.xaml         # WPF UserControl using dynamic theme brushes
│       └── FeatureView.xaml.cs      # View code-behind
├── Properties/
│   └── launchSettings.json          # Launches Inventor with debugger on F5 / "Play"
├── Resources/
│   └── Icons/                       # Embedded 16x16 and 32x32 PNG ribbon button icons
│       ├── ExampleIcon16.png
│       └── ExampleIcon32.png
├── StandardAddInServer.cs           # Add-in entry point implementing ApplicationAddInServer
├── InventorWpfMvvmTemplate.addin    # Manifest template with {AssemblyPath} placeholder
├── InventorWpfMvvmTemplate.csproj   # Multi-version forward-compatible project file
└── InventorWpfMvvmTemplate.sln      # Solution file for Visual Studio 2022
```

---

## 2 Ways to Use This Template

### Option 1: Use as a Visual Studio / `dotnet new` Template (Recommended)

This template is configured for the .NET template engine. When installed, it generates projects with a **fresh, unique GUID**, renames all files and namespaces, allows choosing `.NET 8` or `.NET 10`, and appears directly in Visual Studio's **Create a new project** dialog.

#### 1. Install the template
Run this command in any terminal:
```powershell
# From local directory:
dotnet new install .

# Or directly from the GitHub repository:
dotnet new install <GitHub-Repo-URL>
```

#### 2. Create your project
You can now create new add-ins in two ways:

- **Via Visual Studio 2022**:
  1. Open Visual Studio 2022 and click **Create a new project**.
  2. Search for **Inventor** or select **Autodesk Inventor WPF MVVM Add-in (.NET 8 / .NET 10)**.
  3. Enter your project name and select your target framework (`.NET 8` or `.NET 10`).

- **Via Command Line**:
  ```powershell
  # Create with default (.NET 8):
  dotnet new inventor-wpf -n MyCompanyAddin

  # Or create targeting .NET 10:
  dotnet new inventor-wpf -n MyCompanyAddin --Framework net10.0-windows
  ```

---

### Option 2: Clone or Use GitHub "Use this template"

If you prefer to clone this repository directly or click GitHub's **Use this template** button:

1. Clone or download the repository to your machine:
   ```powershell
   git clone <GitHub-Repo-URL> MyInventorAddin
   cd MyInventorAddin
   ```
2. **Generate a unique GUID** for your add-in:
   - In PowerShell:
     ```powershell
     [System.Guid]::NewGuid().ToString("D").ToUpper()
     ```
   - Update `ClientId` and `[Guid("...")]` in [`StandardAddInServer.cs`](StandardAddInServer.cs).
   - Update `<ClassId>` and `<ClientId>` in [`InventorWpfMvvmTemplate.addin`](InventorWpfMvvmTemplate.addin).
3. Open `InventorWpfMvvmTemplate.sln` in Visual Studio 2022.

---

## Multi-Version Compatibility & .NET 10 Migration

### .NET 10 Migration
The project is architected with full forward-compatibility for .NET 10:
1. **Switching to .NET 10**:
   In [`InventorWpfMvvmTemplate.csproj`](InventorWpfMvvmTemplate.csproj), change:
   ```xml
   <TargetFramework>net10.0-windows</TargetFramework>
   ```
   Or pass it at build time:
   ```powershell
   dotnet build InventorWpfMvvmTemplate.csproj /t:Restore,Build -p:TargetFramework=net10.0-windows
   ```
2. **RollForward Setting**:
   `<RollForward>Major</RollForward>` is enabled in `.csproj` to ensure that compiled assemblies can cleanly load in newer Core CLR host processes without runtime mismatch exceptions.

### Targeting Different Inventor Releases (2025, 2026, and beyond)
- **Manifest Versioning**: The `.addin` manifest specifies:
  ```xml
  <SupportedSoftwareVersionGreaterThan>28..</SupportedSoftwareVersionGreaterThan>
  ```
  Inventor 2025 is version `29`. By leaving off an upper version cap, the manifest is automatically recognized and loaded by Inventor 2025, 2026 (v30), and future releases.
- **Custom Version / Directory**:
  To build and deploy for a newer version:
  ```powershell
  dotnet build -p:InventorVersion=2026
  ```
  The build system automatically adjusts the deployment path to `%APPDATA%\Autodesk\Inventor 2026\Addins\` and resolves the appropriate interop binaries.

---

## Development & Debugging Workflow

### One-Click Debugging (F5 / "Play")
1. Open the solution in **Visual Studio 2022**.
2. Select the **Debug** configuration.
3. Verify that the launch profile dropdown next to the green "Play" button is set to **Inventor 2025**.
4. Press **F5** (or click **Play**):
   - The project builds.
   - The `DeployAddin` target automatically deploys the `.addin` manifest and binaries to:
     ```
     %APPDATA%\Autodesk\Inventor 2025\Addins\GKN\InventorWpfMvvmTemplate\
     ```
   - Autodesk Inventor launches automatically with the Visual Studio debugger attached.
5. In Inventor:
   - Navigate to the **Example Tab** on the ribbon (available in ZeroDoc, Part, and Assembly environments).
   - In the **Example Panel**, click **Show Feature Window**.
   - The WPF dockable window docks on the right side.
   - Click the **Example Action** button inside the window to see MVVM data binding in action.
   - Close or hide the window to test automatic lifecycle cleanup (`OnHide`).

---

## How It Works

### 1. WPF Hosting in an Inventor Dockable Window
Autodesk Inventor's `DockableWindow.AddChild()` expects a Win32 HWND handle. [`Common/DockableWindowBase.cs`](Common/DockableWindowBase.cs) abstracts this using `System.Windows.Interop.HwndSource`:

```csharp
var parameters = new HwndSourceParameters(title)
{
    ParentWindow = (nint)dockableWindow.HWND,
    WindowStyle = 0x10000000 | 0x40000000 // WS_VISIBLE | WS_CHILD
};

var hwndSource = new HwndSource(parameters)
{
    RootVisual = content // WPF UserControl
};

dockableWindow.AddChild((nint)hwndSource.Handle);
```

### 2. Auto-Close Lifecycle Management
When users click the 'X' button on a dockable window, Inventor simply hides it. [`ExampleFeature/FeatureDockableWindow.cs`](ExampleFeature/FeatureDockableWindow.cs) hooks Inventor's `DockableWindowsEvents.OnHide` event to completely close and clean up:

```csharp
private static void OnDockableWindowHide(
    DockableWindow dockableWindow,
    EventTimingEnum beforeOrAfter,
    NameValueMap context,
    out HandlingCodeEnum handlingCode)
{
    handlingCode = HandlingCodeEnum.kEventNotHandled;

    if (beforeOrAfter == EventTimingEnum.kAfter && dockableWindow.InternalName == InternalName)
    {
        Close(); // Disposes HwndSource and deletes the dockable window
        handlingCode = HandlingCodeEnum.kEventHandled;
    }
}
```

### 3. Light & Dark Theme Support
[`Common/ThemeUtils.cs`](Common/ThemeUtils.cs) checks `Globals.InvApp.ThemeManager.ActiveTheme.Name` and injects dynamic brush resources into the view:

```xml
<Grid Background="{DynamicResource WindowBackgroundBrush}">
    <Border Background="{DynamicResource CardBackgroundBrush}"
            BorderBrush="{DynamicResource CardBorderBrush}">
        <TextBlock Foreground="{DynamicResource PrimaryTextBrush}" Text="Feature Window" />
    </Border>
</Grid>
```

Available theme dynamic resources:
- `WindowBackgroundBrush`
- `CardBackgroundBrush`
- `CardBorderBrush`
- `PrimaryTextBrush`
- `SecondaryTextBrush`
- `SeparatorBrush`
- `ButtonBackgroundBrush` / `ButtonForegroundBrush`
- `ControlBackgroundBrush` / `ControlBorderBrush`

`ThemeUtils` also listens to Inventor's `OnApplicationOptionChange` event, automatically re-theming open windows if the user switches themes in **Tools -> Application Options -> Colors**.

---

## Customization Guide

### Changing the Vendor Name
In [`InventorWpfMvvmTemplate.csproj`](InventorWpfMvvmTemplate.csproj), change the `<AddinVendorFolder>` property from `GKN` to your company or personal name:
```xml
<AddinVendorFolder>MyCompany</AddinVendorFolder>
```
Your compiled add-in will be deployed to `%APPDATA%\Autodesk\Inventor <Version>\Addins\MyCompany\<AddinName>\`.

### Adding a New Feature
To add another tool to your add-in:
1. Create a new folder (e.g. `BatchExportFeature/`).
2. Add your WPF View and ViewModel under a `UI/` subfolder.
3. Create a `BatchExportDockableWindow : DockableWindowBase` to manage window state.
4. Add a `public static class BatchExportFeature` with an `Execute()` method.
5. In `StandardAddInServer.cs`, call `RibbonHelpers.CreateButtonDefinition()` and `RibbonHelpers.AddButtonToRibbons()` to add your new button to the ribbon.

---

## Prerequisites

- **Autodesk Inventor 2025 or later**
- **Visual Studio 2022** (v17.8+ recommended) with the **.NET desktop development** workload
- **.NET 8.0 SDK** or **.NET 10.0 SDK**

---

## License

This project is licensed under the [MIT License](LICENSE) — you are free to use it as the foundation for your open-source or commercial Inventor add-ins.
