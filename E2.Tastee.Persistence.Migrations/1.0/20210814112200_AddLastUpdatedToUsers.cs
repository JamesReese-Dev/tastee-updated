using System;
using System.Collections.Generic;
using FluentMigrator;

namespace E2.Tastee.Persistence.Migrations.Migrations
{
    [Migration(20210814112200)]
    public class AddLastUpdatedToUsers : Migration
    {
        public override void Up()
        {
            Create.Column("LastUpdatedByUserId").OnTable("Users").AsInt32().Nullable();
            Create.ForeignKey("FK_User_LastUpdatedByUser")
                .FromTable("Users")
                .ForeignColumn("LastUpdatedByUserId")
                .ToTable("Users")
                .PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_User_LastUpdatedByUser").OnTable("Users");
            Delete.Column("LastUpdatedByUserId").FromTable("Users");
        }
    }
}
