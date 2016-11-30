/* 
 * Created by Faycel MRABET
*/ 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DecryptAnagram
{
    class Program
    {
        const string ANAGRAM = "poultry outwits ants";
        const string TARGET_HASH = "4624d200580677270a54ccff86b9610e";
        const string RAINBOW_TABLE_FILENAME = "Rainbow.txt";

        static void Main(string[] args)
        {
            Console.Title = "Decrypt MD5";
            Console.WriteLine(">>> Decrypt Anagram <<<");
            var words = File.ReadAllLines("wordlist");
            if(!File.Exists(RAINBOW_TABLE_FILENAME))
            {
                Console.WriteLine("Cannot find rainbow-table-file: {0}", RAINBOW_TABLE_FILENAME);
                Console.WriteLine("To decrypt the hash: {0} for the anagram: {1} you need to generate a rainbow-table-file: {2}.", TARGET_HASH, ANAGRAM, RAINBOW_TABLE_FILENAME);
                Console.WriteLine("This process will take about 1-2 hours and about 3GB of disk! ");
                Console.WriteLine("Do you want to start generateing this file (Y=Yes, N=No)?");
                var returnKey = Console.ReadKey();
                if(returnKey.KeyChar.ToString().ToUpper() == "Y")
                {
                    Console.Clear();
                    CreateRainbowTable(words);
                }
            }
            else
                FindTheMagicWords();
        }

        private static void FindTheMagicWords()
        {
            var start = DateTime.Now;
            using (var sr = new System.IO.StreamReader(RAINBOW_TABLE_FILENAME))
            {
                var line = String.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.CursorLeft = 0;
                    var item = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    Console.Write("{0} - Working, please wait... {1}", DateTime.Now - start, item[0]);

                    if (item[0].Trim() == TARGET_HASH) 
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine();
                        Console.WriteLine("Found the magic words: {0}", item[1]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.ReadKey();
                        return;
                    }
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sorry - cannot find the magic words :(");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadKey();
        }

        private static void CreateRainbowTable(string[] words)
        {
            var anagramWords = ANAGRAM.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var anagramChars = new String(ANAGRAM.Replace(" ", String.Empty).Distinct().ToArray());
            
            var aCandidate = new Func<string, int, bool>((word, len) =>
            {
                var count = 0;
                foreach (var c in word.ToCharArray())
                {
                    if (anagramChars.Contains(c)) count++;
                    if (count == len) return true;
                }
                return (count >= len);
            });
            
            var getCandidates = new Func<int, List<string>>(target => {
                return (from word in words
                        where word.Length == anagramWords[target].Length && aCandidate(word, anagramWords[target].Length)
                        select word).ToList();
            });
            
            var MD5 = new Func<string, string, string, string>((string word1, string word2, string word3) => System.BitConverter.ToString(
                System.Security.Cryptography.MD5.Create()
                    .ComputeHash(Encoding.UTF7.GetBytes(String.Format("{0} {1} {2}", word1, word2, word3))))
                .Replace("-", System.String.Empty)
                .ToLower());

            var candidates1 = getCandidates(0);
            var candidates2 = getCandidates(1);
            var candidates3 = getCandidates(2);

            var start = DateTime.Now;
            using (var fs = File.CreateText(RAINBOW_TABLE_FILENAME))
            {
                fs.AutoFlush = true;
                for (var i1 = 0; i1 <= candidates1.Count() - 1; i1++)
                    for (var i2 = 0; i2 <= candidates2.Count() - 1; i2++)
                        for (var i3 = 0; i3 <= candidates3.Count() - 1; i3++)
                        {
                            if (candidates1[i1] == candidates2[i2]) continue;
                            if (candidates1[i1] == candidates3[i3]) continue;
                            if (candidates2[i2] == candidates3[i3]) continue;

                            Console.CursorLeft = 0;
                            var target = String.Format("{0} {1} {2}", candidates1[i1], candidates2[i2], candidates3[i3]);
                            var hash = MD5(candidates1[i1], candidates2[i2], candidates3[i3]);
                            Console.Write("{0}: [{1}/{2}]:{3} - [{4}/{5}]:{6} - [{7}/{8}]:{9}", DateTime.Now - start, i1, candidates1.Count(), candidates1[i1], i2, candidates2.Count(), candidates2[i2], i3, candidates3.Count(), candidates3[i3]);
                            fs.WriteLine(String.Format("{0}|{1}", hash, target));
                        }
            }
        }
    }
}
