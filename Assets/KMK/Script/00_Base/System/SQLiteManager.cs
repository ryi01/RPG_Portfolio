using UnityEngine;
using System.Data.SQLite;
using System.IO;
using System;

public class SQLiteManager : MonoBehaviour
{
    public static SQLiteManager Instance { get; private set; }
    // SQLite연결 객체
    private SQLiteConnection connection;
    // 파일 경로
    private string dbPath;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void InitializeDatabase()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Database");

        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        // 데이터베이스 파일 경로 지정
        dbPath = Path.Combine(folderPath, "rpg_db.db");
        // 해당 경로에 파일이 없으면
        if(!File.Exists(dbPath))
        {
            // 생성
            SQLiteConnection.CreateFile(dbPath);
        }

        connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");

        connection.Open();

        CreateTable();
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
                        ItemId INTEGER NOT NULL,
                        Count INTEGER NOT NULL DEFAULT 0);";

        using (var command = new SQLiteCommand(sql, connection))
        {
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
}
