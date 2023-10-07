using System.IO;
using UnityEngine;

namespace Evaluation
{
    public static class Logger
    {
        private static float GetGameTime()
        {
            return Time.realtimeSinceStartup;
        }

        private static void LogByEvalKey(EvalKey evalKey, string append)
        {
            using StreamWriter sw = File.AppendText($"eval-file-{evalKey.Encode()}.txt");
            sw.WriteLine($"[{GetGameTime()}] {append}");
        }
        
        public static void LogLevelStart(EvalKey evalKey, int levelId)
        {
            LogByEvalKey(evalKey, $"Level {levelId} started");
        }
    }
}