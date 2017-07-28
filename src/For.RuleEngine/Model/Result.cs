using System.ComponentModel;
using For.RuleEngine.Model;

namespace For.RuleEngine.Model
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class Result<TPassResult, TFailureResult> : ValidateBase<TPassResult, TFailureResult>
    {
        public bool IsPass { get; set; }
        
    }
}