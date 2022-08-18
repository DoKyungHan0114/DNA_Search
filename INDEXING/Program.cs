using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// This program is for indexing sequences 

namespace Index
{
    class Program
    {

        //*************** Only these two lines can be modified**********************//
        public static string file_location = @"C:\Users\n10074881\source\repos\INDEXING\INDEXING\bin\Debug\16S.fasta"; // location of fasta file
        public static string save_location = @"C:\Users\n10074881\Desktop\16s.indexquery.txt"; // location for saving result
        //*************** Only these two lines can be modified**********************//


        public static List<int> pos = new List<int>(); // an array to keep ine positions in
        public static List<int> size = new List<int>(); // an array to keep ine size in
        public static bool stop_program = false; // bool variable to keep program run after type one command        
        public static int counter = 0; // Counter line numbers

        public static void TextReader()
        {

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
        }
        public static void SeekIndex()
        {
            int i = 0;
            string text1;
            string text2;
            string textfile;
            List<string> result = new List<string>(); // Create list file for saving result data


            using (FileStream fs = new FileStream(file_location, FileMode.Open, FileAccess.Read))// the "using" construct ensures that the FileStream is properly closed/disposed   
            {
                byte[] bytes;

                while (i < counter)
                {
                    bytes = new byte[size[i]];

                    fs.Seek(pos[i], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                    fs.Read(bytes, 0, size[i]); // get the data off disk - now there is disk access

                    text1 = Encoding.UTF8.GetString(bytes).Substring(0, 12); //Two string for textfile
                    text2 = pos[i].ToString();

                    string space_bar = " "; //Space for put sequence offset for each id
                    textfile = text1 + space_bar + text2;

                    result.Add(textfile);

                    i = i + 2; // Plus by two for only print sequence id
                }

                string[] resulttext = result.ToArray(); //Convert the list to array to save as text file

                File.WriteAllLines(save_location, resulttext);

            }



        }

        static void Main(string[] args)
        {


            TextReader();
            SeekIndex();
            Console.WriteLine("IndexSequence16s 16S.fasta 16S.index");
            Console.WriteLine();
            Console.WriteLine("The indexed file created!!");
            Console.WriteLine();
            Console.WriteLine("Press enter to close the program...");

            // Suspend the screen.  
            Console.ReadLine();
        }
    }


}