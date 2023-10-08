using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Evaluation
{
    public abstract class LatinSquareGenerator
    {
        private static readonly int[] Numbers = { 1, 2, 3, 4, 5 };
        
        public static void Generate()
        {
            var latinSquares = GenerateLatinSquares();

            var strBuild = new StringBuilder();
            foreach (var ls in latinSquares)
                strBuild.AppendLine(ls.Encode());
            
            Debug.Log(strBuild.ToString());
        }

        private static IEnumerable<EvalKey> GenerateLatinSquares()
        {
            for (int i = 0; i < 50; i++)
            {
                yield return GenerateLatinSquare(i);
            }
        }

        private static EvalKey GenerateLatinSquare(int seed)
        {
            Random rand = new Random(seed);
            int[,] square = new int[3, 5];
            
            // deep copy Numbers
            var numbers = new List<int>(Numbers);

            // Fill the first row with random numbers
            for (int i = 0; i < 5; i++)
            {
                var index = rand.Next(0, numbers.Count);
                square[0, i] = numbers[index];
                numbers.RemoveAt(index);
            }

            // Fill the second row with shifted numbers from the first row
            for (int i = 0; i < 5; i++)
            {
                square[1, i] = square[0, (i + 1) % 5];
            }

            // Fill the third row with shifted numbers from the second row
            for (int i = 0; i < 5; i++)
            {
                square[2, i] = square[1, (i + 1) % 5];
            }

            return new EvalKey
            {
                LatinSquare = square
            };
        }
    }
}