using Meadow;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using TankLevelMonitor_Demo.SQLite.Models;

namespace TankLevelMonitor_Demo.SQLite.Database
{
    public class DatabaseManager
    {
        private static readonly Lazy<DatabaseManager> instance =
            new(() => new DatabaseManager());
        public static DatabaseManager Instance => instance.Value;

        bool isConfigured = false;

        SQLiteConnection Database { get; set; }

        private DatabaseManager()
        {
            Initialize();
        }

        protected void Initialize()
        {
            var databasePath = Path.Combine(MeadowOS.FileSystem.DataDirectory, "TankLevelReadings.db");
            Database = new SQLiteConnection(databasePath);

            Database.DropTable<TankLevelReading>(); //convenience while we work on the model object
            Database.CreateTable<TankLevelReading>();
            isConfigured = true;
        }

        public bool SaveReading(TankLevelReading level)
        {
            if (isConfigured == false)
            {
                Console.WriteLine("SaveUpdateReading: DB not ready");
                return false;
            }

            if (level == null)
            {
                Console.WriteLine("SaveUpdateReading: Conditions is null");
                return false;
            }

            Console.WriteLine("Saving tank level reading to DB");

            Database.Insert(level);

            Console.WriteLine($"Successfully saved to database");

            return true;
        }

        public TankLevelReading GetTankLevelReading(int id)
        {
            return Database.Get<TankLevelReading>(id);
        }

        public List<TankLevelReading> GetAllLevelReadings()
        {
            return Database.Table<TankLevelReading>().ToList();
        }
    }
}