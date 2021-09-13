using System;
using System.Collections.Generic;
using FluentMigrator;

namespace E2.Tastee.Persistence.Migrations.Migrations
{
    [Migration(20210913112900)]
    public class AddZipCodeToUsers : Migration
    {
        public override void Up()
        {
            Create.Column("ZipCode").OnTable("Users").AsInt32().Nullable();
        }

        public override void Down()
        {
            
            Delete.Column("ZipCode").FromTable("Users");
        }
    }
}
