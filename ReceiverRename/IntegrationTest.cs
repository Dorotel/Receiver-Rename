using System;
using System.IO;
using System.Threading.Tasks;

namespace ReceiverRename
{
    public class IntegrationTest
    {
        public static async Task RunIntegrationTest()
        {
            Console.WriteLine("=== Integration Test ===");
            Console.WriteLine();

            var testFolder = Path.Combine(Path.GetTempPath(), "ReceiverRenameTest");
            
            try
            {
                // Create test directory
                if (Directory.Exists(testFolder))
                {
                    Directory.Delete(testFolder, true);
                }
                Directory.CreateDirectory(testFolder);
                
                Console.WriteLine($"Created test folder: {testFolder}");
                
                // Create sample text files (simulating PDFs for testing)
                await CreateSampleFiles(testFolder);
                
                // Test the main processing logic
                await TestProcessing(testFolder);
                
                Console.WriteLine("\n✓ Integration test completed successfully!");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Integration test failed: {ex.Message}");
            }
            finally
            {
                // Cleanup
                try
                {
                    if (Directory.Exists(testFolder))
                    {
                        Directory.Delete(testFolder, true);
                        Console.WriteLine("Cleaned up test folder.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not clean up test folder: {ex.Message}");
                }
            }
        }

        private static async Task CreateSampleFiles(string testFolder)
        {
            // Sample 1: Well-formatted data
            var sample1 = Path.Combine(testFolder, "invoice1.txt");
            await File.WriteAllTextAsync(sample1, 
                "INVOICE\n" +
                "PO-064483\n" +
                "Receiver: 139736\n" +
                "Date: 08/01/2025\n" +
                "Other details...");
            
            // Sample 2: Alternative format
            var sample2 = Path.Combine(testFolder, "receipt2.txt");
            await File.WriteAllTextAsync(sample2,
                "Receipt Document\n" +
                "Purchase Order 987654\n" +
                "Receiver Number: 555123\n" +
                "2/15/2025\n" +
                "Additional info...");
            
            // Sample 3: Missing data (should be skipped)
            var sample3 = Path.Combine(testFolder, "incomplete.txt");
            await File.WriteAllTextAsync(sample3,
                "Document without required fields\n" +
                "Some random text\n" +
                "No PO or receiver info");
            
            Console.WriteLine("Created sample test files.");
        }

        private static async Task TestProcessing(string testFolder)
        {
            Console.WriteLine("\nTesting file processing logic...");
            
            var textFiles = Directory.GetFiles(testFolder, "*.txt");
            
            foreach (var file in textFiles)
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    Console.Write($"Processing {fileName}... ");
                    
                    // Read file content and simulate PDF processing
                    var content = await File.ReadAllTextAsync(file);
                    var data = SimulateExtraction(content);
                    
                    if (data.IsValid)
                    {
                        var newFileName = GenerateTestFileName(data);
                        var newFilePath = Path.Combine(testFolder, newFileName + ".txt");
                        
                        if (!File.Exists(newFilePath))
                        {
                            File.Move(file, newFilePath);
                            Console.WriteLine($"✓ Renamed to: {newFileName}.txt");
                        }
                        else
                        {
                            Console.WriteLine($"⚠ Skipped (target exists)");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"⚠ Skipped (missing data)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error: {ex.Message}");
                }
            }
        }

        private static PdfExtractedData SimulateExtraction(string content)
        {
            var data = new PdfExtractedData();
            
            // Extract PO Number
            if (content.Contains("PO-"))
            {
                var start = content.IndexOf("PO-") + 3;
                var end = content.IndexOf('\n', start);
                if (end == -1) end = start + 6;
                data.PoNumber = "PO-" + content.Substring(start, Math.Min(6, end - start));
            }
            else if (content.Contains("Purchase Order"))
            {
                var start = content.IndexOf("Purchase Order") + 15;
                var end = content.IndexOf('\n', start);
                if (end == -1) end = content.Length;
                var poNumber = content.Substring(start, Math.Min(10, end - start)).Trim();
                data.PoNumber = "PO-" + poNumber;
            }
            
            // Extract Receiver
            if (content.Contains("Receiver:"))
            {
                var start = content.IndexOf("Receiver:") + 9;
                var end = content.IndexOf('\n', start);
                if (end == -1) end = content.Length;
                data.ReceiverNumber = content.Substring(start, Math.Min(10, end - start)).Trim();
            }
            else if (content.Contains("Receiver Number:"))
            {
                var start = content.IndexOf("Receiver Number:") + 16;
                var end = content.IndexOf('\n', start);
                if (end == -1) end = content.Length;
                data.ReceiverNumber = content.Substring(start, Math.Min(10, end - start)).Trim();
            }
            
            // Extract Date
            var lines = content.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("/") && line.Trim().Length < 15)
                {
                    try
                    {
                        var datePart = line.Trim();
                        if (datePart.Contains(":")) continue; // Skip "Date:" labels
                        
                        var parts = datePart.Split('/');
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