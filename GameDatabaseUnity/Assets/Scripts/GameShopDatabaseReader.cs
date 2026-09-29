using System;
using System.Collections.Generic;
using SQLite;
using TMPro;
using UnityEngine;

public class ItemRow
{
    public int ItemId { get; set; }
    public string Name { get; set; } = "";
    public int Price { get; set; }
}

public class GameShopDatabaseReader : MonoBehaviour
{
    [SerializeField] private SQLiteAsset databaseAsset;
    [SerializeField] private TMP_Text itemText;

    private void Start()
    {
        if (databaseAsset == null)
        {
            Debug.LogError("Database Asset이 연결되지 않았습니다.");
            return;
        }

        if (itemText == null)
        {
            Debug.LogError("Item Text가 연결되지 않았습니다.");
            return;
        }

        try
        {
            using (SQLiteConnection connection =
                   databaseAsset.CreateConnection())
            {
                List<ItemRow> items = connection.Query<ItemRow>(
                    "SELECT ItemId, Name, Price FROM Item ORDER BY ItemId;");

                if (items.Count == 0)
                {
                    itemText.text = "읽을 아이템이 없습니다.";
                    Debug.Log("Item 테이블이 비어 있습니다.");
                    return;
                }

                ItemRow firstItem = items[0];

                itemText.text =
                    firstItem.Name +
                    " / 가격: " +
                    firstItem.Price;

                Debug.Log(
                    "아이템: " +
                    firstItem.Name +
                    " / 가격: " +
                    firstItem.Price);
            }
        }
        catch (Exception exception)
        {
            itemText.text = "DB 읽기 실패";

            Debug.LogError(
                "DB 읽기 실패: " +
                exception.Message);
        }
    }
}