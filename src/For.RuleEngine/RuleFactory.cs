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
        /// regist rule string func to container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupKey"></param>
        /// <param name="func"></param>
        /// <param name="passResult"></param>
        /// <param name="failureResult"></param>
        void RegisterFunc<T>(string groupKey, string func, TPassResult passResult, TFailureResult failureResult);
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
        /// <summary>
        /// make observable from register container for subscribe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupKey"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        IObservable<Result<TPassResult, TFailureResult>> Apply<T>(string groupKey, T instance);

        /// <summary>
        /// make observable from input parameter use rule template
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        IObservable<Result<TPassResult, TFailureResult>> Apply<T>(T instance, Rule<T, TPassResult, TFailureResult> rule);

        /// <summary>
        /// make observable from input parameter use string func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="func"></param>
        /// <param name="passResult"></param>
        /// <param name="failureResult"></param>
        /// <returns></returns>
        IObservable<Result<TPassResult, TFailureResult>> Apply<T>(T instance, string func, TPassResult passResult, TFailureResult failureResult);

        /// <summary>
        /// observable generate provider
        /// </summary>
        IRuleObserverProvider Provider { get; }
    }
    public class RuleFactory<TPassResult, TFailureResult> : IRuleFactory<TPassResult, TFailureResult>
    {
        private static readonly FormulaProcess _formulaProcessor = new FormulaProcess(); //Separate string formula
        private static readonly ExpressionProcess _expressionProcessor = new ExpressionProcess(); // generate formula to expression
        private readonly List<RuleModel> _lstRules = new List<RuleModel>();


        public IRuleObserverProvider Provider { get; }

        /// <summary>
        /// <see cref="IRuleFactory{TPassResult, TFailureResult}"/>
        /// </summary>
        /// <param name="provider">if null use default provider</param>
        public RuleFactory(IRuleObserverProvider provider = null)
        {
            Provider = provider ?? new RuleObserverDefaultProvider();
        }

        /// <summary>
        /// <see cref="IRuleFactory{TPassResult, TFailureResult}.RegisterFunc{T}(string, string, TPassResult, TFailureResult)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupKey"></param>
        /// <param name="func"></param>
        /// <param name="passResult"></param>
        /// <param name="failureResult"></param>
        public void RegisterFunc<T>(string groupKey, string func, TPassResult passResult, TFailureResult failureResult)
        {
            lock (_lstRules)
            {
                var realFunc = _expressionProcessor.GenerateFunc<T, bool>(_formulaProcessor.SeparateFormula(func)); //make func
                var rule = new BasicFuncRule<T, TPassResult, TFailureResult>(realFunc, passResult, failureResult); // make func rule
                //add to container
                _lstRules.Add(new RuleModel()
                {
                    Key = groupKey,
                    Rule = rule
                });
            }
        }

        /// <summary>
        /// <see cref="IRuleFactory{TPassResult, TFailureResult}.RegisterTemplate{T}(string, Rule{T, TPassResult, TFailureResult})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="rule"></param>
        public void RegisterTemplate<T>(string key, Rule<T, TPassResult, TFailureResult> rule)
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

        /// <summary>
        /// <see cref="IRuleFactory{TPassResult, TFailureResult}.ReseterRules"/>
        /// </summary>
        public void ReseterRules()
        {
            //clean container
            lock (_lstRules)
            {
                _lstRules.Clear();
            }
        }


        /// <summary>
        /// <see cref="IRuleFactory{TPassResult, TFailureResult}.Apply{T}(string, T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupKey"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IObservable<Result<TPassResult, TFailureResult>> Apply<T>(string groupKey, T instance)
        {
            // get concat observable
            lock (_lstRules)
            {
                var funcs = _lstRules.Where(p => p.Key == groupKey);
                return funcs.Aggregate<RuleModel, IObservable<Result<TPassResult, TFailureResult>>>(null, (current, rule) => current == null
                    ? Provider.GenerateObservable(instance, (Rule<T, TPassResult, TFailureResult>)rule.Rule)
                    : current.Concat(Provider.GenerateObservable(instance, (Rule<T, TPassResult, TFailureResult>)rule.Rule)));
            }
        }

        /// <summary>
        /// <see cref="IRuleFactory{TPassResult, TFailureResult}.Apply{T}(T, Rule{T, TPassResult, TFailureResult})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public IObservable<Result<TPassResult, TFailureResult>> Apply<T>(T instance, Rule<T, TPassResult, TFailureResult> rule)
        {
            return Provider.GenerateObservable(instance, rule);
        }

        /// <summary>
        /// <see cref="IRuleFactory{TPassResult, TFailureResult}.Apply{T}(T, string, TPassResult, TFailureResult)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="func"></param>
        /// <param name="passResult"></param>
        /// <param name="failureResult"></param>
        /// <returns></returns>
        public IObservable<Result<TPassResult, TFailureResult>> Apply<T>(T instance, string func, TPassResult passResult, TFailureResult failureResult)
        {
            var realFunc = _expressionProcessor.GenerateFunc<T, bool>(_formulaProcessor.SeparateFormula(func)); //make func
            var rule = new BasicFuncRule<T, TPassResult, TFailureResult>(realFunc, passResult, failureResult); // make func rule
            return Provider.GenerateObservable(instance, rule);
        }
    }
}
