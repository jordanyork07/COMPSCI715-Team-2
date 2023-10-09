using System.IO;
using UnityEngine;

namespace Evaluation
{
    public static class Logger
    {
        private static float GetGameTime()
        {
            return Time.time;
        }

        public static void LogByEvalKey(EvalKey evalKey, string append)
        {
            var deviceId = SystemInfo.deviceUniqueIdentifier;
            using StreamWriter sw = File.AppendText($"eval-file?key={evalKey.Encode()}&id={deviceId}.txt");
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