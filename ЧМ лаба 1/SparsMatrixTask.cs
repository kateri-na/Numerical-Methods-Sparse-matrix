using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Task1_NM
{
    public class SparseMatrixTask
    {
        protected int size;

        protected int ColumnNum; //number of column k

        protected double[]? a; //lower diagonal

        protected double[]? b; //secondary diagonal

        protected double[]? c; //upper diagonal

        protected double[]? p;

        protected double[]? f;

        protected double[]? _f;

        protected double[]? result;

        protected double[]? _result;

        protected double[]? exactlyResult;


        private void Calculate_f()
        {
            for (int i = 0; i < size; ++i)
            {
                if (i == size - ColumnNum - 1 || i == size - ColumnNum || i == size - ColumnNum + 1)
                    _f[i] = c[i] + b[i] + a[i];
                else
                    _f[i] = c[i] + b[i] + a[i] + p[i];
            }
        }

        private void CalculateFforRandom()
        {
            if (ColumnNum == size - 1)
                f[0] = p[0] * exactlyResult[ColumnNum - 1] + b[0] * exactlyResult[size - 1];
            else
                f[0] = p[0] * exactlyResult[ColumnNum - 1] + c[0] * exactlyResult[size - 2] + b[0] * exactlyResult[size - 1];

            for (int i = 1; i < size-1; ++i)
            {
                if (i == size - ColumnNum - 1 || i == size - ColumnNum || i == size - ColumnNum + 1)
                    f[i] = c[i] * exactlyResult[size - i - 2] + b[i] * exactlyResult[size - i - 1] + a[i] * exactlyResult[size - i];
                else
                    f[i] = c[i] * exactlyResult[size - i - 2] + b[i] * exactlyResult[size - i - 1] + a[i] * exactlyResult[size - i] + p[i] * exactlyResult[ColumnNum - 1];
            }

            if (ColumnNum == 2)
                f[size - 1] = p[size - 1] * exactlyResult[ColumnNum - 1] + b[size - 1] * exactlyResult[0];
            else
                f[size - 1] = p[size - 1] * exactlyResult[ColumnNum - 1] + b[size - 1] * exactlyResult[0] + a[size-1]* exactlyResult[1];
        }

        public SparseMatrixTask(String fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            string? readSize = sr.ReadLine();
            size = Convert.ToInt32(readSize);
            string? readColumnNum = sr.ReadLine();
            ColumnNum = Convert.ToInt32(readColumnNum);
            a = new double[size];
            b = new double[size];
            c = new double[size];
            p = new double[size];
            f = new double[size];
            _f = new double[size];
            result = new double[size];
            _result = new double[size];
            exactlyResult = null;
            ReadFromFile(sr);
            Calculate_f();
        }
        public SparseMatrixTask(int size, int minValue, int maxValue)
        {
            Random rnd = new Random();
            this.size = size;
            ColumnNum = rnd.Next(2, size-1);
            a = new double[size];
            b = new double[size];
            c = new double[size];
            p = new double[size];
            f = new double[size];
            _f = new double[size];
            result = new double[size];
            _result = new double[size];
            exactlyResult = new double[size];
            double d = maxValue - minValue;
            for (int i = 0; i < size; ++i)
            {
                exactlyResult[i] = minValue + rnd.NextDouble() * d;

                if (i == 0)
                    a[i] = 0;
                else
                    a[i] = minValue + rnd.NextDouble() * d;

                b[i] = minValue + rnd.NextDouble() * d;

                if (i == size - 1)
                    c[i] = 0;
                else
                    c[i] = minValue + rnd.NextDouble() * d;

                if (i == size - ColumnNum)
                    p[i] = b[i];
                else if (i == size - ColumnNum - 1)
                    p[i] = c[i];
                else if (i == size - ColumnNum + 1)
                    p[i] = a[i];
                else
                    p[i] = minValue + rnd.NextDouble() * d;
            }

            CalculateFforRandom();
            Calculate_f();
        }
        private void ReadArrayFromFile(StreamReader sr, double[] arr)
        {
            string line;
            if ((line = sr.ReadLine()) != null)
            {
                string[] text = line.Split(' ');
                if (!arr.Equals(a))
                    for (int i = 0; i < text.Length; ++i)
                        arr[i] = Convert.ToDouble(text[i]);
                else
                    for (int i = 0; i < text.Length; ++i)
                        arr[i + 1] = Convert.ToDouble(text[i]);
            }
            else
                Console.WriteLine("Can`t read array from file");
        }

        public void ReadFromFile(StreamReader sr)
        {
            ReadArrayFromFile(sr, c);
            c[size - 1] = 0;
            ReadArrayFromFile(sr, b);
            a[0] = 0;
            ReadArrayFromFile(sr, a);
            ReadArrayFromFile(sr, p);
            ReadArrayFromFile(sr, f);
        }

        protected void DivideLine(int rowNum, double element)
        {
            b[rowNum] /= element;
            p[rowNum] /= element;
            f[rowNum] /= element;
            _f[rowNum] /= element;

            if (rowNum > 0)
                a[rowNum] /= element;

            if (rowNum < size - 1)
                c[rowNum] /= element;
        }

        protected void ResetElemOnPrevUpperLine(int rowNum)
        {
            double coeff = -c[rowNum - 1];
            p[rowNum - 1] += p[rowNum] * coeff;
            f[rowNum - 1] += f[rowNum] * coeff;
            _f[rowNum - 1] += _f[rowNum] * coeff;
            c[rowNum - 1] += b[rowNum] * coeff;
            b[rowNum - 1] += a[rowNum] * coeff;


            if (rowNum == size - ColumnNum + 2)
                a[rowNum - 1] = p[rowNum - 1];
        }
        protected void ResetElemOnPrevLowerLine(int rowNum)
        {
            double coeff = -a[rowNum + 1];

            b[rowNum + 1] += c[rowNum] * coeff;
            a[rowNum + 1] += b[rowNum] * coeff;
            p[rowNum + 1] += p[rowNum] * coeff;
            f[rowNum + 1] += f[rowNum] * coeff;
            _f[rowNum + 1] += _f[rowNum] * coeff;

            if (rowNum == size - ColumnNum - 2)
                c[rowNum + 1] = p[rowNum + 1];

            else if (rowNum == size - ColumnNum - 1)
                b[rowNum + 1] = p[rowNum + 1];
        }
        protected void FirstStep()
        {
            for (int rowNum = 0; rowNum < size - ColumnNum; ++rowNum)
            {
                if (b[rowNum] == 0)
                    continue;

                DivideLine(rowNum, b[rowNum]);

                if (a[rowNum + 1] == 0)
                    continue;

                ResetElemOnPrevLowerLine(rowNum);
            }
        }
        protected void SecondStep()
        {
            for (var rowNum = size - 1; rowNum >= size - ColumnNum + 1; --rowNum)
            {
                if (b[rowNum] == 0)
                    continue;

                DivideLine(rowNum, b[rowNum]);

                if (c[rowNum - 1] == 0)
                    continue;

                ResetElemOnPrevUpperLine(rowNum);
            }
        }
        protected void ThirdStep()
        {
            f[size - ColumnNum] /= b[size - ColumnNum];
            _f[size - ColumnNum] /= b[size - ColumnNum];
            p[size - ColumnNum] /= b[size - ColumnNum];
            b[size - ColumnNum] = 1;
        }
        protected void ResetElemOnColumnUp(int rowNum)
        {
            for (int ind = rowNum; ind > 0; --ind)
            {
                double coeff = -p[ind - 1];

                p[ind - 1] += p[rowNum] * coeff;

                f[ind - 1] += f[rowNum] * coeff;

                _f[ind - 1] += _f[rowNum] * coeff;

                if (ind == size - ColumnNum)
                    c[size - ColumnNum - 1] = p[size - ColumnNum - 1];
            }
        }

        protected void ResetElemOnColumnDown(int rowNum)
        {
            for (var ind = rowNum; ind < size - 1; ++ind)
            {
                double coeff = -p[ind + 1];

                p[ind + 1] += p[rowNum] * coeff;

                f[ind + 1] += f[rowNum] * coeff;

                _f[ind + 1] += _f[rowNum] * coeff;

                if (ind == size - ColumnNum)
                    a[size - ColumnNum + 1] = p[size - ColumnNum + 1];

            }

        }
        protected void FourthStep()
        {
            ResetElemOnColumnUp(size - ColumnNum);
            for (int rowNum = size - ColumnNum; rowNum > 0; --rowNum)
                ResetElemOnPrevUpperLine(rowNum);

        }
        protected void FifthStep()
        {
            ResetElemOnColumnDown(size - ColumnNum);
            for (var rowNum = size - ColumnNum; rowNum < size - 1; ++rowNum)
                ResetElemOnPrevLowerLine(rowNum);
        }
        protected void Calculate()
        {
            for (var rowNum = 0; rowNum < size; ++rowNum)
            {
                result[rowNum] = f[size - rowNum - 1];
                _result[rowNum] = _f[size - rowNum - 1];
            }
        }

        protected double Accuracy()
        {
            double max = 0.0;
            double current;
            for (int i = 0; i < size; ++i)
            {
                current = Math.Abs(_result[i] - 1.0);
                if (current > max)
                    max = current;
            }
            return max;
        }
        protected double RelativeError()
        {
            double max = 0.0;
            double current;
            double q = 1e-5;
            for (int index = 0; index < size; ++index)
            {
                if (Math.Abs(exactlyResult[index]) > q)
                    current = Math.Abs((result[index] - exactlyResult[index]) / exactlyResult[index]);
                else
                    current = Math.Abs(result[index] - exactlyResult[index]);
                if (current > max)
                    max = current;
            }
            return max;
        }
        public (double, double) FindAccuracies()
        {
            return (Accuracy(), RelativeError());
        }
        public void SolutionFromFile()
        {
            FirstStep();
            //Console.WriteLine("After the first step:");
            //PrintArrays();
            SecondStep();
            //Console.WriteLine("After the second step:");
            //PrintArrays();
            ThirdStep();
            //Console.WriteLine("After the third step:");
            //PrintArrays();
            FourthStep();
            //Console.WriteLine("After the fourth step:");
            //PrintArrays();
            FifthStep();
            //Console.WriteLine("After the fifth step:");
            //PrintArrays();
            Calculate();

            Console.WriteLine("Accuracy: " + Accuracy());
            Console.WriteLine("Result:");
            PrintArray(result);
            WriteInFileAnswer();
        }
        public void SolutionForRandom()
        {
            FirstStep();
            SecondStep();
            ThirdStep();
            FourthStep();
            FifthStep();
            Calculate();
        }
        public void WriteInFileAnswer()
        {
            StreamWriter writeFile = new StreamWriter("result.txt");
            writeFile.WriteLine("Accuracy: " + Accuracy());
            for (int i = 0; i < size; ++i)
                writeFile.WriteLine(result[i]);
            writeFile.Close();
        }
        private void PrintArray(double[] arr)
        {
            foreach (var item in arr) Console.Write("{0,7} ", string.Format("{0:F1}", item));
            Console.WriteLine();
        }
        public void PrintArrays()
        {
            Console.WriteLine("Column Number:" + ColumnNum);
            Console.Write("c: ");
            PrintArray(c);
            Console.Write("b: ");
            PrintArray(b);
            Console.Write("a: ");
            PrintArray(a);
            Console.Write("p: ");
            PrintArray(p);
            Console.Write("f: ");
            PrintArray(f);
        }
    }
}

