using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using For.RuleEngine.Model;

namespace ConsoleTest.Rules
{
    public class RuleOrder : Rule<Order, string, string>
    {
        public override bool Invoke(Order instance)
        {
            return instance.Total > 5000;
        }
    }
}
