using System;
using System.IO;
using NUnit.Framework;

public class DatabaseHandlerTests
{
    private static string CreateTempDbPath()
    {
        string path = Path.GetTempFileName();
        return path;
    }

    [Test]
    public void ExecuteScalar_ReturnsInsertedValue()
    {
        string path = CreateTempDbPath();
        try
        {
            var db = new DatabaseHandler(path);
            db.ExecuteNonQuery("CREATE TABLE TestTable (Id INTEGER PRIMARY KEY, Value INTEGER);");
            db.ExecuteNonQuery("INSERT INTO TestTable (Value) VALUES (@Value);", ("@Value", 7));

            object result = db.ExecuteScalar("SELECT Value FROM TestTable WHERE Id = 1;");

            Assert.AreEqual(7, Convert.ToInt32(result));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Test]
    public void ExecuteScalar_ThrowsOnInvalidParameterName()
    {
        string path = CreateTempDbPath();
        try
        {
            var db = new DatabaseHandler(path);
            Assert.Throws<ArgumentException>(() => db.ExecuteScalar("SELECT 1;", ("bad", 1)));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
