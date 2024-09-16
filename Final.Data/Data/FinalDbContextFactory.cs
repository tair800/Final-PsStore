//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

//namespace Final.Data.Data
//{
//    public class FinalDbContextFactory : IDesignTimeDbContextFactory<FinalDbContext>
//    {
//        public FinalDbContext CreateDbContext(string[] args)
//        {
//            // Create a new configuration builder and load the appsettings.json
//            var configuration = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json")
//                .Build();

//            var builder = new DbContextOptionsBuilder<FinalDbContext>();
//            var connectionString = configuration.GetConnectionString("DefaultConnection");

//            // Use SQL Server provider
//            builder.UseSqlServer(connectionString);

//            return new FinalDbContext(builder.Options);
//        }
//    }
//}
