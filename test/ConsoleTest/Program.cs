using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using For.RuleEngine;

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
            var finish = false;
            RuleFactory.RegisterFunc<Profile>("1", ".Age>20 & .Name=Ricky & .Sex=男", "His name is Ricky and he is a more than 20 years old", "Less than 30");
            RuleFactory.RegisterFunc<Profile>("1", ".Age<99", "Less than 30", "Over than 30");
            RuleFactory.RegisterFunc<Profile>("1", ".Name=Ricky", "Name is Ricky", "Name is not Ricky");
            RuleFactory.RegisterFunc<Order>("2", ".Total>1000", "100", "0");
            RuleFactory.RegisterFunc<Order>("2", ".Total>3000", "500", "0");
            RuleFactory.RegisterFunc<Order>("2", ".Total>5000", "1000", "0");
            var factory = new RuleFactory();
            var observable = factory.ApplyFuncs("1", new Profile()
            {
                Name = "Ricky",
                Age = 25,
                Sex = "男"
            });

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
            Console.ReadLine();

        }
    }

    public class Rule : For.RuleEngine.Rule<string, string>
    {
        public override bool Invoke()
        {
            return true;
        }
    }
}