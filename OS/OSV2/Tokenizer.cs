using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace OSV2
{
    /// <summary>
    /// The Tokenizer class is responsible for parsing and processing user commands.
    /// It splits input into tokens and calls corresponding methods based on the parsed command.
    /// </summary>
    internal class Tokenizer
    {
        /// <summary>
        /// A variable to store values from the command line input.
        /// </summary>
        public static List<string> Values;

        /// <summary>
        /// Parses user input into a Token structure, splitting it into individual command parts.
        /// Handles quoted strings as single tokens.
        /// </summary>
        /// <param name="Commands">The input string entered by the user.</param>
        public static void parseInput(string Commands)//split user input in struct Token
        {
            Values = new List<string>();//create new list in object

            string Command = "";
            bool insideQuotes = false;

            for (int Counter = 0; Counter < Commands.Length; Counter++)
            {
                char currentChar = Commands[Counter];

                if (currentChar == '\"') // Toggle quotes mode
                {
                    insideQuotes = !insideQuotes;
                }
                else if (currentChar == ' ' && !insideQuotes) // Split on spaces when outside quotes
                {
                    if (!string.IsNullOrWhiteSpace(Command))
                    {
                        Values.Add(Command);
                        Command = "";
                    }
                }
                else
                {
                    Command += currentChar;
                }
            }

            if (!string.IsNullOrWhiteSpace(Command)) // Add the last command if not empty
            {
                Values.Add(Command);
            }

            parseOutput(Values);//call Method
        }

        /// <summary>
        /// Processes the parsed token by calling the corresponding command handler method.
        /// </summary>
        /// <param name="Values">The parsed Values List containing the command and its arguments.</param>
        public static void parseOutput(List<string> Values)//Call Method with first Command Line
        {

            switch (Values.ToArray()[0])
            {
                //Basic Function 

                case "cd"://call Change Directory function 
                    {
                        if (Values.Count == 1 || Values.Count == 2)
                            Parser.cd(Values.Count == 1 ? "" : Values[1], Program.currentDirectory);
                        else
                        {
                            Console.WriteLine($"Error : << cd >> Syntax Error");
                            Parser.help("cd");
                        }
                        break;
                    }

                case "dir": //call Show Content Directory method
                    {
                        if (Values.ToArray().Length > 1)
                            for (int count = 1; count < Values.Count; count++)
                                Parser.dir(Values[count]);
                        else
                            Parser.dir(false);
                        break;
                    }

                case "type"://call Show Content Files function
                    {
                        if (Values.Count() > 1)
                            for (int Count = 1; Count < Values.Count(); Count++)
                                Parser.type(Values[Count]);
                        else
                        {
                            Console.WriteLine($"Error : << type >> Syntax Error");
                            Parser.help("type");
                        }
                        break;
                    }

                //Depend basic function

                case "md"://call Make Directory function
                    {
                        if (Values.Count() == 2)
                            Parser.md(Values[1], true);

                        //bonus//Function
                        else if (Values.Count() == 3 && Values[1] == "-m")
                            Parser.md_m(Values[2]);

                        else
                        {
                            Console.WriteLine($"Error : << md >> Syntax Error");
                            Parser.help("md");
                        }
                        break;
                    }

                case "rd"://call Remove Directory function
                    {
                        //bonus//Function
                        if (Values.Count() > 2 && Values[1] == "-a")
                            for (int Count = 2; Count < Values.Count(); Count++)
                                Parser.rd_a(Values[Count]);

                        else if (Values.Count() > 1)
                            for (int Count = 1; Count < Values.Count(); Count++)
                                Parser.rd(Values[Count], true);

                        else
                        {
                            Console.WriteLine($"Error : << rd >> Syntax Error");
                            Parser.help("rd");

                        }
                        break;
                    }

                case "del"://call Delete Files function
                    {
                        if (Values.Count() > 1)
                            for (int Count = 1; Count < Values.Count(); Count++)
                                Parser.del(Values.ToArray()[Count], true);
                        else
                        {
                            Console.WriteLine($"Error : << del >> Syntax Error");
                            Parser.help("del");
                        }
                        break;

                    }

                case "rename"://call Rename Directory or Files function
                    {
                        if (Values.Count == 3)
                            Parser.rename(Values.ToArray()[1], Values.ToArray()[2]);

                        else
                        {
                            Console.WriteLine($"Error : << rename >> Syntax Error");
                            Parser.help("rename");
                        }
                        break;
                    }

                //Hard Function

                case "copy"://call Copy Files function
                    {
                        if (Values.Count == 2 ||  Values.Count == 3)
                            Parser.copy(Values.ToArray()[1], Values.Count == 2 ? "" : Values.ToArray()[2], true);
                        else
                        {
                            Console.WriteLine($"Error : << copy >> Syntax Error");
                            Parser.help("copy");
                        }
                        break;
                    }

                case "import"://call Import Files function
                    {
                        if (Values.Count == 2 || Values.Count == 3)
                            Parser.import(Values.ToArray()[1], Values.Count == 2 ? "" : Values.ToArray()[2], true);
                        else
                        {
                            Console.WriteLine($"Error : << import >> Syntax Error");
                            Parser.help("import");
                        }
                        break;
                    }

                case "export"://call Export Files function
                    {
                        if (Values.Count == 2 || Values.Count == 3)
                            Parser.export(Values.ToArray()[1], Values.Count == 2 ? "" : Values.ToArray()[2], true);
                        else
                        {
                            Console.WriteLine($"Error : << export >> Syntax Error");
                            Parser.help("export");
                        }
                        break;
                    }

                //Easy Function

                case "cls"://call Clear Screen function
                    {
                        if(Values.Count == 1)
                            Parser.cls();
                        else
                        {
                            Console.WriteLine($"Error : << cls >> Syntax Error");
                            Parser.help("cls");
                        }
                        break;
                    }

                case "help"://call Help function
                    {
                        if (Values.Count() == 1 || Values.Count() == 2)
                            Parser.help(Values.Count() == 1 ? "" : Values.ToArray()[1]);

                        else
                        {
                            Console.WriteLine($"Error : << cls >> Syntax Error");
                            Parser.help("cls");
                        }
                        break;
                    }

                case "quit"://call Quit function
                    {
                        if(Values.Count == 1)
                            Parser.quit();
                        else
                        {
                            Console.WriteLine($"Error : << quit >> Syntax Error");
                            Parser.help("quit");
                        }
                        break;
                    }

                default://command error
                    {
                        Console.WriteLine($"<< {Values.ToArray()[0]} >> Command Error");
                        break;
                    }
            }
        }
    }
}
// Summary of the Tokenizer class
/*
 * The Tokenizer class parses and processes user commands.
 * Key Features:
 * - Splits commands into tokens, respecting quotes for multi-word arguments.
 * - Supports a range of commands: cd, dir, type, md, rd, del, rename, copy, import, export, cls, help, quit.
 * - Validates arguments and provides error handling for syntax issues.
 * - Uses a structured Token to encapsulate commands and their arguments.
 */