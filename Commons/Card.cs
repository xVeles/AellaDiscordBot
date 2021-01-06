using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AellaDiscordBot.Bots.Commons
{
    public class Card
    {
        public int Id { get; set; }
        public string Filename { get; set; }

        public string Name { get; set; }
        public double Weight { get; set; }
        public double Cumulative { get; set; }
        public int Amount { get; set; }
        public string ImgUrl { get; set; }
        public Rarity Rarity { get; set; }

        public Card()
        {
        }

        public Card(string name, double weight, double cumulative)
        {
            Name = name;
            Weight = weight;
            Cumulative = cumulative;
            Amount = 0;
        }

        public Card(string name, string imgUrl)
        {
            Name = name;
            ImgUrl = imgUrl;
            Weight = 0;
            Cumulative = 0;
            Amount = 0;
        }

        public Card (string name, int amount)
        {
            Name = name;
            Amount = amount;
            Weight = 0;
            Cumulative = 0;
        }

        public Card(int id, string name, string filename, Rarity rarity)
        {
            Id = id;
            Name = name;
            Filename = filename;
            Rarity = rarity;
        }

            public Card(int id, string name, string filename, Rarity rarity, int amount)
        {
            Id = id;
            Name = name;
            Filename = filename;
            Rarity = rarity;
            Amount = amount;
        }
    }
}
