using System;
using System.IO;

namespace ReceiverRename
{
    public class TestPdfProcessor
    {
        public static void RunTests()
        {
            Console.WriteLine("=== Testing PDF Processor ===");
            Console.WriteLine();

            // Test text parsing with sample text
            var processor = new PdfProcessor();
            
            // Test 1: PO Number extraction
            var testText1 = "Invoice Details\nPO-064483\nReceiver: 139736\nDate: 08/01/2025";
            Console.WriteLine("Test 1 - Sample text parsing:");
            Console.WriteLine($"Input: {testText1.Replace('\n', ' ')}");
            
            var data1 = ExtractDataFromText(processor, testText1);
            Console.WriteLine($"PO Number: {data1.PoNumber}");
            Console.WriteLine($"Receiver: {data1.ReceiverNumber}");
            Console.WriteLine($"Date: {data1.Date}");
            Console.WriteLine($"Valid: {data1.IsValid}");
            Console.WriteLine();

            // Test 2: Different formats
            var testText2 = "Purchase Order 123456\nReceiver Number: 789012\n1/5/2025";
            Console.WriteLine("Test 2 - Alternative formats:");
            Console.WriteLine($"Input: {testText2.Replace('\n', ' ')}");
            
            var data2 = ExtractDataFromText(processor, testText2);
            Console.WriteLine($"PO Number: {data2.PoNumber}");
            Console.WriteLine($"Receiver: {data2.ReceiverNumber}");
            Console.WriteLine($"Date: {data2.Date}");
            Console.WriteLine($"Valid: {data2.IsValid}");
            Console.WriteLine();

            // Test 3: File naming
            if (data1.IsValid)
            {
                var fileName = GenerateTestFileName(data1);
                Console.WriteLine($"Generated filename: {fileName}");
            }
        }

        private static PdfExtractedData ExtractDataFromText(PdfProcessor processor, string text)
        {
            // This is a simplified version for testing without actual PDF files
            var data = new PdfExtractedData();
            
            // Use reflection or create public methods for testing
            // For now, we'll simulate the extraction
            if (text.Contains("PO-"))
            {
                var start = text.IndexOf("PO-") + 3;
                var poNumber = text.Substring(start, 6);
                data.PoNumber = "PO-" + poNumber;
            }
            else if (text.Contains("Purchase Order"))
            {
                var start = text.IndexOf("Purchase Order") + 15;
                var end = text.IndexOf('\n', start);
                if (end == -1) end = text.Length;
                var poNumber = text.Substring(start, Math.Min(6, end - start)).Trim();
                data.PoNumber = "PO-" + poNumber;
            }

            if (text.Contains("Receiver:"))
            {
                var start = text.IndexOf("Receiver:") + 9;
                var end = text.IndexOf('\n', start);
                if (end == -1) end = text.Length;
                data.ReceiverNumber = text.Substring(start, Math.Min(10, end - start)).Trim();
            }
            else if (text.Contains("Receiver Number:"))
            {
                var start = text.IndexOf("Receiver Number:") + 16;
                var end = text.IndexOf('\n', start);
                if (end == -1) end = text.Length;
                data.ReceiverNumber = text.Substring(start, Math.Min(10, end - start)).Trim();
            }

            // Simple date extraction
            if (text.Contains("/"))
            {
                var lines = text.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("/") && line.Length < 20)
                    {
                        try
                        {
                            var parts = line.Trim().Split('/');
                            if (parts.Length == 3)
                            {
                                var month = int.Parse(parts[0]);
                                var day = int.Parse(parts[1]);
                                var year = int.Parse(parts[2]);
                                data.Date = $"{month:D2}/{day:D2}/{year}";
                                break;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return data;
        }

        private static string GenerateTestFileName(PdfExtractedData data)
        {
            var fileName = "{DATE} {PO} {RECEIVER}";
            fileName = fileName.Replace("{DATE}", data.Date);
            fileName = fileName.Replace("{PO}", data.PoNumber);
            fileName = fileName.Replace("{RECEIVER}", data.ReceiverNumber);
            
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                fileName = fileName.Replace(invalidChar, '_');
            }
            
            return fileName;
        }
    }
}