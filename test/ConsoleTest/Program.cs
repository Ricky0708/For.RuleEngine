using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleTest.Rules;
using For.RuleEngine;
using For.RuleEngine.Model;

namespace ConsoleTest
{

    public class Profile
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
    }
    public class Order
    {
        public int Total { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            RunProfile();
            //            RunOrder();

            Console.ReadLine();

        }

        static void RunProfile()
        {
            var finish = false;
            IRuleFactory<string, string> factory = new RuleFactory<string, string>();
            factory.RegisterFunc<Profile>("1", ".Age>20 & .Name=Ricky & .Sex=男", "His name is Ricky and he is a more than 20 years old", "A");
            factory.RegisterFunc<Profile>("1", ".Age<99", "Less than 30", "B");
            factory.RegisterFunc<Profile>("1", ".Name=Ricky", "Name is Ricky", "C");
            factory.RegisterTemplate<Profile>("1", new RuleProfile() { PassResult = "Pass", FailureResult = "D" });
            factory.RegisterTemplate<Profile>("1", new RuleProfileCompareOrder(new Order() { Total = 1000 }) { PassResult = "A", FailureResult = "E" });
            //-------------------
            var observable = factory.ApplyAsync("1", new Profile() { Name = "Ricky", Age = 25, Sex = "男" });
            //-------------------
            observable.Subscribe(
                next =>
                {
                    Console.WriteLine(next.IsPass);
                    Console.WriteLine(next.PassResult);
                    Console.WriteLine(next.FailureResult);
                    Console.WriteLine("");
                },
                onError =>
                {
                    Console.WriteLine("Err");
                    finish = true;
                },
                () => finish = true);


            SpinWait.SpinUntil(() => finish, 1000 * 60 * 2);
            Console.WriteLine("finsih");
        }

        static void RunOrder()
        {
            var finish = false;
            IRuleFactory<string, string> factory = new RuleFactory<string, string>();
            factory.RegisterFunc<Order>("2", ".Total>1000", "100", "0");
            factory.RegisterFunc<Order>("2", ".Total>3000", "500", "0");
            factory.RegisterFunc<Order>("2", ".Total>5000", "1000", "0");
            factory.RegisterTemplate<Order>("2", new RuleOrder() { PassResult = "Pass", FailureResult = "Failure" });
            var observable = factory.Apply("2", new Order() { Total = 3500 });
            observable.Subscribe(
                next =>
                {
                    Console.WriteLine(next.IsPass);
                    Console.WriteLine(next.PassResult);
                    Console.WriteLine(next.FailureResult);
                    Console.WriteLine("");
                },
                onError =>
                {
                    Console.WriteLine("Err");
                    finish = true;
                },
                () => finish = true);
            SpinWait.SpinUntil(() => finish, 1000 * 60 * 2);
        }

    }
}