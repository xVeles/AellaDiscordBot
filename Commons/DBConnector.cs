using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace AellaDiscordBot.Bots.Commons
{
    public class DBConnector
    {

        private string DBLocation { get; set; }
        public static DBConnector Instance { get; private set; }

        public DBConnector(string location)
        {
            DBLocation = location;
            Instance = this;
        }

        public SQLiteConnection DBConnect()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", DBLocation + "/NadekoBot.db"));
            sqliteConnection.Open();
            return sqliteConnection;
        }

        public static List<Card> GetCards()
        {
            List<Card> cardsCollected = new List<Card>();

            Console.WriteLine($"Reading card data...");

            using SQLiteConnection sqliteConnection = Instance.DBConnect();
            using (SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT * FROM Cards", sqliteConnection))
            {
                try
                {
                    using SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
                    while (sqliteDataReader.Read())
                    {
                        int id = Convert.ToInt32(sqliteDataReader["Id"]);
                        string name = sqliteDataReader["Name"].ToString();
                        string filename = sqliteDataReader["Filename"].ToString();
                        Rarity rarity = (Rarity)Enum.Parse(typeof(Rarity), sqliteDataReader["Rarity"].ToString());

                        Card c = new Card(id, name, filename, rarity);

                        Console.WriteLine($"Card {c.Name} found!");

                        cardsCollected.Add(c);
                    }

                    sqliteDataReader.Close();


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex}");
                }
            }

            sqliteConnection.Close();

            return cardsCollected;
        }

        public static List<Card> GetUserCards(string userId)
        {
            List<Card> cardsCollected = new List<Card>();

            using SQLiteConnection sqliteConnection = Instance.DBConnect();
            using (SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT Cards.*, Amount FROM CardsCollected INNER JOIN Cards ON CardsCollected.CardId = Cards.Id WHERE Userid =  @user", sqliteConnection))
            {
                sqliteCommand.Parameters.Add(new SQLiteParameter("@user", userId));
                
                try
                {
                    using SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
                    while (sqliteDataReader.Read())
                    {
                        int id = Convert.ToInt32(sqliteDataReader["Id"]);
                        int amount = Convert.ToInt32(sqliteDataReader["Amount"]);
                        string name = sqliteDataReader["Name"].ToString();
                        string filename = sqliteDataReader["Filename"].ToString();
                        Rarity rarity = (Rarity)Enum.Parse(typeof(Rarity), sqliteDataReader["Rarity"].ToString());
                        Card c = new Card(id, name, filename, rarity, amount);
                        cardsCollected.Add(c);
                        
                    }

                    sqliteDataReader.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex}");
                }
            }

            sqliteConnection.Close();

            return cardsCollected;
        }

        public static bool RemoveCurrency(string userId, int amount)
        {
            bool exists = false;

            using SQLiteConnection sqliteConnection = Instance.DBConnect();
            using (SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT Count(*) AS c FROM DiscordUser WHERE UserId = @user AND CurrencyAmount >= @amount LIMIT 1", sqliteConnection))
            {
                sqliteCommand.Parameters.Add(new SQLiteParameter("@user", userId));
                sqliteCommand.Parameters.Add(new SQLiteParameter("@amount", amount));

                try
                {
                    //Check currency amount of user
                    using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
                    {
                        while (sqliteDataReader.Read())
                        {
                            int count = Convert.ToInt32(sqliteDataReader["c"]);
                            Console.WriteLine(count);

                            if (count == 1) exists = true;
                        }
                        sqliteDataReader.Close();
                    }

                    // If they don't have enough money
                    if (!exists)
                    {
                        sqliteConnection.Close();
                        return false;
                    }
                }
                catch
                {
                    sqliteConnection.Close();

                    return false;
                }
            }

            using (SQLiteCommand sqliteCommand = new SQLiteCommand("UPDATE DiscordUser SET CurrencyAmount = CurrencyAmount - @amount WHERE UserId = @user", sqliteConnection))
            {
                sqliteCommand.Parameters.Add(new SQLiteParameter("@amount", amount));
                sqliteCommand.Parameters.Add(new SQLiteParameter("@user", userId));

                try
                {
                    sqliteCommand.ExecuteNonQuery();

                    sqliteConnection.Close();
                    return true;
                }
                catch
                {
                    sqliteConnection.Close();
                    return false;
                }
            }
        }

        public static bool AddCurrency(string userId, int amount)
        {
            using SQLiteConnection sqliteConnection = Instance.DBConnect();
            using (SQLiteCommand sqliteCommand = new SQLiteCommand("UPDATE DiscordUser SET CurrencyAmount = CurrencyAmount + @amount WHERE UserId = @user", sqliteConnection))
            {
                sqliteCommand.Parameters.Add(new SQLiteParameter("@amount", amount.ToString()));
                sqliteCommand.Parameters.Add(new SQLiteParameter("@user", userId));

                try
                {
                    sqliteCommand.ExecuteNonQuery();

                    sqliteConnection.Close();
                    return true;
                }
                catch
                {
                    sqliteConnection.Close();
                    return false;
                }
            }
        }

        public static void AddCard(string userId, int cardId)
        {
            bool exists = false;

            using SQLiteConnection sqliteConnection = Instance.DBConnect();
            using (SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT Count(*) AS c FROM CardsCollected WHERE UserId = @user AND CardId = @card", sqliteConnection))
            {
                sqliteCommand.Parameters.Add(new SQLiteParameter("@user", userId));
                sqliteCommand.Parameters.Add(new SQLiteParameter("@card", cardId));

                try
                {
                    using SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
                    while (sqliteDataReader.Read())
                    {
                        int count = Convert.ToInt32(sqliteDataReader["c"]);
                        Console.WriteLine(count);

                        if (count == 1) exists = true;
                    }
                    sqliteDataReader.Close();
                }
                catch (Exception ex)
                {
                    exists = false;
                    Console.WriteLine($"{ex}");
                }
                
            }

            if (exists)
            {
                using SQLiteCommand sqliteCommand = new SQLiteCommand("UPDATE CardsCollected SET Amount = Amount + 1 WHERE UserId = @user AND CardId = @card", sqliteConnection);
                sqliteCommand.Parameters.Add(new SQLiteParameter("@user", userId));
                sqliteCommand.Parameters.Add(new SQLiteParameter("@card", cardId));

                try
                {
                    sqliteCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    sqliteConnection.Close();
                    Console.WriteLine($"{ex}");
                }
            }
            else
            {
                using SQLiteCommand sqliteCommand = new SQLiteCommand("INSERT INTO CardsCollected (UserId, CardId, Amount) VALUES (@user, @card, 1)", sqliteConnection);
                sqliteCommand.Parameters.Add(new SQLiteParameter("@user", userId));
                sqliteCommand.Parameters.Add(new SQLiteParameter("@card", cardId));

                try
                {
                    sqliteCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    sqliteConnection.Close();
                    Console.WriteLine($"{ex}");
                }

            }

            sqliteConnection.Close();
        }
    }

}
