using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Persistence.Migrations._1._0
{
    [Migration(20210913123100)]
    public class AddMothersMaidenNameToUser : Migration
    {
        public override void Up()
        {
            Create.Column("MothersMaidenName").OnTable("Users").AsString(80).Nullable();
        }
        public override void Down()
        {
            Delete.Column("MothersMaidenName").FromTable("Users");
        }

    }
}
