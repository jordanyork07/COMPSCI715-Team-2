using System;
using System.Text;
using UnityEngine;

namespace Evaluation
{
    public abstract class LatinSquareGenerator
    {
        public static void Generate()
        {
            int width = 5;
            int height = 3;
            int[,] latinSquare = new int[height, width];
            GenerateLatinSquares(latinSquare, 0, 0);
        }

        public static void GenerateLatinSquares(int[,] latinSquare, int row, int col)
        {
            int height = latinSquare.GetLength(0);
            int width = latinSquare.GetLength(1);

            if (row == height)
            {
                // Print the Latin Square
                PrintLatinSquare(latinSquare);
            }
            else
            {
                for (int num = 1; num <= width; num++)
                {
                    if (IsSafe(latinSquare, row, col, num))
                    {
                        latinSquare[row, col] = num;

                        int nextRow = row;
                        int nextCol = col + 1;

                        if (nextCol == width)
                        {
                            nextRow++;
                            nextCol = 0;
                        }

                        GenerateLatinSquares(latinSquare, nextRow, nextCol);

                        latinSquare[row, col] = 0; // Backtrack
                    }
                }
            }
        }

        public static bool IsSafe(int[,] latinSquare, int row, int col, int num)
        {
            int height = latinSquare.GetLength(0);
            int width = latinSquare.GetLength(1);

            // Check if 'num' is already present in the current row or column
            for (int i = 0; i < width; i++)
            {
                if (latinSquare[row, i] == num || latinSquare[i, col] == num)
                {
                    return false;
                }
            }

            return true;
        }

        private static void PrintLatinSquare(int[,] latinSquare)
        {
            int height = latinSquare.GetLength(0);
            int width = latinSquare.GetLength(1);

            EvalKey key = new EvalKey { LatinSquare = new int[height, width] };
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    key.LatinSquare[i, j] = latinSquare[i, j];
                }
            }

            key.Print();
        }
    }
}