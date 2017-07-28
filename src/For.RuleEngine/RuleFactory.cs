using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using For.ExpressionCompareGenerateEngine;
using For.RuleEngine.Model;

namespace For.RuleEngine
{
    public interface IRuleFactory
    {
        IObservable<Result<string, string>> ApplyFuncs<T>(string key, T instance);
        IObservable<Result<T, U>> Apply<T, U>(Rule<T, U> rule);
    }
    public class RuleFactory : IRuleFactory
    {
        private static readonly FormulaProcess _formulaProcessor = new FormulaProcess();
        private static readonly ExpressionProcess _expressionProcessor = new ExpressionProcess();
        private static readonly List<FuncModel> _lstFuncs = new List<FuncModel>();
        //        private readonly Lookup<string, FuncModel> _lookup = (Lookup<string, FuncModel>)_lstFuncs.ToLookup(p => p.Key, p => p);

        public static void RegisterFunc<T>(string key, string func, string passResult, string failureResult)
        {
            lock (_lstFuncs)
            {
                var realFunc = _expressionProcessor.GenerateFunc<T, bool>(_formulaProcessor.SeparateFormula(func));
                _lstFuncs.Add(new FuncModel()
                {
                    Key = key,
                    Func = realFunc,
                    PassResult = passResult,
                    FailureResult = failureResult
                });
            }
        }
        public static void ReseterFuncs()
        {
            lock (_lstFuncs)
            {
                _lstFuncs.Clear();
            }
        }
        public IObservable<Result<string, string>> ApplyFuncs<T>(string key, T instance)
        {
            lock (_lstFuncs)
            {
                var funcs = _lstFuncs.Where(p => p.Key == key);
                IObservable<Result<string, string>> obs = null;
                foreach (var func in funcs)
                {
                    var rule = new BasicFuncRule<T, string, string>((Func<T, bool>)func.Func, instance, func.PassResult, func.FailureResult);
                    obs = obs == null ? RuleProvider.GenerateObservable(rule) : obs.Concat(RuleProvider.GenerateObservable(rule));
                }
                return obs;
            }
        }
        public IObservable<Result<T, U>> Apply<T, U>(Rule<T, U> rule)
        {
            return RuleProvider.GenerateObservable(rule);
        }
    }
}
