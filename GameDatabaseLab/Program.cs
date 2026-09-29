using System;
using LiteDB;

namespace GameDatabaseLab
{
    public class GameLog
    {
        public int Id { get; set; }
        public string EventType { get; set; } = "";
        public int PlayerId { get; set; }
        public string Message { get; set; } = "";
        public DateTime OccurredAt { get; set; }
        public string ClientVersion { get; set; } = "";
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            using (LiteDatabase db = new LiteDatabase("GameLogs.db"))
            {
                ILiteCollection<GameLog> logs =
                    db.GetCollection<GameLog>("logs");

                int playerId;

                while (true)
                {
                    Console.Write("조회할 PlayerId를 입력하세요: ");

                    string input = Console.ReadLine() ?? "";

                    if (int.TryParse(input, out playerId))
                    {
                        break;
                    }

                    Console.WriteLine("숫자를 입력해주세요.");
                }

                Console.WriteLine();
                Console.WriteLine("플레이어 " + playerId + "의 로그");

                bool found = false;

                foreach (GameLog log in
                         logs.Find(x => x.PlayerId == playerId))
                {
                    found = true;

                    Console.WriteLine(
                        log.Id + " / "
                        + log.EventType + " / "
                        + log.Message + " / "
                        + log.ClientVersion);
                }

                if (!found)
                {
                    Console.WriteLine("해당 플레이어의 로그가 없습니다.");
                }
            }
        }
    }
}