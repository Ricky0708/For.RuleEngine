using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using For.RuleEngine.Model;

namespace For.RuleEngine
{
    public interface IRuleObserverProvider
    {
        IObservable<Result<TPassresult, TFailureResult>> GenerateObservable<TInstance, TPassresult, TFailureResult>(TInstance instance, Rule<TInstance, TPassresult, TFailureResult> model);
    }
    public class RuleObserverDefaultProvider : IRuleObserverProvider
    {
        /// <summary>
        /// make observable, if exception, it will trigger on error in observer
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TPassresult"></typeparam>
        /// <typeparam name="TFailureResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IObservable<Result<TPassresult, TFailureResult>> GenerateObservable<TInstance, TPassresult, TFailureResult>(TInstance instance, Rule<TInstance, TPassresult, TFailureResult> model)
        {
            var observable = Observable.Create<Result<TPassresult, TFailureResult>>(ob =>
            {
                var isPass = model.Invoke(instance);
                ob.OnNext(new Result<TPassresult, TFailureResult>() //next call back
                {
                    IsPass = isPass,
                    PassResult = model.PassResult,
                    FailureResult = model.FailureResult,
                });
                ob.OnCompleted(); // compleate
                return Disposable.Create(() => { });
            });

            return observable;
        }
    }
}
