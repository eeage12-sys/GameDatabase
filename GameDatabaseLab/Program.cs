using System;
using Microsoft.Data.Sqlite;

namespace GameDatabaseLab
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (SqliteConnection connection =
                   new SqliteConnection("Data Source=GameShop.db"))
            {
                connection.Open();

                using (SqliteCommand pragma = connection.CreateCommand())
                {
                    pragma.CommandText = "PRAGMA foreign_keys = ON;";
                    pragma.ExecuteNonQuery();
                }

                try
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
INSERT INTO Inventory (PlayerId, ItemId, Quantity)
VALUES ($playerId, $itemId, $quantity);";

                        command.Parameters.AddWithValue("$playerId", 1);
                        command.Parameters.AddWithValue("$itemId", 999);
                        command.Parameters.AddWithValue("$quantity", 1);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SqliteException e)
                {
                    Console.WriteLine("등록 거부됨");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}