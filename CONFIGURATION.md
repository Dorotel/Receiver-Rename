# PDF Receiver Rename Tool Configuration Examples

## Default Configuration
- Scans Folder: H:\scans
- File Format: {DATE} {PO} {RECEIVER}

## Alternative Formats

### Format Examples:
1. `{DATE} {PO} {RECEIVER}` → "08/01/2025 PO-064483 139736"
2. `{PO}_{RECEIVER}_{DATE}` → "PO-064483_139736_08/01/2025"  
3. `{DATE}_{PO}-{RECEIVER}` → "08/01/2025_PO-064483-139736"
4. `Invoice_{DATE}_{PO}` → "Invoice_08/01/2025_PO-064483"

### Command Line Usage:
```bash
# Custom folder and format
dotnet run -- "C:\MyScans" "{PO}_{RECEIVER}_{DATE}"

# Different format
dotnet run -- "H:\scans" "INV_{DATE}_{PO}"
```

## Troubleshooting

### Common Issues:
1. **PDF text not extracted**: Ensure PDFs contain searchable text (not scanned images)
2. **Missing data**: Check that PO, Receiver, and Date are clearly formatted in the PDF
3. **File already exists**: Tool skips files if target filename already exists

### Supported PDF Text Patterns:

#### PO Number:
- PO-123456
- P.O. 123456
- Purchase Order 123456

#### Receiver:
- Receiver: 123456
- Receiver Number: 123456
- Rcv: 123456
- Receipt: 123456

#### Date:
- 08/01/2025
- 8/1/2025
- 2025/08/01
- Date: 08/01/2025

### Performance Tips:
- Process files in smaller batches for large folders
- Ensure sufficient disk space for renamed files
- Close other applications using the same PDFs during processing