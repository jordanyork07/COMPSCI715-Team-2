using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Evaluation
{
    // Latin Square
    public record EvalKey
    {
        public int[,] LatinSquare = new int[3,5];

        public string Encode()
        {
            var categories = new List<string>();
            for (var i = 0; i < 3; i++)
            {
                var levels = new List<int>();
                for (var j = 0; j < 5; j++)
                {
                    levels.Add(LatinSquare[i, j]);
                }
                categories.Add(String.Join(',', levels));
            }

            var bytes = Encoding.UTF8.GetBytes(String.Join(';', categories));
            return Convert.ToBase64String(bytes);
        }

        public static EvalKey Decode(string base64)
        {
            var key = new EvalKey();
            
            var bytes = Convert.FromBase64String(base64);
            var toParse = Encoding.UTF8.GetString(bytes);

            var categories = toParse.Split(';');
            var i = 0;
            foreach (var category in categories)
            {
                var j = 0;
                var levels = category.Split(',');
                foreach (var level in levels)
                {
                    var number = int.Parse(level);
                    key.LatinSquare[i, j++] = number;
                }

                i++;
            }

            return key;
        }

        public void Print()
        {
            var rows = LatinSquare.GetLength(0);
            var cols = LatinSquare.GetLength(1);

            var log = new StringBuilder();

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    log.AppendLine(LatinSquare[i, j] + " ");
                }
                log.AppendLine();
            }

            log.AppendLine(Encode());
            
            Debug.Log(log);
        }
    }
}