using System;
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace ReceiverRename
{
    public class PdfExtractedData
    {
        public string PoNumber { get; set; } = string.Empty;
        public string ReceiverNumber { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        
        public bool IsValid => !string.IsNullOrEmpty(PoNumber) && 
                              !string.IsNullOrEmpty(ReceiverNumber) && 
                              !string.IsNullOrEmpty(Date);
    }

    public class PdfProcessor
    {
        public PdfExtractedData ExtractDataFromPdf(string pdfFilePath)
        {
            var data = new PdfExtractedData();
            
            try
            {
                using (var reader = new PdfReader(pdfFilePath))
                using (var pdfDoc = new PdfDocument(reader))
                {
                    // Extract text from first page only
                    var page = pdfDoc.GetPage(1);
                    var strategy = new SimpleTextExtractionStrategy();
                    var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                    
                    // Extract data using regex patterns
                    data.PoNumber = ExtractPoNumber(text);
                    data.ReceiverNumber = ExtractReceiverNumber(text);
                    data.Date = ExtractAndFormatDate(text);
                }
            }
            catch (Exception ex)
            {
                // Log error - for now just return invalid data
                System.Diagnostics.Debug.WriteLine($"Error processing PDF {pdfFilePath}: {ex.Message}");
            }
            
            return data;
        }

        private string ExtractPoNumber(string text)
        {
            // Look for PO number patterns like "PO-064483" or "PO 064483"
            var patterns = new[]
            {
                @"PO[-\s]*(\d{6})",           // PO-064483 or PO 064483
                @"P\.?O\.?[-\s]*(\d{6})",     // P.O.-064483 or P.O. 064483
                @"Purchase\s+Order[-\s]*(\d{6})", // Purchase Order 064483
            };
            
            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return "PO-" + match.Groups[1].Value;
                }
            }
            
            return string.Empty;
        }

        private string ExtractReceiverNumber(string text)
        {
            // Look for receiver number patterns
            var patterns = new[]
            {
                @"Receiver[-\s]*:?\s*(\d{6})",      // Receiver: 139736
                @"Receiver\s+Number[-\s]*:?\s*(\d{6})", // Receiver Number: 139736
                @"Rcv[-\s]*:?\s*(\d{6})",           // Rcv: 139736
                @"Receipt[-\s]*:?\s*(\d{6})",       // Receipt: 139736
            };
            
            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            
            return string.Empty;
        }

        private string ExtractAndFormatDate(string text)
        {
            // Look for date patterns and format as MM/dd/yyyy
            var patterns = new[]
            {
                @"(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})",     // MM/dd/yyyy or M/d/yyyy
                @"(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{2})",      // MM/dd/yy or M/d/yy
                @"Date[-\s]*:?\s*(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})", // Date: MM/dd/yyyy
                @"(\d{4})[\/\-](\d{1,2})[\/\-](\d{1,2})",      // yyyy/MM/dd
            };
            
            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern);
                if (match.Success)
                {
                    try
                    {
                        int month, day, year;
                        
                        if (match.Groups.Count == 4) // MM/dd/yyyy format
                        {
                            month = int.Parse(match.Groups[1].Value);
                            day = int.Parse(match.Groups[2].Value);
                            year = int.Parse(match.Groups[3].Value);
                            
                            // Handle 2-digit years
                            if (year < 100)
                            {
                                year += (year < 50) ? 2000 : 1900;
                            }
                        }
                        else if (pattern.Contains("yyyy") && pattern.IndexOf("yyyy") < pattern.IndexOf("MM")) // yyyy/MM/dd format
                        {
                            year = int.Parse(match.Groups[1].Value);
                            month = int.Parse(match.Groups[2].Value);
                            day = int.Parse(match.Groups[3].Value);
                        }
                        else
                        {
                            continue;
                        }
                        
                        // Validate date components
                        if (month >= 1 && month <= 12 && day >= 1 && day <= 31 && year >= 1900 && year <= 2100)
                        {
                            return $"{month:D2}/{day:D2}/{year}";
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            return string.Empty;
        }
    }
}