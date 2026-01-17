using FluentMigrator;

namespace BankingServiceProject.InfrastructureLayerProject.Postgres.Migrations;

[Migration(2026011001)]
public class CreateBankingProviderType : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            CREATE TABLE banking_providers
            (
                name VARCHAR(32) PRIMARY KEY,
                api_base_url VARCHAR(255)
            );
            """);

        Execute.Sql(
            """
            INSERT INTO banking_providers (name, api_base_url)
            VALUES ('MOCK', 'http://localhost:8081');
            """);
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS banking_providers;");
    }
}