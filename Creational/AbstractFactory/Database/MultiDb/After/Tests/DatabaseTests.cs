using NUnit.Framework;
using DatabaseFactory.After.Context;

namespace DatabaseFactory.After.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        [Test]
        public void MySqlFactory_CreateConnection()
        {
            var factory = new MySqlFactory();
            var connection = factory.CreateConnection("localhost");
            Assert.That(connection.GetConnectionString(), Is.EqualTo("localhost"));
        }

        [Test]
        public void PostgresFactory_CreateCommand()
        {
            var factory = new PostgresFactory();
            var command = factory.CreateCommand();
            Assert.IsNotNull(command);
        }

        [Test]
        public void SqlServerFactory_CreateDataReader()
        {
            var factory = new SqlServerFactory();
            var reader = factory.CreateDataReader();
            Assert.IsNotNull(reader);
        }

        [Test]
        public void ProviderReturnsCorrectFactory_MySql()
        {
            var factory = DatabaseFactoryProvider.GetFactory("mysql");
            Assert.That(factory, Is.InstanceOf<MySqlFactory>());
        }

        [Test]
        public void ProviderReturnsCorrectFactory_Postgres()
        {
            var factory = DatabaseFactoryProvider.GetFactory("postgres");
            Assert.That(factory, Is.InstanceOf<PostgresFactory>());
        }

        [Test]
        public void DataReader_ReturnsData()
        {
            var factory = new MySqlFactory();
            var reader = factory.CreateDataReader();
            var data = reader.Read();
            Assert.That(data.Count, Is.GreaterThan(0));
        }

        [Test]
        public void DatabaseAccessLayer_ExecutesQuery()
        {
            var factory = new PostgresFactory();
            var dal = new DatabaseAccessLayer(factory, "localhost");
            dal.ExecuteQuery("SELECT * FROM users");
            Assert.Pass();
        }
    }
}
