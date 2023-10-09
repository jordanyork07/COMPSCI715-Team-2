using System.Text;
using UnityEngine;

namespace Evaluation
{
    public abstract class EvalKeyGenerator
    {
        public static void Generate()
        {
            // latin "rectangle"
            var square = new int[6, 3] {
                { 1, 2, 3 },
                { 3, 1, 2 },
                { 2, 3, 1 },
                { 1, 3, 2 },
                { 3, 2, 1 },
                { 2, 1, 3 }
            };

            var strBuild = new StringBuilder();
            for (var row = 0; row < 6; row++)
            {
                var key = new EvalKey
                {
                    Order = new[] { square[row, 0], square[row, 1], square[row, 2] }
                };

                strBuild.AppendLine(key.Encode());
            }
            
            Debug.Log(strBuild);
        }
    }
}