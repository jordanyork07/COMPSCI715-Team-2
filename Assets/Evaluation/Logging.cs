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

        public static void LogByEvalKey(EvalKey evalKey, string append)
        {
            using StreamWriter sw = File.AppendText($"eval-file-{evalKey.Encode()}.txt");
            //Debug.Log("Logging to " + $"eval-file-{evalKey.Encode()}.txt");
            sw.WriteLine($"[{GetGameTime()}] {append}");
            Debug.Log($"[{GetGameTime()}] {append}");
        }
        
        public static void LogLevelStart(EvalKey evalKey, int levelId)
        {
            LogByEvalKey(evalKey, $"Level {levelId} started");
        }
    }
}