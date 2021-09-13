using FluentMigrator;

namespace E2.Tastee.Persistence.Migrations.Migrations
{
    [Migration(20210913162654)]
    public class AddBloodTypeToUsers : Migration
    {
        public override void Up()
        {
            Create.Column("UserBloodType").OnTable("Users").AsString(15).Nullable();
        }

        public override void Down()
        {
            Delete.Column("UserBloodType").FromTable("Users");
        }
    }
}
