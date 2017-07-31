using System;
using For.RuleEngine.Interface;

namespace For.RuleEngine.Model
{
    /// <summary>
    /// func rule model
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TPassResult"></typeparam>
    /// <typeparam name="TFailureResult"></typeparam>
    internal class BasicFuncRule<TInstance, TPassResult, TFailureResult> : Rule<TInstance, TPassResult, TFailureResult>
    {
        private readonly Func<TInstance, bool> _func;
        public sealed override TPassResult PassResult { get; set; }
        public sealed override TFailureResult FailureResult { get; set; }

        internal BasicFuncRule(Func<TInstance, bool> func, TPassResult passResult, TFailureResult failureResult)
        {
            _func = func;
            this.PassResult = passResult;
            this.FailureResult = failureResult;
        }

        public override bool Invoke(TInstance instance)
        {
            return _func.Invoke(instance);
        }


    }
}