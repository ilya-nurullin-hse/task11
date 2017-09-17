using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task11
{
    class Program
    {
        static readonly Random rand = new Random();

        const int keySize = 10;

        static void Main(string[] args)
        {
            var key = GenerateKey();

            string text = RandomString(keySize * keySize);
            Console.WriteLine("Randrom text: {0}", text);
            Console.WriteLine("Encoded text: {0}", Encode(key, text));
            Console.WriteLine("Decoded text: {0}", Decode(key, Encode(key, text)));
            Console.WriteLine("Decoded text =? Random text: {0}", Decode(key, Encode(key, text)) == text);
        }

        static int[,] GenerateKey()
        {
            int[,] key = new int[keySize, keySize];

            for (int i = 0; i < keySize; i++)
            {
                for (int j = 0; j < keySize; j++)
                {
                    if (key[i, j] == 1 || key[i, j] == -1)
                        continue;

                    int[][] coords = { new []{ i, j }, new[] { j, keySize - 1 - i}, new[] { keySize - 1 - j, i},
                        new[] { keySize - 1 - i, keySize - 1 - j}
                    };

                    int r = rand.Next(0, 3);
                    key[coords[r][0], coords[r][1]] = 1;
                    for (int k = 0; k < coords.Length; k++)
                    {
                        if (k == r)
                            continue;

                        key[coords[k][0], coords[k][1]] = -1;
                    }
                }
            }
            return key;
        }

        static string Encode(int[,] key, string text)
        {
            char[,] res = new char[keySize,keySize];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < keySize; j++)
                {
                    for (int k = 0; k < keySize; k++)
                    {
                        if (key[j, k] == 1)
                        {
                            res[j, k] = text.First();
                            text = text.Remove(0, 1);
                        }
                    }
                }
                key = RotateKey(key);
            }

            string result = "";
            for (int i = 0; i < keySize; i++)
            {
                for (int j = 0; j < keySize; j++)
                {
                    result += res[i, j];
                }
            }

            return result;
        }

        static string Decode(int[,] key, string text)
        {
            char[,] res = new char[keySize, keySize];

            for (int i = 0; i < keySize; i++)
            {
                for (int j = 0; j < keySize; j++)
                {
                    res[i, j] = text.First();
                    text = text.Remove(0, 1);
                }
            }

            string result = "";

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < keySize; j++)
                {
                    for (int k = 0; k < keySize; k++)
                    {
                        if (key[j, k] == 1)
                        {
                            result += res[j, k];
                        }
                    }
                }
                key = RotateKey(key);
            }
            return result;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        static int[,] RotateKey(int[,] key)
        {
            int[,] res = new int[keySize,keySize];
            for (int i = 0; i < keySize; i++)
            {
                for (int j = 0; j < keySize; j++)
                {
                    res[i, j] = key[keySize - 1 - j, i];
                }
            }
            return res;
        }
    }
}
