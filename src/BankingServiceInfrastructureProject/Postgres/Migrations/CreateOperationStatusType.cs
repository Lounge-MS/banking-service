using FluentMigrator;

namespace BankingServiceProject.InfrastructureLayerProject.Postgres.Migrations;

[Migration(2026011000)]
public class CreateOperationStatusType : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            CREATE TABLE operation_statuses
            (
                name VARCHAR(32) PRIMARY KEY
            );
            """);

        Execute.Sql(
            """
            INSERT INTO operation_statuses (name)
            VALUES
                ('CREATED'),
                ('COMPLETED'),
                ('CANCELLED'),
                ('COMPENSATED');
            """);
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS operation_statuses;");
    }
}