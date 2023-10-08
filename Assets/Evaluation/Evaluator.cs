namespace Evaluation
{
    public static class Evaluator
    {
        public static EvalKey Key { get; private set; }

        public static void SetEvalKey(string base64)
        {
            Key = EvalKey.Decode(base64);
        }
    }
}