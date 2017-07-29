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
        IObservable<Result<TPassResult, TFailureResult>> ApplyFuncs<T>(string key, T instance);
        void RegisterFunc<T>(string key, string func, TPassResult passResult, TFailureResult failureResult);
        void RegisterTemplate<T>(string key, Rule<T, TPassResult, TFailureResult> rule);
        void ReseterRules();
    }
    public class RuleFactory : IRuleFactory<string, string>
    {
        private static readonly FormulaProcess _formulaProcessor = new FormulaProcess();
        private static readonly ExpressionProcess _expressionProcessor = new ExpressionProcess();
        private static readonly List<RuleModel> _lstRules = new List<RuleModel>();

        public void RegisterFunc<T>(string key, string func, string passResult, string failureResult)
        {
            lock (_lstRules)
            {
                var realFunc = _expressionProcessor.GenerateFunc<T, bool>(_formulaProcessor.SeparateFormula(func));
                var rule = new BasicFuncRule<T, string, string>(realFunc, passResult, failureResult);
                _lstRules.Add(new RuleModel()
                {
                    Key = key,
                    Rule = rule
                });
            }
        }

        public void RegisterTemplate<T>(string key, Rule<T, string, string> rule)
        {
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
            lock (_lstRules)
            {
                _lstRules.Clear();
            }
        }

        public IObservable<Result<string, string>> ApplyFuncs<T>(string key, T instance)
        {
            lock (_lstRules)
            {
                var funcs = _lstRules.Where(p => p.Key == key);
                return funcs.Aggregate<RuleModel, IObservable<Result<string, string>>>(null, (current, rule) => current == null
                    ? RuleProvider.GenerateObservable(instance, (Rule<T, string, string>)rule.Rule)
                    : current.Concat(RuleProvider.GenerateObservable(instance, (Rule<T, string, string>)rule.Rule)));
            }
        }

    }
}
