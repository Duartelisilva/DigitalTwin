using UnityEngine;
using SQLite4Unity3d;
using System.Linq;
using System;

public class CarDataDatabase : MonoBehaviour
{
    private SQLiteConnection _connection;

    // Start is called before the first frame update
    void Start()
    {
        string dbPath = System.IO.Path.Combine(Application.persistentDataPath, "CarData.db");
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // Create table to store car data if not exists
        _connection.CreateTable<CarData>();

        Debug.Log("Database created at: " + dbPath);
    }

    // Method to save car data to the database
    public void SaveCarData(string carName, Vector3 position, Vector3 speed, float direction)
    {
        var carData = new CarData
        {   
            
            CarName = carName,
            PositionX = position.x,
            PositionY = position.z,
            VelocityX = speed.x,
            VelocityZ = speed.z,
            Direction = direction,
            Timestamp = DateTime.Now
        };

        _connection.Insert(carData);
        //Debug.Log($"Saved {carName} data to the database: Position ({position.x}, {position.z}), Velocity ({velocity.x}, {velocity.z}), Direction {direction}");
    }

    // Optional: Fetch data (if needed)
    public void FetchCarData()
    {
        var allData = _connection.Table<CarData>().ToList();
        foreach (var data in allData)
        {
           // Debug.Log($"{data.CarName}: Position ({data.PositionX}, {data.PositionZ}), Velocity ({data.VelocityX}, {data.VelocityZ}), Direction {data.Direction}");
        }
    }

    // Optional: Clean up when done
    void OnApplicationQuit()
    {
        if (_connection != null)
        {
            _connection.Close();
        }
    }
}

// SQLite table for storing car data
public class CarData
{
    [PrimaryKey, AutoIncrement]
    
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } 
    public string CarName { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float VelocityX { get; set; }
    public float VelocityZ { get; set; }
    public float Direction { get; set; }
    
}
