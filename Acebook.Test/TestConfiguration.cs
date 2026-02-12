using Microsoft.Extensions.Configuration;

namespace Acebook.Test;

public static class TestConfiguration
{
    public static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: false)
            .Build();
    }

    public static string GetConnectionString()
    {
        return GetConfiguration().GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found");
    }
}