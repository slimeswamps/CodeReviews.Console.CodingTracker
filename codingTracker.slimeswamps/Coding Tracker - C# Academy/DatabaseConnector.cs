using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker;

class DatabaseConnector
{
    private readonly string connectionString = "DataSource = codingTracker.db";

    internal void CreateTable()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            var command = @"CREATE TABLE IF NOT EXISTS tracker(
                trackerID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                startTime TEXT NOT NULL,
                endTime TEXT NOT NULL,
                duration TEXT NOT NULL,
                date TEXT NOT NULL);";

            connection.Execute(command);
        }
        
    }

    internal List<Records> GetRecords()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            var command = @"SELECT *
                FROM tracker";
            var records = connection.Query<Records>(command).ToList();
            return records;
        }
    }

    internal void AddRecord(string startTime,string endTime,string duration,string date)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            var command = @"INSERT INTO tracker(startTime,endTime,duration,date)
                VALUES (@startTime,@endTime,@duration,@date)";

            connection.Execute(command,new {startTime, endTime, duration, date});
        }
    }

    internal void UpdateRecord(Enums.tableFields fieldToUpdate,int recordToUpdate, string updatedValue)
    {
        switch(fieldToUpdate)
        {
            case Enums.tableFields.startTime:
                using (var connection = new SqliteConnection(connectionString))
                {
                    var command = @"UPDATE tracker
                        SET startTime = @startTime
                        WHERE trackerID = @trackerID";

                    connection.Execute(command, new { trackerID = recordToUpdate, startTime = updatedValue });
                }
                break;

            case Enums.tableFields.endTime:
                using (var connection = new SqliteConnection(connectionString))
                {
                    var command = @"UPDATE tracker
                        SET endTime = @endTime
                        WHERE trackerID = @trackerID";

                    connection.Execute(command, new { trackerID = recordToUpdate, endTime = updatedValue });
                }
                break;

            case Enums.tableFields.duration:
                using(var connection = new SqliteConnection(connectionString))
                {
                    var command = @"UPDATE tracker
                        SET duration = @duration
                        WHERE trackerID = @trackerID";

                    connection.Execute(command, new { trackerID = recordToUpdate, duration = updatedValue });
                }
                break;


            case Enums.tableFields.date:
                using (var connection = new SqliteConnection(connectionString))
                {
                    var command = @"UPDATE tracker
                        SET date = @date
                        WHERE trackerID = @trackerID";

                    connection.Execute(command,new {trackerID = recordToUpdate, date = updatedValue});
                }
                break;
        }
    }

    internal Records GetRecord(int trackerID)
    {
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            var command = @"SELECT *
                FROM tracker
                WHERE trackerID = @trackerID";

            Records value = connection.QuerySingle<Records>(command, new {trackerID = trackerID});
            return value;
        }
    }

    internal void DeleteRecord(int trackerID)
    {
        using(SqliteConnection connection = new SqliteConnection(connectionString))
        {
            var command = @"DELETE FROM tracker
                WHERE trackerID = @trackerID";
            connection.Execute(command, new { trackerID = trackerID });
        }
    }
}

public class Records()
{
    public int? TrackerID {  get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public string? Duration { get; set; }
    public string? Date { get; set; }
}