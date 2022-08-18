using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;



namespace DirecFileAccessDemo
{


    public class Program
    {

        public static List<int> pos = new List<int>(); // an array to keep ine positions in
        public static List<int> size = new List<int>(); // an array to keep ine size in
        public static bool stop_program = false; // bool variable to keep program run after type one command


        //*****The four lines below for locations of files (Only this four lines can be modified) ********//

        public static string file_location = @"C:\Users\n10074881\source\repos\Assignment2\Assignment2\bin\Debug\16s.fasta";  //location for file to read
        public static string location_to_save_textfile = @"C:\Users\n10074881\Desktop\text.txt";  //Level1 , Location for saving textfile from Level1
        public static string location_of_query_textfile = @"C:\Users\n10074881\source\repos\Assignment1\Assignment1\bin\Debug\query.txt";  //Level3, Location of text file to read in Level3
        public static string location_of_resultfile = @"C:\Users\n10074881\Desktop\result.txt";  //Level3 , Location for saving result of Level3
        public static string location_of_indexfile = @"C:\Users\n10074881\Desktop\16s.indexquery.txt"; //Level4, Location of index file 


        //***********************************************************************************************//

        public static void TextReader()
        {
            int counter = 0; // Counter line numbers
            string line; // Variable for read file
            int position = 0; // file position of first line
            StreamReader file = new StreamReader(file_location);


            while ((line = file.ReadLine()) != null)
            {
                pos.Insert(counter, position); // store line position
                size.Insert(counter, line.Length + 1); // store line size
                counter++;
                position = position + line.Length + 1; // add 1 for '\n' character in file

            }


            file.Close();
        }


