using HeatDevBLL.Models.Entities;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeatDevBLL.Models
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class MySettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders
        {
            get { yield break; }
        }

        public string DefaultConfiguration => "Npgsql";
        public string DefaultDataProvider => "Npgsql";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "PostgresConfig",
                        ProviderName = "Npgsql",
                        ConnectionString = "User ID=postgres; Password=359741268; Server=localhost; Port=5432; Database=heat_dev; Pooling=true;"
                    };
            }
        }
    }

    public class DBContext : DataConnection
    {
        public ITable<User> Users { get => GetTable<User>(); }
        public ITable<UserProfile> UsersProfiles { get => GetTable<UserProfile>(); }
        public ITable<Order> Orders { get => GetTable<Order>(); }
        public ITable<OrderCategory> OrderCategories { get => GetTable<OrderCategory>(); }
        public ITable<OrderStatus> orderStatuses { get => GetTable<OrderStatus>(); }

        static DBContext()
        {
            DefaultSettings = new MySettings();
            TurnTraceSwitchOn();
            WriteTraceLine = (s1, s2) =>
            {
                Console.WriteLine(s1, s2);
            };
            //LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }
    }

}
