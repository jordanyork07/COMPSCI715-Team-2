namespace Evaluation
{
    public static class Evaluator
    {
        public static EvalKey Key { get; private set; }

        static Evaluator()
        {
            Key = new EvalKey();
        }

        public static void SetEvalKey(string base64)
        {
            Key = EvalKey.Decode(base64);
        }
    }
}