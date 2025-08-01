using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Ghostscript.NET;

using ReceiverRenameGUI; // Add this to ensure PdfProcessor is found

namespace ReceiverRenameGUI;

public partial class Form1 : Form
{
    private List<(string OldPath, string NewPath, string Status)> previewList = new();
    private bool previewMode = true;

    public Form1()
    {
        InitializeComponent();
#if DEBUG
        textBoxFolder.Text = @"G:\\scans";
#else
        textBoxFolder.Text = @"H:\\scans";
#endif
        string gsDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gsdll64.dll");
        var gsVersionInfo = new Ghostscript.NET.GhostscriptVersionInfo(gsDllPath);
        // Add event handler for listBoxMessages selection
        listBoxMessages.SelectedIndexChanged += listBoxMessages_SelectedIndexChanged;
    }

    private void listBoxMessages_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listBoxMessages.SelectedItem == null)
            return;
        string selected = listBoxMessages.SelectedItem.ToString();
        // Try to extract the original PDF filename from the selected line
        string fileName = null;
        if (selected.Contains("→"))
            fileName = selected.Split('→')[0].Trim();
        else if (selected.Contains("(no change)") || selected.Contains("Skipped"))
            fileName = selected.Split(' ')[0].Trim();
        else
            return;
        string folder = textBoxFolder.Text.Trim();
        string pdfPath = Path.Combine(folder, fileName);
        // Remove the PictureBox logic
    }

    private void buttonBrowse_Click(object sender, EventArgs e)
    {
        using var fbd = new FolderBrowserDialog();
        if (fbd.ShowDialog() == DialogResult.OK)
        {
            textBoxFolder.Text = fbd.SelectedPath;
        }
    }


    private async void buttonProcess_Click(object sender, EventArgs e)
    {
        string folder = textBoxFolder.Text.Trim();
        string format = textBoxFormat.Text.Trim();
        listBoxMessages.Items.Clear();
        progressBar.Value = 0;

        if (!Directory.Exists(folder))
        {
            MessageBox.Show("Scans folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (string.IsNullOrWhiteSpace(format))
        {
            MessageBox.Show("Please enter a file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var pdfFiles = Directory.GetFiles(folder, "*.pdf");
        if (pdfFiles.Length == 0)
        {
            listBoxMessages.Items.Add("No PDF files found in the selected folder.");
            return;
        }

        var processor = new PdfProcessor();
        int processed = 0;
        progressBar.Maximum = pdfFiles.Length;
        previewList.Clear();

        foreach (var file in pdfFiles)
        {
            string fileName = Path.GetFileName(file);
            await System.Threading.Tasks.Task.Yield(); // allow UI update

            var data = processor.ExtractDataFromPdf(file);

            string statusMsg;
            if (!data.IsValid)
            {
                statusMsg = $"{fileName} ⚠ Skipped (data not found)";
                previewList.Add((file, null, $"⚠ Skipped (data not found)"));
            }
            else
            {
                string newName = format
                    .Replace("{DATE}", data.Date)
                    .Replace("{PO}", data.PoNumber)
                    .Replace("{RECEIVER}", data.ReceiverNumber);
                string newPath = Path.Combine(folder, newName + ".pdf");
                if (!file.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                {
                    statusMsg = $"{fileName} → {Path.GetFileName(newPath)} (Will Rename)";
                    previewList.Add((file, newPath, "Will Rename"));
                }
                else
                {
                    statusMsg = $"{fileName} (no change)";
                    previewList.Add((file, newPath, "No Change"));
                }
            }
            processed++;
            progressBar.Value = processed;
            // Show progress in the list as well
            listBoxMessages.Items.Add($"[{processed}/{pdfFiles.Length}] {statusMsg}");
            listBoxMessages.TopIndex = listBoxMessages.Items.Count - 1; // auto-scroll
        }

        // Show preview
        listBoxMessages.Items.Add("Preview of changes:");
        foreach (var (oldPath, newPath, status) in previewList)
        {
            string oldName = Path.GetFileName(oldPath);
            string msg = status == "Will Rename"
                ? $"{oldName} → {Path.GetFileName(newPath)}"
                : status == "No Change"
                    ? $"{oldName} (no change)"
                    : $"{oldName} {status}";
            listBoxMessages.Items.Add(msg);
        }
        listBoxMessages.Items.Add("Preview complete. Click 'Confirm Rename' to confirm and rename.");
        previewMode = false;
        buttonProcess.Text = "Confirm Rename";
        buttonProcess.Click -= buttonProcess_Click;
        buttonProcess.Click -= buttonProcess_Confirm_Click;
        buttonProcess.Click += buttonProcess_Confirm_Click;
    }

    private async void buttonProcess_Confirm_Click(object sender, EventArgs e)
    {
        int processed = 0;
        progressBar.Value = 0;
        progressBar.Maximum = previewList.Count;
        listBoxMessages.Items.Clear();
        foreach (var (oldPath, newPath, status) in previewList)
        {
            string oldName = Path.GetFileName(oldPath);
            if (status == "Will Rename" && !string.IsNullOrEmpty(newPath))
            {
                try
                {
                    File.Move(oldPath, newPath, overwrite: true);
                    listBoxMessages.Items.Add($"{oldName} → {Path.GetFileName(newPath)} ✓ Renamed");
                }
                catch (Exception ex)
                {
                    listBoxMessages.Items.Add($"{oldName} → {Path.GetFileName(newPath)} ⚠ Error: {ex.Message}");
                    // Log full error details for debugging
                    listBoxMessages.Items.Add($"[Error Details] {ex.ToString()}");
                }
            }
            else if (status == "No Change")
            {
                listBoxMessages.Items.Add($"{oldName} (no change)");
            }
            else
            {
                listBoxMessages.Items.Add($"{oldName} {status}");
            }
            processed++;
            progressBar.Value = processed;
            await System.Threading.Tasks.Task.Yield();
        }
        listBoxMessages.Items.Add($"Done. {processed} file(s) processed.");
        previewMode = true;
        buttonProcess.Text = "Process Files";
        buttonProcess.Click -= buttonProcess_Confirm_Click;
        buttonProcess.Click -= buttonProcess_Click;
        buttonProcess.Click += buttonProcess_Click;
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            "File Format Help:\n\n" +
            "You can use the following placeholders in the File Format box to customize the output filename for each PDF:\n" +
            "  {DATE}     - The extracted date (MM-dd-yyyy)\n" +
            "  {PO}       - The extracted PO number (e.g., PO-123456)\n" +
            "  {RECEIVER} - The extracted receiver number (6 digits)\n\n" +
            "Example: '{DATE} {PO} {RECEIVER}' will produce a filename like '08-01-2025 PO-123456 139736.pdf'.\n\n" +
            "You can add your own text or separators as needed.\n\n" +
            "If a value is missing, it will be left blank in the filename.",
            "File Format Help",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }
}
