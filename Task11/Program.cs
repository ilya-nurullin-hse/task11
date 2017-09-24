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
            Console.WriteLine("Шифровка текста с помощью решетки заключается в следующем. Решетка, т.е. квадрат 10х10 клеток, " +
                              "некоторые клетки в котором вырезаны, совмещается с целым квадратом 10х10 клеток и через прорези " +
                              "на бумагу наносятся первые буквы текста. Затем решетка поворачивается на 90 и через прорези " +
                              "записываются следующие буквы. Это повторяется еще дважды. Таким образом, на бумагу наносится 100 " +
                              "букв текста. Решетку можно изображать квадратной матрицей порядка 10 из нулей и единиц. Требуется " +
                              "зашифровать данную последовательность из 100 букв, затем ее расшифровать. Даны поледовательность из " +
                              "100 букв и матрица - ключ.Зашифровать данную последовательность.Расшифровать данную поледовательность.");
            while (true)
            {
                Console.WriteLine("1. Сгенерировать матрицу-ключ автоматически");
                Console.WriteLine("2. Задать матрицу-ключ вручную (матрица 10x10)");
                var answer = readInt("Выберите вариант", x => x == 1 || x == 2, "Ожидалось 1 или 2");
                int[,] key = new int[keySize, keySize];

                if (answer == 1)
                {
                    key = GenerateKey();
                    for (int i = 0; i < keySize; i++)
                    {
                        for (int j = 0; j < keySize; j++)
                        {
                            Console.Write("{0} ", key[i, j] == -1 ? 0 : 1);
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Вводите матрицу построчно. Элементы в строке разделяйте пробелом. " +
                                      "Допустимые значения элементов: 0 и 1. Количество элементов в каждой строчке = {0}",
                        keySize);
                    for (int i = 0; i < keySize; i++)
                    {
                        try
                        {
                            var row = Console.ReadLine().Split().Select(int.Parse).Select(x => x == 0 ? -1 : x)
                                .ToArray();
                            if (row.Length != keySize)
                            {
                                Console.WriteLine(
                                    "Ожидалось элементов строчке: {0}, получено = {1}. Попробуйе ещё раз.",
                                    keySize, row.Length);
                                i--;
                                continue;
                            }
                            if (row.Any(x => x != -1 && x != 1))
                            {
                                Console.WriteLine(
                                    "В строке обнаружены недопустимые элементы. Разрешены только 0 и 1. Попробуйе ещё раз.");
                                i--;
                                continue;
                            }
                            for (int j = 0; j < keySize; j++)
                            {
                                key[i, j] = row[j];
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Неправильный формат строки. Попробуйе ещё раз.");
                        }
                    }
                    if (!CheckKey(key))
                    {
                        Console.WriteLine("Невалидная матрица-ключ. Попробуйте ещё раз.");
                        continue;
                    }
                }

                Console.WriteLine("1. Сгенерировать текст автоматически");
                Console.WriteLine("2. Задать текст вручную (ровно 100 символов)");
                answer = readInt("Выберите вариант", x => x == 1 || x == 2, "Ожидалось 1 или 2");

                string text = "";

                if (answer == 1)
                    text = RandomString(keySize * keySize);
                else
                {
                    int length = 0;
                    do
                    {
                        Console.Write("Введите текст: ");
                        text = Console.ReadLine();
                        length = text.Length;
                        if (length != 100)
                        {
                            Console.WriteLine("Текст должен быть длины 100 символов. Попробуйте ещё раз.");
                        }
                    } while (length != 100);
                }

                Console.WriteLine("Randrom text: {0}", text);
                Console.WriteLine("Encoded text: {0}", Encode(key, text));
                Console.WriteLine("Decoded text: {0}", Decode(key, Encode(key, text)));
                Console.WriteLine("Decoded text =? Random text: {0}", Decode(key, Encode(key, text)) == text);
            }
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

                    int[][] coords =
                    {
                        new[] {i, j}, new[] {j, keySize - 1 - i}, new[] {keySize - 1 - j, i},
                        new[] {keySize - 1 - i, keySize - 1 - j}
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
            char[,] res = new char[keySize, keySize];
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
            int[,] res = new int[keySize, keySize];
            for (int i = 0; i < keySize; i++)
            {
                for (int j = 0; j < keySize; j++)
                {
                    res[i, j] = key[keySize - 1 - j, i];
                }
            }
            return res;
        }

        static bool CheckKey(int[,] key)
        {
            for (int i = 0; i < keySize; i++)
            {
                for (int j = 0; j < keySize; j++)
                {
                    if (key[i, j] == -1
                          && ! (key[j, keySize - 1 - i] == 1
                              || key[keySize - 1 - j, i] == 1
                              || key[keySize - 1 - i, keySize - 1 - j] == 1)
                    )
                        return false;
                    if (key[i, j] == 1 
                    && key[j, keySize - 1 - i] != -1 
                    && key[keySize - 1 - j, i] != -1 
                    && key[keySize - 1 - i, keySize - 1 - j] != -1
                    )
                        return false;
                }
            }
            return true;
        }

        static int readInt(string msg, Func<int, bool> filter, string errFilter)
        {
            int n = 0;
            bool ok = true;

            do
            {
                try
                {
                    Console.Write("{0}: ", msg);
                    n = Int32.Parse(Console.ReadLine());
                    if (!filter(n))
                    {
                        Console.WriteLine(errFilter);
                        ok = false;
                    }
                    else
                        ok = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Введите число");
                    ok = false;
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Вы ввели слишком большое число");
                    ok = false;
                }
            } while (!ok);
            return n;
        }
    }
}