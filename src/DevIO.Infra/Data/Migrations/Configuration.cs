using DevIO.Infra.Data.Context;
using System.Data.Entity.Migrations;

namespace DevIO.Infra.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MeuDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MeuDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
