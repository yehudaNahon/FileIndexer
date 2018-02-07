using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ClassLibrary1
{
    public class SearchDB
    {
        public SearchDB(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            storingPath = path;
            fileList = new List<string>();
            words = new Dictionary<string, List<int>>();
            blackList = new List<string>();
        }
        
        public void IndexFiles(string[] files)
        {
            int id = 0;
            var wordList = new List<WordInFile>();
            
            foreach (var file in files)
            {
                Console.WriteLine("indexing: " + file);

                if (fileList.Contains(file))
                {
                    Console.WriteLine(String.Format("file {0} is already indexed", file));
                    continue;
                }

                id = StoreFile(file);
                
                // add all the words found to the dictionary
                foreach (var word in GetWordsFromFile(file))
                {
                    if (!blackList.Contains(word))
                    {
                        wordList.Add(new WordInFile { word = word, file = id });
                    }
                }
            }

            // create a dictionary by word with a list of files that use that word
            var newWordsTable = wordList.GroupBy(t => t.word).ToDictionary(x => x.Key, t => t.Select(g => g.file).Distinct().ToList());

            // combine all the old words with the new
            words = words.Concat(newWordsTable).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private static List<string> GetWordsFromFile(string filePath)
        {
            // check file exists
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("%s is'nt a file path", filePath);
            }

            // read all the text from the file
            var text = File.ReadAllText(filePath);

            // split the text to words and make all words lowercase to fix abnormality (also removing all single letters words from the list)
            var punctuation = text.Where(Char.IsPunctuation).Distinct().ToArray();

            var words = text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim(punctuation).ToLower()).Where(x => x.Length > 1);

            return words.ToList();
        }

        private int StoreFile(string file)
        {
            var fileName = Path.GetFileName(file);
            var fullPath = Path.Combine(storingPath, fileName);
            
            File.Copy(file, fullPath);

            // un comment when you want to remove from src
            //File.Delete(file);

            fileList.Add(fileName);
            return fileList.Count();
        }

        public string storingPath;
        public List<string> fileList { get; set; }
        public  Dictionary<string, List<int>> words { get; set; }
        public List<string> blackList { get; set; }
    }

    public struct WordInFile
    {
        public string word;
        public int file;
    }
}
