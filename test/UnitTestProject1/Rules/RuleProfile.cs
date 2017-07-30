using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using For.RuleEngine.Model;

namespace UnitTestProject1
{
    public class RuleProfile : Rule<Profile, string, string>
    {
        public override bool Invoke(Profile instance)
        {
            return instance.Sex == "Boy";
        }
    }

    public class RuleProfileForNoRegister : Rule<Profile, string, string>
    {
        private readonly Order _order;

        public RuleProfileForNoRegister(Order order)
        {
            _order = order;
        }
        public override bool Invoke(Profile instance)
        {
            return instance.Age == _order.Total;
        }
    }
}
