using UnityEngine;
using System.Data.SQLite;
using System.IO;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

public class SQLiteManager : MonoBehaviour
{
    // SQLite연결 객체
    private SQLiteConnection connection;
    // 파일 경로
    private string dbPath;

    #region 초기 설정
    public void InitializeDatabase()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Database");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        // 데이터베이스 파일 경로 지정
        dbPath = Path.Combine(folderPath, "rpg_db.db");
        // 해당 경로에 파일이 없으면
        if (!File.Exists(dbPath))
        {
            // 생성
            SQLiteConnection.CreateFile(dbPath);
        }
        // DB랑 연결된 통로 
        // 게임과 SQLite 파일을 연결하는 선
        connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");

        connection.Open();

        CreateTable();

        if (!HasPlayerData()) InsertDefaultPlayer();
    }

    // 테이블 생성 쿼리
    public void CreateTable()
    {
        CreatePlayerTable();
        CreateInventoryTable();
        CreatePlayerQuestTable();
    }

    private void CreatePlayerTable()
    {
        // user_tb(테이블) 생성 쿼리
        string sql = @"CREATE TABLE IF NOT EXISTS Player(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Level INTEGER NOT NULL DEFAULT 1,
                        Exp INTEGER NOT NULL DEFAULT 0,
                        Gold INTEGER NOT NULL DEFAULT 0,
                        CurrentHP REAL NOT NULL DEFAULT 100);";

        // SQLiteCommand : 명령 
        // using 사용 이유 : 잠깐만 쓰고 정리하기 위함 
        using (var command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    private void CreateInventoryTable()
    {
        // user_tb(테이블) 생성 쿼리
        string sql = @"CREATE TABLE IF NOT EXISTS Inventory(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PlayerId INTEGER NOT NULL,
                        SlotIndex INTEGER NOT NULL,
                        ItemId INTEGER NOT NULL,
                        Count INTEGER NOT NULL DEFAULT 0);";

        using (var command = new SQLiteCommand(sql, connection))
        {
            // ExecuteNonQuery : 조회 결과를 돌려주지 않음
            // 주로 테이블 만들기, isnert, update, delete에 사용됨 
            command.ExecuteNonQuery();
        }
    }

    private void CreatePlayerQuestTable()
    {
        string sql = @"CREATE TABLE IF NOT EXISTS PlayerQuest(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PlayerId INTEGER NOT NULL,
                        QuestId INTEGER NOT NULL,
                        CurrentCount INTEGER NOT NULL DEFAULT 0,
                        IsAccepted INTEGER NOT NULL DEFAULT 0,
                        IsCompleted INTEGER NOT NULL DEFAULT 0,
                        IsRewardClaimed INTEGER NOT NULL DEFAULT 0
                        );";

        using (var command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    #endregion
    #region 플레이어 데이터 로드 / 저장
    public PlayerSaveData LoadPlayer(int playerId = 1)
    {
        // @id = 자리 표시자. id가 들어올 자리다
        // LIMIT = 1개만 찾아라
        string sql = @"SELECT * FROM Player WHERE Id = @id LIMIT 1;";
        using (var command = new SQLiteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", playerId);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new PlayerSaveData
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Level = Convert.ToInt32(reader["Level"]),
                        CurrentExp = Convert.ToInt32(reader["Exp"]),
                        Gold = Convert.ToInt32(reader["Gold"]),
                        CurrentHP = Convert.ToSingle(reader["CurrentHp"])
                    };
                }
            }
        }
        return null;
    }
    public int InsertDefaultPlayer(string name = "Player")
    {
        string sql = @"INSERT INTO Player (Name, Level, Exp, Gold, CurrentHP)
                        VALUES (@name, 1, 0, 100, 100);";

        using (var command = new SQLiteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@name", "Ryi");

            command.ExecuteNonQuery();
        }
        return (int)connection.LastInsertRowId;
    }

    public void UpdatePlayer(PlayerSaveData data)
    {
        string sql = @"UPDATE PLAYER SET Name = @name, Level = @level, Exp = @exp, Gold = @gold, CurrentHp = @hp
                        WHERE Id = @id;";

        using (var command = new SQLiteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", data.Id);
            command.Parameters.AddWithValue("@name", data.Name);
            command.Parameters.AddWithValue("@level", Mathf.Max(1, data.Level));
            command.Parameters.AddWithValue("@exp", Mathf.Max(0, data.CurrentExp));
            command.Parameters.AddWithValue("@gold", Mathf.Max(0, data.Gold));
            command.Parameters.AddWithValue("@hp", Mathf.Max(0, data.CurrentHP));

            command.ExecuteNonQuery();
        }
    }
    public bool HasPlayerData()
    {
        string sql = @"SELECT COUNT(*) FROM Player;";
        using (var command = new SQLiteCommand(sql, connection))
        {
            long count = (long)command.ExecuteScalar();
            return count > 0;
        }
    }
    #endregion
    #region 인벤토리
    public void ClearInventory(int playerId)
    {
        string sql = @"DELETE FROM Inventory WHERE PlayerId = @playerId;";

        using (var command = new SQLiteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@playerId", playerId);
            command.ExecuteNonQuery();
        }
    }

    public List<InventorySaveData> LoadInventory(int playerId)
    {
        List<InventorySaveData> list = new List<InventorySaveData>();

        string sql = @"SELECT * FROM Inventory WHERE PlayerId = @playerId ORDER BY SlotIndex ASC;";
        using (var command = new SQLiteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@playerId", playerId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    InventorySaveData data = new InventorySaveData
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        PlayerId = Convert.ToInt32(reader["PlayerId"]),
                        SlotIndex = Convert.ToInt32(reader["SlotIndex"]),
                        ItemId = Convert.ToInt32(reader["ItemId"]),
                        Count = Convert.ToInt32(reader["Count"])
                    };
                    list.Add(data);
                }
            }
        }
        return list;
    }
    public void InsertInventoryItem(int playerId, int slotIndex, int itemId, int count)
    {
        string sql = @"INSERT INTO Inventory(PlayerId, SlotIndex, ItemId, Count)
                        VALUES(@playerId, @slotIndex, @itemId, @count);";

        using (var command = new SQLiteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@playerId", playerId);
            command.Parameters.AddWithValue("@slotIndex", slotIndex);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.Parameters.AddWithValue("@count", count);

            command.ExecuteNonQuery();
        }
    }
    #endregion
    #region 퀘스트
    public void ClearQuest(int playerId)
    {
        string sql = @"DELETE FROM PlayerQuest WHERE PlayerId = @playerId;";
        using (var command = new SQLiteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@playerId", playerId);
            command.ExecuteNonQuery();
        }
    }
    public List<PlayerQuestSaveData> LoadPlayerQuest(int playerId)
    {
        List<PlayerQuestSaveData> list = new List<PlayerQuestSaveData>();

        string sql = @"SELECT * FROM PlayerQuest WHERE PlayerId = @playerId;";
        using (var command = new SQLiteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@playerId", playerId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PlayerQuestSaveData data = new PlayerQuestSaveData
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        PlayerId = Convert.ToInt32(reader["PlayerId"]),
                        QuestId = Convert.ToInt32(reader["QuestId"]),
                        CurrentCount = Convert.ToInt32(reader["CurrentCount"]),
                        IsAccepted = Convert.ToInt32(reader["IsAccepted"]),
                        IsCompleted = Convert.ToInt32(reader["IsCompleted"]),
                        IsReward = Convert.ToInt32(reader["IsRewardClaimed"])
                    };

                    list.Add(data);
                }
            }
        }
        return list;
    }
    public void SavePlayerQuest(PlayerQuestSaveData data)
    {

    }
    #endregion

    public void ResetInveontoryTable()
    {
        string dropSql = "DROP TABLE IF EXISTS Inventory;";

        using (var dropCommand = new SQLiteCommand(dropSql, connection))
        {
            dropCommand.ExecuteNonQuery();
        }

        CreateInventoryTable();
    }

    public void ResetInventoryTable()
    {
        string deleteSql = "DELETE FROM Inventory;";
        using(var deleteCommand = new SQLiteCommand(deleteSql, connection))
        {
            deleteCommand.ExecuteNonQuery();
        }

        string resetIdSql = "DELETE FROM sqlite_sequence WHERE name='Inventory';";
        using (var resetCommand = new SQLiteCommand(resetIdSql, connection))
        {
            resetCommand.ExecuteNonQuery();
        }
    }
}
