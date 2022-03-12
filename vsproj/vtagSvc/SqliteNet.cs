using SQLite;
using System.Reflection;

namespace vtagSvc
{
    public class SqliteNet
    {
        public readonly SQLiteAsyncConnection Conn;

        public SqliteNet(string dbFilePath)
        {
            Conn = new SQLiteAsyncConnection(dbFilePath, storeDateTimeAsTicks: true);

            // code first entities
            var tables = from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.IsDefined(typeof(SqliteTableAttribute))
                            && t.IsClass
                         select t;

            using (var sync = Conn.GetConnection())
            {
                // initial database configuration
                sync.ExecuteScalar<string>($"PRAGMA journal_mode = WAL"); // to improve performace of trasaction for massive insertion
                sync.ExecuteScalar<int>($"PRAGMA fullfsync = ON"); // https://www.sqlite.org/atomiccommit.html#_incomplete_disk_flushes
                sync.ExecuteScalar<int>($"PRAGMA synchronous = EXTRA"); // for extra durability than FULL

                // update schema
                sync.CreateTables(CreateFlags.None, tables.ToArray());
            }
        }

        #region tables

        [AttributeUsage(AttributeTargets.Class)]
        public class SqliteTableAttribute : Attribute
        { }

        #region filesystem

        [SqliteTable]
        public class File
        {
            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }

            [Indexed]
            public string Name { get; set; } = Guid.NewGuid().ToString();

            public long Size { get; set; } = 0;

            [Indexed]
            public string Sha256 { get; set; } = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"; // SHA256 for empty contents

            public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow; // index 없이, 목록을 가져가서 정렬하는 방식 사용할 것
            public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
            public byte[] Content { get; set; } = Array.Empty<byte>();
        }

        [SqliteTable]
        public class Tag // directory like (grouping)
        {
            public const char SEPARATOR = '/';

            [PrimaryKey, AutoIncrement]
            public long Id { get; set; }

            [Indexed]
            public string Name { get; set; } = Guid.NewGuid().ToString();

            public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;
            public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

            public bool IsDirectory => Name?.StartsWith(SEPARATOR) ?? false;
        }

        [SqliteTable]
        public class FileOnTag
        {
            // TOOD: TagId-FileId 를 composite primary key 로

            [Indexed]
            public long TagId { get; set; }

            [Indexed]
            public long FileId { get; set; }
        }

        #endregion filesystem

        #endregion tables
    }
}