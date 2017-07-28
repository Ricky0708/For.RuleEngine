using System;
using For.RuleEngine.Model;

namespace For.RuleEngine
{
    public abstract class Rule<TPassResult, TFailureResult> : ValidateBase<TPassResult, TFailureResult>
    {
        public abstract bool Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInstance">驗證實體型別</typeparam>
    /// <typeparam name="TPassResult">驗證成功回傳型別</typeparam>
    /// <typeparam name="TFailureResult">驗證失則回傳型別</typeparam>
    internal class BasicFuncRule<TInstance, TPassResult, TFailureResult> : Rule<TPassResult, TFailureResult>
    {

        private readonly Func<TInstance, bool> _func;
        private readonly TInstance _instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">驗證的委派</param>
        /// <param name="instance">驗證的實體</param>
        /// <param name="passResult">成功的回傳值</param>
        /// <param name="failureResult">失敗的回傳值</param>
        internal BasicFuncRule(Func<TInstance, bool> func, TInstance instance, TPassResult passResult, TFailureResult failureResult)
        {
            _func = func;
            this.PassResult = passResult;
            this.FailureResult = failureResult;
            this._instance = instance;
        }

        public override bool Invoke()
        {
            return Invoke(_instance);
        }
        internal bool Invoke(TInstance instance)
        {
            return _func.Invoke(instance);
        }

        
    }

  
}
