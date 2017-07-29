using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using For.ExpressionCompareGenerateEngine;
using For.RuleEngine;
using For.RuleEngine.Model;

namespace For.RuleEngine
{
    public interface IRuleFactory<TPassResult, TFailureResult>
    {
        /// <summary>
        /// get observable for subscribe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupKey"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        IObservable<Result<TPassResult, TFailureResult>> Apply<T>(string groupKey, T instance);
        /// <summary>
        /// regist rule string func to container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="passResult"></param>
        /// <param name="failureResult"></param>
        void RegisterFunc<T>(string key, string func, TPassResult passResult, TFailureResult failureResult);
        /// <summary>
        /// register rule template to container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="rule"></param>
        void RegisterTemplate<T>(string key, Rule<T, TPassResult, TFailureResult> rule);
        /// <summary>
        /// reset container
        /// </summary>
        void ReseterRules();
    }
    public class RuleFactory : IRuleFactory<string, string>
    {
        private static readonly FormulaProcess _formulaProcessor = new FormulaProcess(); //Separate string formula
        private static readonly ExpressionProcess _expressionProcessor = new ExpressionProcess(); // generate formula to expression
        private readonly List<RuleModel> _lstRules = new List<RuleModel>();

        public void RegisterFunc<T>(string key, string func, string passResult, string failureResult)
        {
            lock (_lstRules)
            {
                var realFunc = _expressionProcessor.GenerateFunc<T, bool>(_formulaProcessor.SeparateFormula(func)); //make func
                var rule = new BasicFuncRule<T, string, string>(realFunc, passResult, failureResult); // make func rule
                //add to container
                _lstRules.Add(new RuleModel()
                {
                    Key = key,
                    Rule = rule
                });
            }
        }

        public void RegisterTemplate<T>(string key, Rule<T, string, string> rule)
        {
            //add to container
            lock (_lstRules)
            {
                _lstRules.Add(new RuleModel()
                {
                    Key = key,
                    Rule = rule
                });
            }
        }

        public void ReseterRules()
        {
            //clean container
            lock (_lstRules)
            {
                _lstRules.Clear();
            }
        }

        public IObservable<Result<string, string>> Apply<T>(string groupKey, T instance)
        {
            // get concat observable
            lock (_lstRules)
            {
                var funcs = _lstRules.Where(p => p.Key == groupKey);
                return funcs.Aggregate<RuleModel, IObservable<Result<string, string>>>(null, (current, rule) => current == null
                    ? RuleProvider.GenerateObservable(instance, (Rule<T, string, string>)rule.Rule)
                    : current.Concat(RuleProvider.GenerateObservable(instance, (Rule<T, string, string>)rule.Rule)));
            }
        }

    }
}
