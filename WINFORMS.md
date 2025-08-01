# PDF Receiver Rename Tool - WinForms Version

## Note for Windows Users

While the main application is a console-based tool that works cross-platform, Windows users who prefer a graphical interface can create a WinForms version.

### Creating a WinForms Version

1. **On a Windows machine with Visual Studio:**
   ```bash
   dotnet new winforms -n ReceiverRenameGUI -f net8.0-windows
   ```

2. **Copy the core logic files:**
   - PdfProcessor.cs
   - PdfExtractedData class

3. **Add the same NuGet package:**
   ```xml
   <PackageReference Include="itext7" Version="8.0.5" />
   ```

4. **Design a simple form with:**
   - TextBox for scans folder path
   - TextBox for file format
   - Browse button for folder selection
   - Process button
   - Progress bar
   - ListBox for progress messages

### Form Layout Example:
```
┌─────────────────────────────────────────┐
│ Scans Folder: [H:\scans        ] [Browse]│
│ File Format:  [{DATE} {PO} {RECEIVER}  ]│
│ ──────────────────────────────────────── │
│ [Process Files]                Progress  │
│ ████████████████████████████████████  95%│
│ ──────────────────────────────────────── │
│ Processing: invoice1.pdf... ✓ Renamed   │
│ Processing: invoice2.pdf... ⚠ Skipped   │
│ Processing: invoice3.pdf... ✓ Renamed   │
│ ...                                      │
└─────────────────────────────────────────┘
```

The console version provides all the same functionality and is suitable for automation and scripting scenarios.