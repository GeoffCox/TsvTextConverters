using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToTsv
{
    class Program
    {
        static void Run(string inputDir, string outputFile)
        {
            if (!Directory.Exists(inputDir))
            {
                throw new Exception($"Input directory {inputDir} does not exist.");
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputFile)))
            {
                throw new Exception($"The directory for the output file {outputFile} does not exist.");
            }

            using (var textWriter = File.CreateText(outputFile))
            {
                textWriter.WriteLine("Title\tText");
                foreach (var inputFile in Directory.EnumerateFiles(inputDir, "*.txt*"))
                {
                    var file = Path.GetFileNameWithoutExtension(inputFile);
                    var text = File.ReadAllText(inputFile);
                    text = text.Replace("\t", "    ");
                    text = text.Replace("\r\n", "\\r\\n");
                    text = text.Replace("\r", "\\r\\n");
                    text = text.Replace("\n", "\\r\\n");
                    text = text.Replace("\"", "\\\"");
                    textWriter.WriteLine($"{file}\t{text}");
                    Console.Write(".");
                }                
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    Run(args[0], args[1]);
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
                Console.WriteLine("Usage: TextToTsv <input directory> <output file>");
            }



            if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }
        }
    }
}
