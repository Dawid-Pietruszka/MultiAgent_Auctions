using ActressMas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAS_Coursework
{
    public enum ServiceType { Provider, Consumer, NA };

    class HouseholdAgent : Agent
    {
        private int generation;
        private int demand;
        private int utilityBuy;
        private int utilitySell;
        private int _currentBid;
        private int balance;
        private int difference = 0;
        private bool participate;
        private int shortage;
        private int surplus;
        private int sold;
        private int bought;
        private ServiceType _type;
        public HouseholdAgent()
        {
        }
        public override void Setup()
        {
            Send("EnvironmentAgent", "start");
        }
        public override void Act(Message message)
        {
            int highestBid = 0;
            message.Parse(out string action, out string parameters);
            
            switch (action)
            {
                case "inform":
                    string getMessage = parameters;
                    string[] content = getMessage.Split();
                    demand = Convert.ToInt32(content[0]);
                    generation = Convert.ToInt32(content[1]);
                    utilityBuy = Convert.ToInt32(content[2]);
                    utilitySell = Convert.ToInt32(content[3]);
                    string msg = $"{Name}: \r\n\tMy demand is " + demand + "\r\n\tMy Generation is " + generation + "\r\n\tMy Utility Price is " + utilityBuy + "\r\n\tMy Utility Sell is " + utilitySell;
                    Console.WriteLine(msg);

                    if(generation > demand)
                    {
                        surplus = generation - demand;
                        participate = false;
                        Console.WriteLine($"\r\tI am a Provider");
                        _type = ServiceType.Provider;
                        Send("Auctioneer", $"provider {surplus} {utilitySell}");
                        
                    }
                    else if(generation == demand)
                    {
                        Console.WriteLine($"\r\tI do not need to participate");
                        _type = ServiceType.NA;
                        Settings.NoHouseholds--;
                        Stop();
                    }
                    else
                    {
                        _currentBid = 1;
                        shortage = demand - generation;
                        participate = true;
                        Console.WriteLine($"\r\tI am a Consumer");
                        _type = ServiceType.Consumer;
                        Send("Auctioneer", $"consumer {shortage}");
                        HandleStart();
                    }
                    break;

                case "bid":
                    if(participate == true)
                    {
                        int bid = Convert.ToInt32(parameters);
                        if (bid > highestBid)
                            highestBid = bid;
                        if (highestBid > 0)
                            HandleBid(highestBid);
                    }
                    break;

                case "newBid":
                    if (participate == true)
                    {
                        _currentBid = Convert.ToInt32(parameters);
                        highestBid = 0;
                        HandleStart();
                    }
                    break;

                case "transaction":
                    Console.WriteLine($"{message.Format()}");
                    generation--;
                    balance = balance + Convert.ToInt32(parameters);
                    sold++;
                    difference = difference + (utilitySell - _currentBid);
                    Send("Auctioneer", $"newBid");
                    break;

                case "end":
                    if(surplus > 0)
                    {
                        balance = balance + (surplus * utilitySell);
                    }
                    else
                    {
                        balance = balance + ((shortage * -1) * utilityBuy);
                    }
                    Console.WriteLine($"{message.Format()}: My new balance is: £{balance}");
                    Send("CalculatorAgent", $"output {Name} {_type} {bought} {sold} {balance} {difference}");
                    Stop();
                    break;

                case "winner":
                    Console.WriteLine($"[{Name}] Winner");
                    HandleWinner(parameters);
                    break;

                default:
                    break;
            }
        }
        private void HandleBid(int receivedBid)
        {
            int next = receivedBid + Settings.Increment;
            if (receivedBid >= _currentBid && next < utilityBuy)
            {
                _currentBid = next;
                Broadcast($"bid {next}");
            }
        }
        private void HandleStart()
        {
            if (_currentBid > 0)
                Broadcast($"bid {_currentBid}");
        }
        private void HandleWinner(string supplier)
        {
            if (Name != "none")
            {
                generation++;
                bought++;
                difference = difference + (utilityBuy -_currentBid);
                Console.WriteLine($"[{Name}]: I have won with £{_currentBid} from {supplier} , My energy is now: " + generation + " My demand is: " + demand);

                if (generation >= demand)
                {
                    participate = false;
                    Console.WriteLine($"[{Name}]: Has met it's demand");
                }

            Send(supplier, $"transaction {_currentBid}");
            }
        }
    }
}