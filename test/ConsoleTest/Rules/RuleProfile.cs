using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using For.RuleEngine.Model;

namespace ConsoleTest.Rules
{
    public class RuleProfile : Rule<Profile, string, string>
    {
        public override bool Invoke(Profile instance)
        {
            return instance.Sex == "Boy";
        }
    }
}
