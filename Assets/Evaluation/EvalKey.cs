using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Evaluation
{
    public record EvalKey
    {
        public int[] Order = new int[3];

        public string Encode()
        {
            var bytes = Encoding.UTF8.GetBytes(String.Join(',', Order));
            return Convert.ToBase64String(bytes);
        }

        public static EvalKey Decode(string base64)
        {
            var key = new EvalKey();
            
            var bytes = Convert.FromBase64String(base64);
            var toParse = Encoding.UTF8.GetString(bytes);

            var i = 0;
            foreach (var order in toParse.Split(','))
            {
                var number = int.Parse(order);
                key.Order[i++] = number;
            }

            return key;
        }

        public void Print()
        {
            var log = new StringBuilder();

            log.AppendLine(String.Join(',', Order));
            log.AppendLine(Encode());
            
            Debug.Log(log);
        }
    }
}