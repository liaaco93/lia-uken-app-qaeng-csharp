//#!/usr/bin/env csi
using System.IO;
using System.Collections;

public enum ErrorCode : ushort
{
    None = 0,
    Generic = 1,
    FileDoesNotExist = 100,
    IOException = 200,
    UnparseableLine = 300,
    IntegerOverflow = 900
}

public ErrorCode CountFile(string filePath, out Dictionary<int, uint> fileCounts)
{
    fileCounts = new Dictionary<int, uint>();

    if (System.IO.File.Exists(filePath))
    {
        try
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                string line;
                while((line = file.ReadLine()) != null)
                {
                    int lineValue;
                    //Error Checking: Checks that line is an integer
                    if (!int.TryParse(line, out lineValue))
                    {
                        Debug.WriteLine(String.Format("Error reading {0}: Cannot parse line \"{1}\" as integer", filePath, line));
                        return ErrorCode.UnparseableLine;
                    }

                    if (fileCounts.ContainsKey(lineValue))
                    {
                        //Error Checking: Checks that count doesn't exceed max unsigned int value
                        if (fileCounts[lineValue] == uint.MaxValue){
                            Debug.WriteLine(String.Format("Error reading {0}: integer overflow on key {1}", filePath, lineValue));
                            return ErrorCode.IntegerOverflow;
                        }
                        fileCounts[lineValue]++;
                    }
                    else
                    {
                        fileCounts.Add(lineValue, 1);
                    }
                }
            }
            return 0;
        }
        catch(IOException e)
        {
            Debug.WriteLine(String.Format("Error reading {0}: {1}", filePath, e.Message));
            return ErrorCode.IOException;
        }
    }
    else
    {
        Debug.WriteLine("Error reading {0}: File does not exist");
        return ErrorCode.FileDoesNotExist;
    }
}

public KeyValuePair<int, uint> FindLeastRepeated(Dictionary<int, uint> repetitionCounts)
{
    KeyValuePair<int, uint> leastRepeated = new KeyValuePair<int, uint>(0,0);

    Debug.Assert(repetitionCounts.Count > 0);
    foreach(KeyValuePair<int, uint> kvp in repetitionCounts)
    {
        if (leastRepeated.Value == 0
            || kvp.Value < leastRepeated.Value
            || (kvp.Value == leastRepeated.Value && kvp.Key < leastRepeated.Key))
            {
                leastRepeated = kvp;
            }
    }
    return leastRepeated;
}

if (Args.Count > 0)
{
    string[] outputLines = new string[Args.Count];
    for (int i = 0; i < Args.Count; i++)
    {
        Dictionary<int, uint> fileCount = new Dictionary<int, uint>();

        ErrorCode fileCountReturn = CountFile(Args[i], out fileCount);
        if (fileCountReturn == 0)
        {
            KeyValuePair<int, uint> leastRepeated = FindLeastRepeated(fileCount);
            Debug.Assert(leastRepeated.Value > 0);
            outputLines[i] = String.Format("File: {0}, Number: {1}, Repeated: {2}",
                                            Args[i],
                                            leastRepeated.Key,
                                            leastRepeated.Value);
        }
        else
        {
            string errorMessage;

            switch (fileCountReturn)
            {
                case ErrorCode.FileDoesNotExist:
                    errorMessage = "File does not exist";
                    break;

                case ErrorCode.IOException:
                    errorMessage = "IOException raised while trying to read file";
                    break;

                case ErrorCode.UnparseableLine:
                    errorMessage = "Could not parse all lines in file as int";
                    break;
                
                case ErrorCode.IntegerOverflow:
                    errorMessage = "Count exceeded max value for unsigned int";
                    break;

                default:
                    errorMessage = "Unknown error";
                    break;
            }

            outputLines[i] = String.Format("Error while counting {0}: {1}", Args[i], errorMessage);
        }
    }

    string outputPath = "output.txt";
    try
    {
        using (StreamWriter outputFile = new StreamWriter(outputPath, true))
        {
            foreach (string result in outputLines)
            {
                outputFile.WriteLine(result);
            }
            Console.WriteLine(String.Format("Output written to {0}", outputPath));
        }
    }
    catch (IOException e)
    {
        Console.WriteLine(String.Format("Error writing to {0}: {1}", e.Message));
    }
}
else
{
    Console.WriteLine(String.Format("Usage:    dotnet-script RepetitionCounter.csx [arguments]\n\nArguments:\n{0,-32}{1}",
                                     "  file",
                                     "Path to text file with newline-delimited list of integers"));
}