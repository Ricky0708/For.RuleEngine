using For.RuleEngine.Interface;
namespace For.RuleEngine.Model
{
    /// <summary>
    /// rule
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TPassResult"></typeparam>
    /// <typeparam name="TFailureResult"></typeparam>
    public abstract class Rule<TInstance, TPassResult, TFailureResult> : IResultBase<TPassResult, TFailureResult>, IContainerRule
    {
        public abstract bool Invoke(TInstance instance);
        public abstract TPassResult PassResult { get; set; }
        public abstract TFailureResult FailureResult { get; set; }
    }

}