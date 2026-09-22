using System;
using Microsoft.Data.Sqlite;

namespace GameDatabaseLab
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const int playerId = 1;
            const int itemId = 1;
            const int unitPrice = 30;

            Console.Write("구매할 포션 수량을 입력하세요: ");

            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("숫자를 입력해야 합니다.");
                return;
            }

            if (quantity <= 0)
            {
                Console.WriteLine("구매 수량은 1개 이상이어야 합니다.");
                return;
            }

            int totalPrice = unitPrice * quantity;

            using (SqliteConnection connection =
                   new SqliteConnection("Data Source=GameShop.db"))
            {
                connection.Open();

                using (SqliteCommand pragma = connection.CreateCommand())
                {
                    pragma.CommandText = "PRAGMA foreign_keys = ON;";
                    pragma.ExecuteNonQuery();
                }

                using (SqliteTransaction transaction =
                       connection.BeginTransaction())
                {
                    try
                    {
                        // 1. 골드 차감
                        using (SqliteCommand spendGold =
                               connection.CreateCommand())
                        {
                            spendGold.Transaction = transaction;

                            spendGold.CommandText = @"
UPDATE Player
SET Gold = Gold - $totalPrice
WHERE PlayerId = $playerId
AND Gold >= $totalPrice;";

                            spendGold.Parameters.AddWithValue(
                                "$totalPrice", totalPrice);

                            spendGold.Parameters.AddWithValue(
                                "$playerId", playerId);

                            if (spendGold.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException(
                                    "골드가 부족합니다.");
                            }
                        }

                        // 2. 인벤토리 수량 증가
                        using (SqliteCommand addPotion =
                               connection.CreateCommand())
                        {
                            addPotion.Transaction = transaction;

                            addPotion.CommandText = @"
INSERT INTO Inventory (PlayerId, ItemId, Quantity)
VALUES ($playerId, $itemId, $quantity)
ON CONFLICT (PlayerId, ItemId)
DO UPDATE SET Quantity = Quantity + $quantity;";

                            addPotion.Parameters.AddWithValue(
                                "$playerId", playerId);

                            addPotion.Parameters.AddWithValue(
                                "$itemId", itemId);

                            addPotion.Parameters.AddWithValue(
                                "$quantity", quantity);

                            if (addPotion.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException(
                                    "포션 구매 처리에 실패했습니다.");
                            }
                        }

                        transaction.Commit();

                        Console.WriteLine(
                            quantity + "개 구매를 완료했습니다.");

                        Console.WriteLine(
                            "총 가격: " + totalPrice + " Gold");
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();

                        Console.WriteLine(
                            "구매를 취소했습니다: "
                            + exception.Message);
                    }
                }
            }
        }
    }
}