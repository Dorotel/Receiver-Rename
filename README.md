# PDF Receiver Rename Tool

A .NET console application that automatically renames PDF files based on extracted content from the first page of each PDF.

## Features

- Extracts PO Number, Receiver Number, and Date from PDF files
- Renames files using a customizable format: `{DATE} {PO} {RECEIVER}`
- Shows progress for each processed file
- Automatically opens the scans folder when complete (Windows only)
- Supports both interactive and command-line modes

## Requirements

- .NET 8.0 or later
- PDF files containing the required information on the first page

## Installation

1. Clone this repository
2. Navigate to the `ReceiverRename` folder
3. Build the application:
   ```bash
   dotnet build
   ```

## Usage

### Interactive Mode
Run without arguments for interactive configuration:
```bash
dotnet run
```

### Command Line Mode
```bash
dotnet run -- "C:\path\to\scans" "{DATE} {PO} {RECEIVER}"
```

### Test Mode
Run built-in tests:
```bash
dotnet run -- --test
```

### Integration Test
Run end-to-end testing with sample files:
```bash
dotnet run -- --integration-test
```

## Default Settings

- **Scans Folder**: `H:\scans`
- **File Format**: `{DATE} {PO} {RECEIVER}`

## Data Extraction

The tool looks for the following patterns in PDF text:

### PO Number
- `PO-064483` or `PO 064483`
- `P.O.-064483` or `P.O. 064483` 
- `Purchase Order 064483`

### Receiver Number
- `Receiver: 139736`
- `Receiver Number: 139736`
- `Rcv: 139736`
- `Receipt: 139736`

### Date
- Formats: `MM/dd/yyyy`, `M/d/yyyy`, `yyyy/MM/dd`
- Automatically pads month and day with zeros
- Example: `1/5/2025` becomes `01/05/2025`

## File Naming

The default format `{DATE} {PO} {RECEIVER}` produces filenames like:
- `08/01/2025 PO-064483 139736.pdf`

Invalid filename characters are automatically replaced with underscores.

## Error Handling

- Files that cannot be processed are skipped with a warning
- Files with missing required data are skipped
- Duplicate target filenames are detected and skipped
- Processing continues even if individual files fail

## Building for Distribution

To create a standalone executable:
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## Dependencies

- iText7 (v8.0.5) - for PDF text extraction