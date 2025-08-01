using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using Tesseract;
using Ghostscript.NET.Rasterizer;

namespace ReceiverRenameGUI
{
    public class PdfExtractedData
    {
        public string PoNumber { get; set; } = string.Empty;
        public string ReceiverNumber { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public bool IsValid => !string.IsNullOrEmpty(PoNumber) ||
                               !string.IsNullOrEmpty(ReceiverNumber) ||
                               !string.IsNullOrEmpty(Date);
    }

    public class PdfProcessor
    {
        public PdfExtractedData ExtractDataFromPdf(string pdfFilePath)
        {
            PdfExtractedData data = null;
            int maxTries = 3;
            int attempt = 0;
            do
            {
                string text = ExtractTextFromPdfImage(pdfFilePath);
                data = new PdfExtractedData
                {
                    PoNumber = ExtractPoNumber(text),
                    ReceiverNumber = ExtractReceiverNumber(text),
                    Date = ExtractAndFormatDate(text)
                };
                attempt++;
                // If any field is found, return immediately
                if (!string.IsNullOrEmpty(data.PoNumber) ||
                    !string.IsNullOrEmpty(data.ReceiverNumber) ||
                    !string.IsNullOrEmpty(data.Date))
                    break;
            } while (attempt < maxTries);
            return data;
        }

        private string ExtractTextFromPdfImage(string pdfFilePath)
        {
            try
            {
                using (var rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(pdfFilePath);
                    using (var img = rasterizer.GetPage(600, 1))
                    using (var stream = new MemoryStream())
                    {
                        img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        stream.Position = 0;
                        string tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
                        string lang = "eng";
                        string trainedDataFile = Path.Combine(tessdataPath, $"{lang}.traineddata");
                        if (!Directory.Exists(tessdataPath) || !File.Exists(trainedDataFile))
                        {
                            return string.Empty;
                        }
                        using (var engine = new TesseractEngine(tessdataPath, lang, EngineMode.Default))
                        using (var pix = Pix.LoadFromMemory(stream.ToArray()))
                        using (var page = engine.Process(pix))
                        {
                            return page.GetText();
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ExtractPoNumber(string text)
        {
            // PO is the first PO-xxxxxx pattern in the text
            var match = Regex.Match(text, @"PO-\d{6}");
            if (match.Success)
                return match.Value;
            return string.Empty;
        }

        private string ExtractReceiverNumber(string text)
        {
            // Find the PO line
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (lines[i].Contains("PO-"))
                {
                    // Look for a 6-digit number before a date in the next line
                    var nextLine = lines[i + 1];
                    // Find all 6-digit numbers
                    var matches = Regex.Matches(nextLine, @"\b\d{6}\b");
                    foreach (Match match in matches)
                    {
                        // Check if a date appears after this number in the line
                        int idx = nextLine.IndexOf(match.Value) + match.Value.Length;
                        string after = nextLine.Substring(idx);
                        if (Regex.IsMatch(after, @"\b\d{1,2}/\d{1,2}/\d{4}\b"))
                            return match.Value;
                    }
                }
            }
            return string.Empty;
        }

        private string ExtractAndFormatDate(string text)
        {
            // Find the PO line
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (lines[i].Contains("PO-"))
                {
                    var nextLine = lines[i + 1];
                    // Find the first date after a 6-digit number
                    var match = Regex.Match(nextLine, @"\b\d{6}\b.*?(\d{1,2}/\d{1,2}/\d{4})");
                    if (match.Success && match.Groups.Count > 1)
                    {
                        if (DateTime.TryParse(match.Groups[1].Value, out DateTime dt))
                            return dt.ToString("MM-dd-yyyy");
                        else if (Regex.IsMatch(match.Groups[1].Value, @"^\d{1,2}/\d{1,2}/\d{4}$"))
                            return match.Groups[1].Value; // fallback: return as-is if it looks like a full date
                    }
                }
            }
            // Fallback: first full date in text
            var fallback = Regex.Match(text, @"\b\d{1,2}/\d{1,2}/\d{4}\b");
            if (fallback.Success)
            {
                if (DateTime.TryParse(fallback.Value, out DateTime dt))
                    return dt.ToString("MM-dd-yyyy");
                else if (Regex.IsMatch(fallback.Value, @"^\d{1,2}/\d{1,2}/\d{4}$"))
                    return fallback.Value;
            }
            return string.Empty;
        }
    }
}