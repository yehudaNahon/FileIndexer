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
        public struct WordAccurance
        {
            public int count;
            public List<int> fileIds;
        }
        
        public struct WordInFile
        {
            public string word;
            public int file;
        }

        static void Main(string[] args)
        {
            var path = "C:\\Users\\yehud\\Documents\\books";

            // the word table linking a word to its count and files using it
            var wordTable = new Dictionary<string, List<int>>();

            // a list of file path's 
            var fileTable = new List<string>();

            // check the path is a valid path
            if(!IsDirectory(path))
            {
                Console.WriteLine("path is not a directory");
            }

            // get all the files in the directory
            var files = Directory.GetFiles(path);

            int id = 0;
            var wordList = new List<WordInFile>();
            foreach (var file in files)
            {
                if(fileTable.Contains(file))
                {
                    Console.WriteLine("file {0} is already indexed");
                    continue;
                }

                // get a list of words from the file
                var fileWords = GetWordsFromFile(file);

                // the id will be the next index in the list
                id = fileTable.Count();
                fileTable.Add(file);

                // add all the words found to the dictionary
                foreach (var word in fileWords)
                {
                    
                    wordList.Add(new WordInFile{word = word, file = id });
                }
            }

            //wordList.ForEach(iter => Console.WriteLine(String.Format("{0} => {1}", iter.file, iter.word)));

            // create a dictionary by word with a list of files that use that word
            var newWordsTable = wordList.GroupBy(t => t.word).ToDictionary(x => x.Key, t => t.Select(g => g.file).Distinct().ToList());
            
            foreach(var word in newWordsTable)
            {
                Console.Write(String.Format("{0} => ", word.Key));
                word.Value.ForEach(file => Console.Write(String.Format("{0} |", file)));
                Console.WriteLine("");
            }
            
        }
        
        static List<string> GetWordsFromFile(string filePath)
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
        
        static bool IsDirectory(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }


    }
}
