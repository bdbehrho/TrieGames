using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace TrieGames
{
    class Node
    {
        public Node[] children;
        public char letter;
        public string word;
        public Node()
        {
            children = new Node[26];
        }

        public Node(char character)
        {
            children = new Node[26];
            this.letter = character;
        }

        public Node findChild(char character)
        {
            int index = character - 'a';
            if (index >= 0 && index < 26)
            {
                return children[index];
            }
            else
            {
                return null;
            }
        }

        public List<string> WordsBeneath()
        {
            List<string> words = new List<string>();

            WordsBeneath(words);

            return words;
        }

        private void WordsBeneath(List<string> currentWords)
        {
            if (word != null)
            {
                currentWords.Add(word);
            }
            foreach (Node child in children)
            {
                if (child != null)
                {
                    child.WordsBeneath(currentWords);
                }
            }
        }
    }

    class Trie
    {
        Node root;
        public Trie(IEnumerable<string> words)
        {
            root = new Node();
            Node currentNode = root;
            foreach (string word in words)
            {
                if (word == null) break;
                currentNode = root;
                for (int i = 0; i < word.Length; i++)
                {
                    Node next = currentNode.findChild(word[i]);
                    if (next != null)
                    {
                        currentNode = next;
                    }
                    else
                    {
                        currentNode.children[word[i] - 'a'] = new Node(word[i]);
                        currentNode = currentNode.findChild(word[i]);
                    }
                }
                currentNode.word = word;
            }

        }

        #region Boggle

        int[] movements = new int[] { -1, 0, 1 };

        public IEnumerable<string> SearchBoggleMap(List<string> boggleMap)
        {
            List<string> result = new List<string>();

            int length = boggleMap.Count;

            foreach (string line in boggleMap)
            {
                if (line.Length != length)
                {
                    throw new ArgumentException("Boggle map not square");
                }
            }

            bool[,] used = new bool[length, length];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    used[i, j] = false;
                }
            }

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    StartSearch(boggleMap, i, j, root, used, result);
                }
            }

            return result;
        }

        private void StartSearch(List<string> map, int index1, int index2, Node current, bool[,] used, List<string> result)
        {
            Node next = current.findChild(map[index1][index2]);
            if (next == null)
            {
                return;
            }
            else if (next.word != null)
            {
                if (!result.Contains(next.word))
                {
                    result.Add(next.word);
                }
            }

            used[index1, index2] = true;
            foreach (int move1 in movements)
            {
                foreach (int move2 in movements)
                {
                    int new1 = index1 + move1;
                    int new2 = index2 + move2;
                    if (new1 >= 0 && new1 < map.Count &&
                        new2 >= 0 && new2 < map.Count &&
                        !used[new1, new2])
                    {
                        StartSearch(map, new1, new2, next, used, result);
                    }
                }
            }

            used[index1, index2] = false;
        }

        #endregion

        #region Jumble

        public string UnJumble(string jumbled)
        {
            return UnJumble(jumbled, root);
        }

        private string UnJumble(string letters, Node current)
        {
            if (current == null)
            {
                return null;
            }
            else if (current.word != null && letters.Length == 0)
            {
                return "" + current.letter;
            }
            for (int i = 0; i < letters.Length; i++)
            {
                Node next = current.findChild(letters[i]);
                string before = UnJumble(letters.Substring(0, i) + letters.Substring(i + 1, letters.Length - i - 1), next);
                if (before != null)
                {
                    return current.letter + before;
                }
            }
            return null;
        }

        #endregion

        public Node findNode(string start)
        {
            Node current = root;
            for (int i = 0; i < start.Length; i++)
            {
                current = current.findChild(start[i]);
                if (current == null)
                {
                    break;
                }
            }
            return current;
        }
    }

    class TrieGames
    {
        public static Trie trie;

        static void randomize(List<string> list)
        {
            Random rnd = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int r1 = rnd.Next(0, list.Count);
                int r2 = rnd.Next(0, list.Count);
                string temp = list[r1];
                list[r1] = list[r2];
                list[r2] = temp;
            }
        }

        static void PromptJumble()
        {
            string line;
            List<string> map = new List<string>();
            Console.Write("\nAllowed lengths: ");
            string allowed = Console.ReadLine();
            Console.WriteLine();

            List<int> allowedLengths = new List<int>();

            if (!allowed.Equals("all"))
            {
                IEnumerable<string> values = allowed.Split();
                foreach (string value in values)
                {
                    allowedLengths.Add(Convert.ToInt32(value));
                }
            }

            while ((line = Console.ReadLine()) != "")
            {
                map.Add(line);
            }

            IEnumerable<string> result = trie.SearchBoggleMap(map);

            result = result.OrderBy(s => s.Length);
            Console.WriteLine();
            foreach (string value in result)
            {
                if (allowed.Equals("all") || allowedLengths.Contains(value.Length))
                {
                    Console.WriteLine(value);
                }
            }
        }

        static void Main(string[] args)
        {

            string[] words = new string[1000];

            Random rand = new Random();
            string fileName = "..\\..\\words.txt";
            int maxIndex = 0;

            using (StreamReader sr = new StreamReader(fileName))
            {
                var regex = new Regex("^[a-z]+$");
                while (sr.Peek() >= 0)
                {
                    string word = sr.ReadLine();
                    if (regex.IsMatch(word))
                    {
                        words[maxIndex] = word.ToLower();
                        maxIndex++;
                        if (maxIndex == words.Length - 1)
                        {
                            Array.Resize(ref words, words.Length * 2);
                        }
                    }
                }
            }

            trie = new Trie(words);

            string command;
            while ((command = Console.ReadLine()) != "done")
            {
                string[] commands = command.Split();
                if (commands[0] == "unjumble")
                {
                    Console.WriteLine(trie.UnJumble(commands[1]));
                }
                else if (commands[0] == "boggle")
                {
                    PromptJumble();
                }
            }
        }
    }
}
