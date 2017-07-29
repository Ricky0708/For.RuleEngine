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
    internal class RuleProvider
    {
        internal static IObservable<Result<TPassresult, TFailureResult>> GenerateObservable<TInstance, TPassresult, TFailureResult>(TInstance instance, Rule<TInstance,TPassresult,TFailureResult> model)
        {
            var observable = Observable.Create<Result<TPassresult, TFailureResult>>(ob =>
            {
                var isPass = model.Invoke(instance);
                ob.OnNext(new Result<TPassresult, TFailureResult>()
                {
                    IsPass = isPass,
                    PassResult = model.PassResult,
                    FailureResult = model.FailureResult,
                });
                ob.OnCompleted();
                return Disposable.Create(() => { });
            });
            return observable;
        }
    }
}
