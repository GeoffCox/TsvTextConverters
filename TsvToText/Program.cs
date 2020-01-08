using GenericParsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsvToText
{
    class Program
    {
        static void Run(string inputFile, string outputDir, bool hasHeaders, int? contentColumn, int? fileNameColumn)
        {
            if (!File.Exists(inputFile))
            {
                throw new Exception($"Input file {inputFile} does not exist.");
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputDir)))
            {
                throw new Exception($"The directory for the output {outputDir} does not exist.");
            }

            using (GenericParser parser = new GenericParser(inputFile))
            {
                parser.ColumnDelimiter = '\t';
                parser.MaxBufferSize = 4096 * 10;
                var itemIndex = 0;
                var firstItemIndex = hasHeaders ? 1 : 0;
                int fileNameColumnIndex = fileNameColumn  ?? - 1;
                int contentColumnIndex = contentColumn ?? 0;
                string fileName;
                string content;
                while (parser.Read())
                {
                    if (itemIndex >= firstItemIndex)
                    {
                        content = parser[contentColumnIndex];
                        content = content.Replace("\\r", "\r");
                        content = content.Replace("\\n", "\n");
                        content = content.Replace("\\\"", "\"");
                        fileName = fileNameColumnIndex >= 0 ? $"{parser[fileNameColumnIndex]}.txt" : $"{itemIndex}.txt";
                        string outputFile = Path.Combine(outputDir, fileName);
                        File.WriteAllText(outputFile, content);
                        Console.Write(".");
                    }

                    itemIndex++;
                }
                Console.WriteLine("");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length >= 3)
            {
                try
                {
                    var hasHeaders = args.Any(a => a == "/H" || a == "/h");
                    var valueArgs = args.Where(a => !(a == "/H" || a == "/h")).ToArray();
                    var tsvFile = valueArgs[0];
                    var outputDirectory = valueArgs[1];
                    var contentColumn = valueArgs.Length >= 3 ? (int?)int.Parse(valueArgs[2]) : null;
                    var fileNameColumn = valueArgs.Length >= 4 ? (int?)int.Parse(valueArgs[3]) : null;

                    Run(tsvFile, outputDirectory, hasHeaders, contentColumn, fileNameColumn);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was an error converting.");
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                Console.WriteLine("------------------------------------");
                Console.WriteLine("TextToTsv");
                Console.WriteLine("------------------------------------");
                Console.WriteLine("Converts a TSV file to a set of text files (.txt)");
                Console.WriteLine("TextToStv tsvFile outputDirectory [/H] [contentColumn] [fileNameColumn]");
                Console.WriteLine();
                Console.WriteLine("tsvFile          The path to the TSV file.");
                Console.WriteLine("outputDirectory  The path to the directory where files should be created");
                Console.WriteLine("/H               If the the first row of the TSV files is headers");
                Console.WriteLine("contentColumn    The index of the column containing the file content");
                Console.WriteLine("fileNameColumn   The index of the column containing the file name (without extension) to use.");
            }

            if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }
        }
    }
}
