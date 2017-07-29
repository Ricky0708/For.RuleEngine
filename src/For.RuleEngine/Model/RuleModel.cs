using System;

namespace For.RuleEngine.Model
{
    internal class RuleModel
    {
        internal string Key { get; set; }
        internal IRule Rule { get; set; }
    }
    /// <summary>
    /// for container collect rules
    /// </summary>
    internal interface IRule
    {
    }

    public abstract class Rule<TInstance, TPassResult, TFailureResult> : _ValidateBase<TPassResult, TFailureResult>, IRule
    {
        public abstract bool Invoke(TInstance instance);
    }

    internal class BasicFuncRule<TInstance, TPassResult, TFailureResult> : Rule<TInstance, TPassResult, TFailureResult>
    {
        private readonly Func<TInstance, bool> _func;

       
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