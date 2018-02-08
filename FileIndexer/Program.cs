using ClassLibrary1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] options = { "Index Folder", "Search Words" };
            var dbPath = "testDb.json";
            
            var db = GetDB(dbPath); 
            
            while(true)
            {
                var choise = PrintMenu(options);
                switch (choise)
                {
                    case 0:
                        IndexFiles(db, dbPath);
                        break;
                    case 1:
                        Search(db);
                        break;
                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            }
        }
        
        static int PrintMenu(string[] options)
        {
            Console.WriteLine("Menu:");
            int index = 0;
            foreach(var option in options)
            {
                Console.WriteLine(index.ToString() + ") " + option);
                index++;
            }

            string choise;
            do
            {
                choise = Console.ReadLine();
            } while (!int.TryParse(choise, out int number));
            
            return Convert.ToInt32(choise);
        }

        static void Search(SearchDB db)
        {
            Console.WriteLine("Enter the words to search");

            var text = Console.ReadLine();

            // split the text to words and make all words lowercase to fix abnormality (also removing all single letters words from the list)
            var punctuation = text.Where(Char.IsPunctuation).Distinct().ToArray();

            var words = text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim(punctuation).ToLower()).Where(x => x.Length > 1);

            foreach(var word in words)
            {
                var files = db.SearchWord(word);
                Console.WriteLine(word + ":");
                files.ForEach(file => Console.WriteLine("- " + file));
            }
        }

        static void IndexFiles(SearchDB db, string dbPath)
        {
            var storingFolder = "IndexedFiles";
            var folder = "C:\\Users\\yehuda nahon\\Documents\\books";
            // check the path is a valid path
            if (!IsDirectory(folder))
            {
                Console.WriteLine("path is not a directory");
            }

            if(!Directory.Exists(storingFolder))
            {
                Directory.CreateDirectory(storingFolder);
            }

            Console.WriteLine("indexing files...");

            var files = Directory.GetFiles(folder);
            db.IndexFiles(files, storingFolder);

            Console.WriteLine("updating data base");
            try
            {
                UpdateDB(dbPath, db);
            }
            catch (Exception e)
            {
                Console.WriteLine("failed updating db");
            }

            Console.WriteLine("finished writing to json file");
        }

        static void UpdateDB(string path, SearchDB dB)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, dB);
            }
        }

        static SearchDB GetDB(string path)
        {
            SearchDB db = new SearchDB();
            // check if theres a db to read
            if (File.Exists(path))
            {
                Console.WriteLine("reading from db");
                var dbData = File.ReadAllText(path);
                try
                {
                    db = JsonConvert.DeserializeObject<SearchDB>(dbData);
                }
                catch(Exception e)
                {
                    Console.WriteLine("invalid db reseting");
                    db = new SearchDB();
                    UpdateDB(path, db);
                }
                
            }
            else
            {
                // if the db doesnt exist create it for later updating
                Console.WriteLine("creating db");
                File.WriteAllText(path, JsonConvert.SerializeObject(db));
            }

            return db;
        }
        

        static bool IsDirectory(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }




    }
}
