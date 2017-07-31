using System.ComponentModel;

namespace For.RuleEngine.Interface
{
    /// <summary>
    /// pass and result common interface
    /// </summary>
    /// <typeparam name="TPassResult"></typeparam>
    /// <typeparam name="TFailureResult"></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IResultBase<TPassResult, TFailureResult>
    {
        TPassResult PassResult { get; set; }
        TFailureResult FailureResult { get; set; }
    }
}