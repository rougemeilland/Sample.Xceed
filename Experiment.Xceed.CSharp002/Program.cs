using System.IO;
using System.Text;
using Xceed.Compression;
using Xceed.FileSystem;
using Xceed.Zip;

namespace Experiment.Xceed.CSharp002
{
    internal class Program
    {
        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // Create a test ZIP file. Specify the path name of the ZIP file in the first command argument.
        private static void Main(string[] args)
        {
            var localEncoding = Encoding.GetEncoding("shift_jis");

            ZipArchive.DefaultExtraHeaders = ExtraHeaders.None;
            ZipArchive.OEMEncodingOverride = localEncoding;

            var zipArchiveFile = new DiskFile(args[0]);
            var zipArchive = new ZipArchive(zipArchiveFile)
            {
                DefaultCompressionMethod = CompressionMethod.Deflated,
                DefaultCompressionLevel = CompressionLevel.Highest,
                DefaultTextEncoding = TextEncoding.Unicode, // Set bit 11 of the general purpose flag.
                DefaultUnicodeUsagePolicy = UnicodeUsagePolicy.Always, // Set bit 11 of the general purpose flag even if the entry name and comment are only in the ASCII character set.
            };
            zipArchive.Comment = $"This is comment for zip archive file '{zipArchive.HostFile.Name}'.";

            zipArchive.BeginUpdate();
            try
            {
                {
                    var file = (ZippedFile)zipArchive.CreateFile($"file_0x554e.txt", true);
                    file.ExtraHeaders = ExtraHeaders.Unicode; // Add extra field 0x554e to this entry.
                    file.Comment = $"This is comment for file '{file.FullName}'.";
                    using var outputStream = file.OpenWrite(true);
                    using var writer = new StreamWriter(outputStream);
                    writer.WriteLine($"Hello, this file is '{file.FullName}'.");
                    writer.WriteLine($"こんにちは、このファイルは '{file.FullName}' です。");
                }

                {
                    var file = (ZippedFile)zipArchive.CreateFile($"file_0x5455.txt", true);
                    file.ExtraHeaders = ExtraHeaders.ExtendedTimeStamp; // Add extra field 0x5455 to this entry.
                    file.Comment = $"This is comment for file '{file.FullName}'.";
                    using var outputStream = file.OpenWrite(true);
                    using var writer = new StreamWriter(outputStream);
                    writer.WriteLine($"Hello, this file is '{file.FullName}'.");
                    writer.WriteLine($"こんにちは、このファイルは '{file.FullName}' です。");
                }

                {
                    var file = (ZippedFile)zipArchive.CreateFile($"file_0x000a.txt", true);
                    file.ExtraHeaders = ExtraHeaders.FileTimes; // Add extra field 0x000a to this entry.
                    file.Comment = $"This is comment for file '{file.FullName}'.";
                    using var outputStream = file.OpenWrite(true);
                    using var writer = new StreamWriter(outputStream);
                    writer.WriteLine($"Hello, this file is '{file.FullName}'.");
                    writer.WriteLine($"こんにちは、このファイルは '{file.FullName}' です。");
                }
            }
            finally
            {
                zipArchive.EndUpdate();
            }
        }
    }
}