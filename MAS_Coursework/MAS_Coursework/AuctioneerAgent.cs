using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAS_Coursework
{
    class AuctioneerAgent : Agent
    {
        private string _highestBidder;
        private int _highestBid;
        private int _turnsToWait;
        private int auctionNum = 1;
        private Dictionary<string, int> providers =
    new Dictionary<string, int>();
        private List<int> minSell = new List<int>();
        private Dictionary<string, int> customers =
new Dictionary<string, int>();
        private string current_provider;
        public override void Setup()
        {
            _highestBidder = "";
            _highestBid = 0;
            _turnsToWait = 2;
            Console.WriteLine("Auction");
        }
        public override void Act(Message message)
        {
            try
            {
                message.Parse(out string action, out string parameters);
                _turnsToWait = 2;
                string senderID;
                switch (action)
                {
                    case "bid":
                        Console.WriteLine($"\t{message.Format()}");
                        HandleBid(message.Sender, Convert.ToInt32(parameters));
                        break;

                    case "newBid":
                        Console.Write("\n\rA new bid begins..."); 
                        _highestBid = 0;
                        _highestBidder = "";
                        break;

                    case "provider":
                        senderID = message.Sender;
                        string[] msg = parameters.Split(' ');
                        providers.Add(senderID, Convert.ToInt32(msg[0]));
                        minSell.Add(Convert.ToInt32(msg[1]));
                        break;

                    case "consumer":
                        senderID = message.Sender;
                        customers.Add(senderID, Convert.ToInt32(parameters));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void HandleBid(string sender, int bid)
        {
            if (bid > _highestBid && bid >= minSell[0])
            {
                _highestBid = bid;
                _highestBidder = sender;
            }
        }
        private void HandleFinish()
        {
            
            Console.WriteLine($"\n\r[Auctioneer]:{auctionNum} Auction finished");
            auctionNum++;
            if (providers.Count == 0 || customers.Count == 0)
            {
                Broadcast("end");
                Stop();
            }
            else if (_highestBidder != "")
            {
                var first_provider = providers.First();
                current_provider = first_provider.Key;
                Dictionary<string, int> tempprov = new Dictionary<string, int>();
                Dictionary<string, int> tempcust = new Dictionary<string, int>();
                foreach (var item in providers)
                {
                    tempprov.Add(item.Key, item.Value);
                    if (current_provider == item.Key)
                    {
                        if (item.Value > 0)
                        {
                            tempprov[current_provider]--;
                        }
                    }
                }
                providers = tempprov;
                if(providers[current_provider] == 0)
                {
                    providers.Remove(current_provider);
                    minSell.Remove(minSell[0]);
                }
                foreach (var item in customers)
                {
                    tempcust.Add(item.Key, item.Value);
                    if (_highestBidder == item.Key)
                    {
                        if (item.Value > 0)
                        {
                            tempcust[_highestBidder]--;
                        }
                    }
                }
                customers = tempcust;
                if (customers[_highestBidder] == 0)
                {
                    customers.Remove(_highestBidder);
                }
                
                _highestBid = 0;
                string highestBidder = _highestBidder;
                _highestBidder = "";
                Send(highestBidder, $"winner {current_provider}"); 
            }
            else
            {
                if(_highestBidder == "")
                {
                    int x = minSell[0];
                    Broadcast($"newBid {x}");
                }
                
            }
        }
        public override void ActDefault()
        {
            if (--_turnsToWait <= 0)
                HandleFinish();
        }
    }
}