        public static void Level1(string line, string ouput) //Level1
        {
            int Line; //int variable to convert from string variable
            int.TryParse(line, out Line); // Convert string to int
            int Output; //int variable to convert from string variable
            int.TryParse(ouput, out Output);// Convert string to int

            string[] textfile = new string[Output]; // Declare array same length as output to save as text file

            int i = 0;

            if (Output > 0 && Line > 0)
            {
                using (FileStream fs = new FileStream(file_location, FileMode.Open, FileAccess.Read))
                {
                    // the "using" construct ensures that the FileStream is properly closed/disposed   

                    Line = Line - 1;
                    byte[] bytes;
                    byte[] bytes2;
                    while (i < Output)
                    {
                        bytes = new byte[size[Line + 2 * i]];
                        bytes2 = new byte[size[Line + 2 * i + 1]];

                        fs.Seek(pos[Line + 2 * i], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                        fs.Read(bytes, 0, size[Line + 2 * i]); // get the data off disk - now there is disk access

                        fs.Seek(pos[Line + 2 * i + 1], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                        fs.Read(bytes2, 0, size[Line + 2 * i + 1]); // get the data off disk - now there is disk access
                        Console.WriteLine(Encoding.Default.GetString(bytes)); // display the line    
                        Console.WriteLine(Encoding.Default.GetString(bytes2)); // display the line 

                        string text1 = System.Text.Encoding.UTF8.GetString(bytes); //Two string for textfile
                        string text2 = System.Text.Encoding.UTF8.GetString(bytes2);

                        textfile[i] = text1 + text2;

                        File.WriteAllLines(location_to_save_textfile, textfile); // Save in textfile
                        i++;
                    }

                }

            }
            if (Output < 0 || Line < 0) //if user put negative number, program does not run
            {
                Console.WriteLine("Error, only positive numbers are allowed for line numbers and sequences");
            }


        }
        public static string Level2(string sequence_name) //Level 2
        {
            int i = 0; //This is for find appropriate sequence id
            string result = ""; // Store nothing at first

            var file = file_location;
            var Target = sequence_name; //Target sequnce
            using (FileStream fs = new FileStream(file_location, FileMode.Open, FileAccess.Read))
            {
                foreach (string line in File.ReadLines(file))
                {
                    if (line.Contains(Target)) //Use contain to find sequence user want.
                    {
                        byte[] bytes = new byte[size[i]];
                        byte[] bytes2 = new byte[size[i + 1]];

                        fs.Seek(pos[i], SeekOrigin.Begin);

                        fs.Read(bytes, 0, size[i]);

                        fs.Seek(pos[i + 1], SeekOrigin.Begin);

                        fs.Read(bytes2, 0, size[i + 1]);



                        Console.WriteLine(Encoding.Default.GetString(bytes));
                        Console.WriteLine(Encoding.Default.GetString(bytes2));

                        string text1 = System.Text.Encoding.UTF8.GetString(bytes); //Two string for textfile
                        string text2 = System.Text.Encoding.UTF8.GetString(bytes2);

                        result = text1 + text2;

                    }

                    i++;
                }

            }
            if (result == "") //if program cannot find sequence, print error
            {
                Console.WriteLine("Error,sequence {0} not found", sequence_name);
            }


            return result;

        }
        public static void Level3()
        {
            List<string> query = new List<string>();
            List<string> result = new List<string>();
            int counter = 0;

            using (StreamReader query_file = new StreamReader(location_of_query_textfile))
            {

                string line_query;

                while ((line_query = query_file.ReadLine()) != null)
                {
                    query.Add(line_query);
                    counter++;

                }
                query_file.Close();

            }
            string[] Query = query.ToArray(); //Convert list to array



            for (int j = 0; j < counter; j++)
            {


                result.Add(Level2(Query[j])); //Using level2 to match the sequence id
            }
            string[] textfile = result.ToArray();
            File.WriteAllLines(location_of_resultfile, textfile); // Save to text file

        }

        public static void Level4()
        {
            int counter = 0; // Counter for read all lines in file
            int i = 0;
            int j = 0;

            List<string> file_offset = new List<string>(); //Create list to receive the file offset information
            List<string> number = new List<string>(); //Create list to store number (offset number)
            List<string> result_txt = new List<string>(); //Create list to store result data to save as text file

            string[] number2; //Create string array to convert from "number" list
            int[] number3; // Create int array to use file offset to seek appropriate DNA sequence 

            using (StreamReader index_file = new StreamReader(location_of_indexfile))
            {

                string line_query;
                string offset_number;

                while ((line_query = index_file.ReadLine()) != null)
                {
                    offset_number = line_query.Substring(13); // Only store file offset with Substring
                    file_offset.Add(offset_number);
                    counter++;

                }

                string[] position_number = file_offset.ToArray();
                while (j < counter)
                {
                    number.Add(position_number[j]);

                    j++;
                }
                number2 = number.ToArray(); // Convert recieved data
                number3 = Array.ConvertAll<string, int>(number2, int.Parse); //Convert to int

                byte[] bytes;
                byte[] bytes2;

                using (FileStream fs = new FileStream(file_location, FileMode.Open, FileAccess.Read)) // Same process as level 1 2 3
                {
                    while (i < counter)
                    {
                        int POS = number3[i]; // file offset is same as postion value 
                        int Size = 0; //For initialise Size of byte array
                        int Size2 = 0;
                       
                        for (int k = 0; k < counter; k++)
                        {
                            if (POS == pos[k]) //Seek position of DNA
                            {
                                Size = size[k];
                                Size2 = size[k + 1];
                            }
                        }
                        bytes = new byte[Size];
                        bytes2 = new byte[Size2];


                        fs.Seek(POS, SeekOrigin.Begin); //Use recieved data as position to seek id
                        fs.Read(bytes, 0, Size);
                        fs.Seek(POS + 1, SeekOrigin.Begin);
                        fs.Read(bytes2, 0, Size2);

                        Console.WriteLine(Encoding.UTF8.GetString(bytes));
                        Console.WriteLine(Encoding.UTF8.GetString(bytes2));

                        string text1 = System.Text.Encoding.UTF8.GetString(bytes); //Two string for textfile
                        string text2 = System.Text.Encoding.UTF8.GetString(bytes2);

                        result_txt.Add(text2); // Store at list

                        i++;
                    }

                }

                index_file.Close();
            }
            string[] final = result_txt.ToArray();

            File.WriteAllLines(location_of_resultfile, final); // Save to text file
        }
        public static void Level5(string query_string)
        {
            int i = 0; 
            string result = ""; // Store nothing at first

            var file = file_location;
         
            Regex regex = new Regex(query_string); //Create regex

            using (FileStream fs = new FileStream(file_location, FileMode.Open, FileAccess.Read))
            {
                foreach (string line in File.ReadLines(file))
                {
                    Match m = regex.Match(line);
                    if (m.Success) 
                    {
                        byte[] bytes = new byte[size[i - 1]]; // -1 for finding sequence id (previous postion is id)

                        fs.Seek(pos[i - 1], SeekOrigin.Begin);

                        fs.Read(bytes, 0, size[i - 1]);

                        string id = Encoding.Default.GetString(bytes);
                        Console.WriteLine(id.Substring(0, 13)); // Substring to only pring id


                        result = id;


                    }

                    i++;
                }

            }
            if (result == "") //if program cannot find sequence, print error
            {
                Console.WriteLine("Error,query string {0} not found", query_string);
            }


        }
        public static void Level6(string query_string)
        {
            int i = 0; //This is for find appropriate sequence id
            string result = ""; // Store nothing at first

            var file = file_location;
            var Target = query_string; //Target sequnce
            Regex regex = new Regex(query_string);

            using (FileStream fs = new FileStream(file_location, FileMode.Open, FileAccess.Read))
            {
                foreach (string line in File.ReadLines(file))
                {
                    Match m = regex.Match(line);
                    if (m.Success) //Use contain to find sequence user want.
                    {
                        byte[] bytes = new byte[size[i]];

                        fs.Seek(pos[i], SeekOrigin.Begin);

                        fs.Read(bytes, 0, size[i]);

                        string id = Encoding.Default.GetString(bytes);
                        Console.WriteLine(id.Substring(0, 13));


                        result = id;


                    }

                    i++;
                }

            }
            if (result == "") //if program cannot find sequence, print error
            {
                Console.WriteLine("Error,meta-data {0} not found", query_string);
            }

        }
        public static void Level7(string DNA)
        {


            List<string> filter = new List<string>();
            filter.Add(DNA);

            Int32 length = filter.Count;


            if (filter.Contains("*")) // Remove * for clear data
            {
                filter.Remove("*");
            }

            string id = string.Join(",", filter);

            Level5(id); // Using level5 with filtered data


        }
        public static List<string> CommandReader()
        {
            List<string> input = new List<string>();

            string command = Console.ReadLine();
            string[] tokens = command.Split(' '); //split the element of array every time user press spacebar

            foreach (string token in tokens)
            {
                input.Add(token);
            }
            return input; //return command input

        }

        static void Main(string[] args)
        {

            while (!stop_program)
            {
                Console.WriteLine("Type your query at line below :");

                List<string> input = CommandReader();
                TextReader();
                string output = " ";

                string program_name = input[0];
                string flag = input[1];
                string filename = input[2];
                string line = input[3]; //This can be line number, sequence id or text filename depends on levels.

                //******Error handlings******//
                if (program_name != "Search16s")
                {
                    Console.WriteLine("Error, please input right program name");
                    break;
                }

                if (flag != "-level1" && flag != "-level2" && flag != "-level3" && flag != "-level4" && flag != "-level5" && flag != "-level6" && flag != "-level7")
                {
                    Console.WriteLine("Error, type right flag!");
                }


                //*****Implement different function depends on user input*******
                if (flag == "-level1")
                {
                    output = input[4];
                    Level1(line, output);
                }
                if (flag == "-level2")
                {

                    Level2(line);
                }
                if (flag == "-level3")
                {
                    Level3();
                }
                if (flag == "-level4")
                {
                    output = input[4];
                    Level4();
                }
                if (flag == "-level5")
                {
                    Level5(input[3]);
                }
                if (flag == "-level6")
                {
                    Level6(input[3]);

                }
                if (flag == "-level7")
                {
                    Level7(input[3]);

                }




            }



            // Suspend the screen.  
            Console.ReadLine();
        }
    }
}