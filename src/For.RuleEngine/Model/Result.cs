using System.ComponentModel;
using For.RuleEngine.Interface;
using For.RuleEngine.Model;

namespace For.RuleEngine.Model
{
    /// <summary>
    /// observer result
    /// </summary>
    /// <typeparam name="TPassResult"></typeparam>
    /// <typeparam name="TFailureResult"></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class Result<TPassResult, TFailureResult> : IResultBase<TPassResult, TFailureResult>
    {
        public bool IsPass { get; set; }
        public TPassResult PassResult { get; set; }
        public TFailureResult FailureResult { get; set; }
    }
}