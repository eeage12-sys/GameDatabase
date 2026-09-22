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

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
UPDATE Item
SET Price = $price
WHERE ItemId = $itemId;";

                    command.Parameters.AddWithValue("$price", 150);
                    command.Parameters.AddWithValue("$itemId", 4);

                    int changedRows = command.ExecuteNonQuery();

                    Console.WriteLine(
                        changedRows + "건의 가격을 수정했습니다.");
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
SELECT Name, Price
FROM Item
WHERE ItemId = $itemId;";

                    command.Parameters.AddWithValue("$itemId", 4);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine(
                                reader.GetString(0)
                                + " / 가격: "
                                + reader.GetInt64(1));
                        }
                    }
                }
            }
        }
    }
}