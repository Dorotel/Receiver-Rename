using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ReceiverRename
{
    internal class Program
    {
        private static string scansFolder = @"H:\scans";
        private static string fileNameFormat = "{DATE} {PO} {RECEIVER}";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== PDF Receiver Rename Tool ===");
            Console.WriteLine();

            // Check for test mode
            if (args.Length > 0 && args[0].ToLower() == "--test")
            {
                TestPdfProcessor.RunTests();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            // Handle command line arguments
            if (args.Length > 0 && args[0].ToLower() != "--test")
            {
                scansFolder = args[0];
            }
            if (args.Length > 1)
            {
                fileNameFormat = args[1];
            }

            // Interactive mode if no arguments provided
            if (args.Length == 0)
            {
                ShowSettings();
                ConfigureSettings();
            }

            await ProcessPdfFiles();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void ShowSettings()
        {
            Console.WriteLine($"Current Settings:");
            Console.WriteLine($"  Scans Folder: {scansFolder}");
            Console.WriteLine($"  File Format: {fileNameFormat}");
            Console.WriteLine();
        }

        private static void ConfigureSettings()
        {
            Console.Write("Enter scans folder path (or press Enter to use current): ");
            var newScansFolder = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(newScansFolder))
            {
                scansFolder = newScansFolder;
            }

            Console.Write("Enter file format (or press Enter to use current): ");
            var newFileFormat = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(newFileFormat))
            {
                fileNameFormat = newFileFormat;
            }

            Console.WriteLine();
        }

        private static async Task ProcessPdfFiles()
        {
            try
            {
                if (!Directory.Exists(scansFolder))
                {
                    Console.WriteLine($"Error: Scans folder does not exist: {scansFolder}");
                    return;
                }

                var pdfFiles = Directory.GetFiles(scansFolder, "*.pdf", SearchOption.TopDirectoryOnly);

                if (pdfFiles.Length == 0)
                {
                    Console.WriteLine("No PDF files found in the scans folder.");
                    return;
                }

                Console.WriteLine($"Found {pdfFiles.Length} PDF files to process...");
                Console.WriteLine();

                int processedCount = 0;
                int renamedCount = 0;
                int skippedCount = 0;

                foreach (var pdfFile in pdfFiles)
                {
                    try
                    {
                        var fileName = Path.GetFileName(pdfFile);
                        Console.Write($"Processing: {fileName} ... ");

                        var pdfProcessor = new PdfProcessor();
                        var extractedData = await Task.Run(() => pdfProcessor.ExtractDataFromPdf(pdfFile));

                        if (extractedData != null && extractedData.IsValid)
                        {
                            var newFileName = GenerateNewFileName(extractedData);
                            var newFilePath = Path.Combine(scansFolder, newFileName + ".pdf");

                            if (!File.Exists(newFilePath))
                            {
                                File.Move(pdfFile, newFilePath);
                                Console.WriteLine($"✓ Renamed to: {newFileName}.pdf");
                                renamedCount++;
                            }
                            else
                            {
                                Console.WriteLine($"⚠ Skipped (target file already exists)");
                                skippedCount++;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"⚠ Skipped (could not extract required data)");
                            skippedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Error: {ex.Message}");
                        skippedCount++;
                    }

                    processedCount++;
                }

                Console.WriteLine();
                Console.WriteLine($"Processing complete: {renamedCount} renamed, {skippedCount} skipped");

                // Open the scans folder when complete (Windows only)
                if (OperatingSystem.IsWindows())
                {
                    try
                    {
                        Process.Start("explorer.exe", scansFolder);
                        Console.WriteLine("Opened scans folder in Explorer.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not open folder: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static string GenerateNewFileName(PdfExtractedData data)
        {
            var fileName = fileNameFormat;
            fileName = fileName.Replace("{DATE}", data.Date);
            fileName = fileName.Replace("{PO}", data.PoNumber);
            fileName = fileName.Replace("{RECEIVER}", data.ReceiverNumber);

            // Remove invalid file name characters
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            return fileName;
        }
    }
}
