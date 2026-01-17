using FluentMigrator;

namespace BankingServiceProject.InfrastructureLayerProject.Postgres.Migrations;

[Migration(2026011002)]
public class CreateOperationsTable : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            CREATE TABLE operations
            (
                id VARCHAR(36) PRIMARY KEY,
                idempotency_key VARCHAR(36) UNIQUE NOT NULL,
                metainfo JSONB NOT NULL DEFAULT '{}',
                payment_url VARCHAR(255) NOT NULL,
                amount DECIMAL(10,2) NOT NULL CHECK (amount > 0),
                status varchar(32) DEFAULT 'CREATED',
                banking_provider VARCHAR(32) NOT NULL,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

                CONSTRAINT fk_operation_status
                    FOREIGN KEY (status) references operation_statuses (name),

                CONSTRAINT fk_banking_provider
                    FOREIGN KEY (banking_provider) references banking_providers (name)
            );
            """);

        Execute.Sql(
            """
            CREATE OR REPLACE FUNCTION update_updated_at_column()
            RETURNS TRIGGER AS $$
            BEGIN
                NEW.updated_at = CURRENT_TIMESTAMP;
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
            """);

        Execute.Sql(
            """
            CREATE TRIGGER set_timestamp
            BEFORE UPDATE ON operations
            FOR EACH ROW
            EXECUTE FUNCTION update_updated_at_column();
            """);
    }

    public override void Down()
    {
        Execute.Sql("DROP TRIGGER IF EXISTS set_timestamp ON operations;");
        Execute.Sql("DROP FUNCTION IF EXISTS update_updated_at_column;");
        Execute.Sql("DROP TABLE IF EXISTS operations;");
    }
}