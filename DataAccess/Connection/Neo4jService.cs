using Neo4j.Driver;

namespace DataAccess.Connection
{
    public class Neo4jService : IAsyncDisposable
    {
        private readonly IDriver _driver;

        public Neo4jService(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }
        public IDriver Driver
        {
            get { return _driver; }
        }

        public async ValueTask DisposeAsync()
        {
            await _driver.DisposeAsync();
        }
    }
}
