using System.ComponentModel;

namespace For.RuleEngine.Model
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ValidateBase<TPassResult, TFailureResult>
    {
        public TPassResult PassResult { get; set; }
        public TFailureResult FailureResult { get; set; }
    }
}