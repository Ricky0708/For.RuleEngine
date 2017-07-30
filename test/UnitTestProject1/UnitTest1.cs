using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using For.RuleEngine;
using For.RuleEngine.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FuncTest()
        {
            var finish = false;
            IRuleFactory<string, string> factory = new RuleFactory<string, string>();
            factory.RegisterFunc<Profile>("1", ".Age>20 & .Name=Ricky & .Sex=男", "His name is Ricky and he is a more than 20 years old", "Less than 30");
            var observable = factory.Apply("1", new Profile()
            {
                Name = "Ricky",
                Age = 25,
                Sex = "男"
            });
            observable.Subscribe(
                next =>
                {
                    Assert.AreEqual(next.IsPass, true);
                    Assert.AreEqual(next.PassResult, "His name is Ricky and he is a more than 20 years old");
                    Assert.AreEqual(next.FailureResult, "Less than 30");
                },
                onError =>
                {
                    Console.WriteLine("Err");
                    finish = true;
                },
                () => finish = true);

            SpinWait.SpinUntil(() => finish, 1000 * 60 * 2);
        }
        [TestMethod]
        public void TemplateTest()
        {
            var finish = false;
            IRuleFactory<string, string> factory = new RuleFactory<string, string>();
            factory.RegisterTemplate<Profile>("1", new RuleProfile() { PassResult = "Pass", FailureResult = "Failure" });

            var observable = factory.Apply("1", new Profile()
            {
                Name = "Ricky",
                Age = 25,
                Sex = "男"
            });
            var p = observable.Subscribe(
                next =>
                {
                    Assert.AreEqual(next.IsPass, false);
                    Assert.AreEqual(next.PassResult, "Pass");
                    Assert.AreEqual(next.FailureResult, "Failure");
                },
                onError =>
                {
                    Console.WriteLine("Err");
                    finish = true;
                },
                () => finish = true);
            SpinWait.SpinUntil(() => finish, 1000 * 60 * 2);
            p.Dispose();
        }

        [TestMethod]
        public void FuncWithoutRegister()
        {
            var finish = false;
            IRuleFactory<string, string> factory = new RuleFactory<string, string>();
            factory.RegisterTemplate<Profile>("1", new RuleProfile() { PassResult = "Pass", FailureResult = "Failure" });
            var observable = factory.Apply(new Profile()
            {
                Name = "Ricky",
                Age = 25,
                Sex = "男"
            }, ".Name = Ricky", "Pass", "Failure");
            var p = observable.Subscribe(
                next =>
                {
                    Assert.AreEqual(next.IsPass, true);
                    Assert.AreEqual(next.PassResult, "Pass");
                    Assert.AreEqual(next.FailureResult, "Failure");
                },
                onError =>
                {
                    Console.WriteLine("Err");
                    finish = true;
                },
                () => finish = true);
            SpinWait.SpinUntil(() => finish, 1000 * 60 * 2);
            p.Dispose();
        }
        [TestMethod]
        public void TemplateWithoutRegister()
        {
            var finish = false;
            IRuleFactory<string, string> factory = new RuleFactory<string, string>();
            var observable = factory.Apply(new Profile()
            {
                Name = "Ricky",
                Age = 25,
                Sex = "男"
            }, new RuleProfileForNoRegister(new Order() { Total = 25 }) { PassResult = "Pass", FailureResult = "Failure" });

            var p = observable.Subscribe(
                next =>
                {
                    Assert.AreEqual(next.IsPass, true);
                    Assert.AreEqual(next.PassResult, "Pass");
                    Assert.AreEqual(next.FailureResult, "Failure");
                },
                onError =>
                {
                    Console.WriteLine("Err");
                    finish = true;
                },
                () => finish = true);
            SpinWait.SpinUntil(() => finish, 1000 * 60 * 2);
            p.Dispose();
        }
    }
}
