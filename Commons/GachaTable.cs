using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AellaDiscordBot.Bots.Commons
{
    public class GachaTable
    {
        public static GachaTable Instance;
        public List<Card> Cards { get; private set; }
        public Random Random { get; private set; }
        public ConfigJson Config { get; private set; }

        public Card GetRandomCard()
        {
            double value = Random.NextDouble() * Cards[^1].Cumulative;
            return Cards.Last(card => card.Cumulative <= value);
        }

        public GachaTable(ConfigJson config)
        {
            Config = config;

            GetCards();

            Random = new Random();

            Instance = this;
        }

        public void GetCards()
        {
            Cards = new List<Card>();

            //string[] fileImgs = DBConnector.GetCards();

            List<Card> cards = DBConnector.GetCards().OrderByDescending(c => c.Rarity).ThenBy(c => c.Id).ToList();

            int UQCards = cards.Where(c => c.Rarity == Rarity.UQ).Count();
            int HHQCards = cards.Where(c => c.Rarity == Rarity.HHQ).Count();
            int HQCards = cards.Where(c => c.Rarity == Rarity.HQ).Count();
            int NQPCards = cards.Where(c => c.Rarity == Rarity.NQP).Count();
            int NQCards = cards.Where(c => c.Rarity == Rarity.NQ).Count();

            double cumulativeWeight = 0.0;
            int totalCards = cards.Count();

            double uqWeight = Config.UQRate / UQCards;
            double hhqWeight = Config.HHQRate / HHQCards;
            double hqWeight = Config.HQRate / HQCards;
            double nqpWeight = Config.NQPRate / NQPCards;
            double nqWeight = Config.NQRate / NQCards;

            Console.WriteLine($"UQ Cards found {UQCards} | Weight: {uqWeight}");
            Console.WriteLine($"HHQ Cards found {HHQCards} | Weight: {hhqWeight}");
            Console.WriteLine($"HQ Cards found {HQCards} | Weight: {hqWeight}");
            Console.WriteLine($"NQP Cards found {NQPCards} | Weight: {nqpWeight}");
            Console.WriteLine($"NQ Cards found {NQCards} | Weight: {nqWeight}");

            

            foreach (Card card in cards)
            {
                Console.WriteLine($"{card.Name} sorted");
                switch (card.Rarity)
                {
                    case Rarity.UQ:
                        card.Weight = uqWeight;
                        cumulativeWeight += uqWeight;
                        card.Cumulative = cumulativeWeight;
                        break;
                    case Rarity.HHQ:
                        card.Weight = hhqWeight;
                        cumulativeWeight += hhqWeight;
                        card.Cumulative = cumulativeWeight;
                        break;
                    case Rarity.HQ:
                        card.Weight = hqWeight;
                        cumulativeWeight += hqWeight;
                        card.Cumulative = cumulativeWeight;
                        break;
                    case Rarity.NQP:
                        card.Weight = nqpWeight;
                        cumulativeWeight += nqpWeight;
                        card.Cumulative = cumulativeWeight;
                        break;
                    case Rarity.NQ:
                        card.Weight = nqWeight;
                        cumulativeWeight += nqWeight;
                        card.Cumulative = cumulativeWeight;
                        break;
                }

                Cards.Add(card);
            }

            Console.WriteLine($"{Cards.Count} total cards");
        }

        public static Card FindCard(string name)
        {
            return Instance.Cards.Find(c => c.Name == name);
        }

        public static Card FindCard(int id)
        {
            return Instance.Cards.Find(c => c.Id == id);
        }
    }
}
