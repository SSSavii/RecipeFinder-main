using Microsoft.Extensions.Configuration;
using Neo4j.Driver;


namespace RecipeFinder.Infrastructure.Database
{
    using Neo4j.Driver;
    using System;

    public class Neo4jContext
    {
        public IDriver Driver { get; }
        public Neo4jContext(IConfiguration configuration)
        {
            var settings = configuration.GetSection("Neo4j").Get<Neo4jSettings>();
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var driver = GraphDatabase.Driver(settings.Uri, AuthTokens.Basic(settings.User, settings.Password));

        }
    }
}
