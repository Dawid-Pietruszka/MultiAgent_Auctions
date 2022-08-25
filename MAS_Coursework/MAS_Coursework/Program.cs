using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAS_Coursework
{
    class Program
    {
        static void Main(string[] args)
        {
            var env = new EnvironmentMas(randomOrder: false, parallel: false);
            List<HouseholdAgent> agents = new List<HouseholdAgent>();
            string name = "";
            for(int x = 1; x <= Settings.NoHouseholds; x++)
            {
                var houseHoldAgent = new HouseholdAgent();
                name = "Household" + x.ToString();
                env.Add(houseHoldAgent, $"{name}");
                agents.Add(houseHoldAgent);
                name = "";
            }

            var environmentAgent = new EnvironmentAgent();
            env.Add(environmentAgent, "EnvironmentAgent");
            var auctioneerAgent = new AuctioneerAgent();
            env.Add(auctioneerAgent, "Auctioneer");
            var calculatorAgent = new CalculatorAgent();
            env.Add(calculatorAgent, "CalculatorAgent");
            env.Start();
        }
    }
}
