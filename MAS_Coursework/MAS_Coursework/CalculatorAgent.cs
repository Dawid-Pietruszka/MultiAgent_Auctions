using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAS_Coursework
{
    class CalculatorAgent : Agent
    {
        private List<List<string>> data = new List<List<string>>();

        public override void Setup()
        {

        }
        public override void Act(Message message)
        {
            try
            {
                message.Parse(out string action, out string parameters);
                switch (action)
                {
                    case "output":
                        string[] msg = parameters.Split(' ');
                        List<string> msgList = new List<string>();
                        msgList.AddRange(msg);
                        data.Add(msgList);
                        if (data.Count() == Settings.NoHouseholds)
                        {
                            HandleOutput();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void HandleOutput()
        {
            Console.WriteLine("{0,10}{1,10}{2,10}{3,10}{4,10}{5,10}", "ID", "Type", "Purchases", "Sold", "Balance", "Profit");
            double profitavg = 0;
            double profitavgprovider = 0;
            double profitavgconsumer = 0;
            int count = 0;
            int countc = 0;
            foreach (var item in data)
            {
                Console.WriteLine("{0,10}{1,10}{2,10}{3,10}{4,10}{5,10}", item[0].ToString(), item[1].ToString(), item[2].ToString(), item[3].ToString(), item[4].ToString(), item[5].ToString());
                profitavg = profitavg + Convert.ToInt32(item[5]);
                if(item[1] == "Provider")
                {
                    profitavgprovider = profitavgprovider + Convert.ToInt32(item[5]);
                    count++;
                }
                else
                {
                    profitavgconsumer = profitavgconsumer + Convert.ToInt32(item[5]);
                    countc++;
                }
            }
            profitavg = profitavg / Settings.NoHouseholds;
            profitavgprovider = profitavgprovider / count;
            profitavgconsumer = profitavgconsumer / countc;
            Console.WriteLine("The average profit for the Households is: " + profitavg.ToString());
            Console.WriteLine("The average profit for the Providers is: " + profitavgprovider.ToString());
            Console.WriteLine("The average profit for the Consumers is: " + profitavgconsumer.ToString());
        }
    }
}
