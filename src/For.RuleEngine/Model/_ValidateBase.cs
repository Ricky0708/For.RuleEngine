using System.ComponentModel;

namespace For.RuleEngine.Model
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class _ValidateBase<TPassResult, TFailureResult>
    {
        public TPassResult PassResult { get; set; }
        public TFailureResult FailureResult { get; set; }
    }
}