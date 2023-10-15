using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1_NM
{
    class Programm
    {
        public static void Main(string[] args)
        {
            SparseMatrixTask matrixTask = new SparseMatrixTask(fileName: "data.txt");
            Console.WriteLine("Input data from moodle file:");
            matrixTask.PrintArrays();
            matrixTask.SolutionFromFile();

            const int countTests = 10;
            var testCases = new (int size, int minValue, int maxValue)[]
            {
                (10, -10, 10),
                (10, -100, 100),
                (10, -1000, 1000),
                (100, -10, 10),
                (100, -100, 100),
                (100, -1000, 1000),
                (1000, -10, 10),
                (1000, -100, 100),
                (1000, -1000, 1000)
            };

            Console.WriteLine();
            foreach (var (size, minValue, maxValue) in testCases)
            {
                double unitAccuracy = 0;
                double relativeError = 0;
                for (int count = 0; count < countTests; ++count)
                {
                    SparseMatrixTask matrix = new SparseMatrixTask(size, minValue, maxValue);
                    matrix.SolutionForRandom();
                    (double unit, double relative) = matrix.FindAccuracies();
                    unitAccuracy += unit;
                    relativeError += relative;
                }
                unitAccuracy /= countTests;
                relativeError /= countTests;
                Console.WriteLine();
                Console.WriteLine($"size: {size}; minValue: {minValue}; maxValue: {maxValue};");
                Console.WriteLine($"Средняя относительная погрешность системы: {relativeError}");
                Console.WriteLine($"Среднее значение оценки точности: {unitAccuracy}");

            }
            Console.ReadLine();
        }
    }
}

