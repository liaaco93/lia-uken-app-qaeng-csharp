Finds the integer with the least repetitions in a list of integers.

Usage:      [command] RepetitionCounter.csx [arguments]

Command:
    interpreter         command to run whichever C# script interpreter is preferred
                        tested using dotnet-script

Arguments:
    filepath            path to integer list for counting

Outputs are appended to output.txt in the same folder as the script; output.txt is created if it does not already exist. In the case of ties, the smallest integer is returned.

Example:
    dotnet-script RepetitionCounter.csx src/*.txt
    This uses dotnet-script to run RepetitionCounter.csx on every .txt file in the folder src.