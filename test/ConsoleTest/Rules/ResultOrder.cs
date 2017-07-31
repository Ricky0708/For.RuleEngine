using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using For.RuleEngine.Interface;
using For.RuleEngine.Model;

namespace ConsoleTest.Rules
{
    public class RuleOrder : Rule<Order, string, string>
    {
        public override string PassResult { get; set; }
        public override string FailureResult { get; set; }

        public override bool Invoke(Order instance)
        {
            return instance.Total > 5000;
        }
    }
}