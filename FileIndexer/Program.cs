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
            var dbPath = "testDb.json";
            var path = "C:\\Users\\yehuda nahon\\Documents\\books";
            var db = GetDB(dbPath); 
            
            // check the path is a valid path
            if(!IsDirectory(path))
            {
                Console.WriteLine("path is not a directory");
            }

            // get all the files in the directory
            var files = Directory.GetFiles(path);

            Console.WriteLine("indexing files...");
            db.IndexFiles(files);
            
            Console.WriteLine("updating data base");
            try
            {
                UpdateDB(dbPath, db);
            }
            catch(Exception e)
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
