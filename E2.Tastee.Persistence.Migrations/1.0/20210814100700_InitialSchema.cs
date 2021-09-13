using System;
using System.Collections.Generic;
using FluentMigrator;

namespace E2.Tastee.Persistence.Migrations.Migrations
{
    [Migration(20210814100700)]
    public class InitialSchema : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("FirstName").AsString(30).NotNullable()
                .WithColumn("LastName").AsString(30).NotNullable()
                .WithColumn("Username").AsString(50).NotNullable()
                .WithColumn("Email").AsString(50).NotNullable()
                .WithColumn("MobilePhoneNumber").AsAnsiString(15).Nullable()
                .WithColumn("Timezone").AsAnsiString(150).Nullable()
                .WithColumn("HashedPassword").AsString(64).NotNullable()
                .WithColumn("PasswordSalt").AsString(64).NotNullable()
                .WithColumn("TwoFactorAuthCode").AsString(6).Nullable()
                .WithColumn("TwoFactorAuthCodeExpiresAt").AsDateTime().Nullable()
                .WithColumn("ResetPasswordToken").AsGuid().Nullable()
                .WithColumn("ResetPasswordRequestedAt").AsDateTime().Nullable()
                .WithColumn("ResetPasswordExpiresAt").AsDateTime().Nullable()
                .WithColumn("LastLockedOutAt").AsDateTime().Nullable()
                .WithColumn("MustChangePassword").AsBoolean().Nullable()
                .WithColumn("FailedAttemptCount").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("PasswordLastChangedAt").AsDateTime().Nullable()
                .WithColumn("CreatedByUserId").AsInt32().Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable()
                .WithColumn("DeactivatedAt").AsDateTime().Nullable()
                .WithColumn("DeactivatedByUserId").AsInt32().Nullable()
                .WithColumn("LastLoggedOnAt").AsDateTime().Nullable();
            // resource user
            Execute.Sql(@"SET IDENTITY_INSERT Users ON;
INSERT INTO [dbo].[Users] (Id, [FirstName],[LastName],[Username],[Email],[MobilePhoneNumber],Timezone,[PasswordSalt],[HashedPassword],[CreatedAt],[UpdatedAt],
	[FailedAttemptCount],[MustChangePassword])
VALUES(-1, 'Resource', 'Account', 'resource@account.com', 'resource@account.com', '5555555555', 'Eastern Standard Time', '3cb58457f059b4c4600673f7ba5039d1cc8a0d474436d09973c69a3d3450cf5a', 
	'34d2814971ab270bac159fd052f5f53b4bc9268d39146783e7ee8e0dd98d3a8f', GETUTCDATE(), GETUTCDATE(), 0, 0);
SET IDENTITY_INSERT Users OFF;
UPDATE dbo.Users SET CreatedByUserId = -1 WHERE Id = -1;");
            Create.ForeignKey("Users", "Users", "CreatedByUserId", "fk_Users_CreatedByUser");
            Create.ForeignKey("Users", "Users", "DeactivatedByUserId", "fk_Users_DeactivatedByUser");
            Execute.Sql(@"ALTER TABLE dbo.Users ALTER COLUMN CreatedByUserId int NOT NULL");

            Create.EnumTable("Roles", 100);
            Execute.Sql(@"INSERT INTO dbo.Roles (Id, Name) VALUES (1, 'Administrator'), (2, 'Taster')");

            Create.Table("UserRoles")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable()
                .WithColumn("RoleId").AsInt32().NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("CreatedByUserId").AsInt32().NotNullable()
                .WithColumn("DeactivatedAt").AsDateTime().Nullable()
                .WithColumn("DeactivatedByUserId").AsInt32().Nullable()
                ;
            Create.ForeignKey("FK_UserRoles_Role")
                .FromTable("UserRoles")
                .ForeignColumn("RoleId")
                .ToTable("Roles")
                .PrimaryColumn("Id");
            Create.ForeignKey("FK_UserRoles_User")
                .FromTable("UserRoles")
                .ForeignColumn("UserId")
                .ToTable("Users")
                .PrimaryColumn("Id");
            Create.ForeignKey("FK_UserRoles_CreatedByUser")
                .FromTable("UserRoles")
                .ForeignColumn("CreatedByUserId")
                .ToTable("Users")
                .PrimaryColumn("Id");
            Create.ForeignKey("FK_UserRoles_DeactivatedByUser")
                .FromTable("UserRoles")
                .ForeignColumn("DeactivatedByUserId")
                .ToTable("Users")
                .PrimaryColumn("Id");
            Execute.Sql(@"CREATE UNIQUE NONCLUSTERED INDEX [UQ_UserRoles] ON dbo.UserRoles (
	            UserId, RoleId, [DeactivatedAt])");

            // seed admin user, Password1
            Execute.Sql(@"INSERT INTO [dbo].[Users] ([FirstName],[LastName],[Username],[Email],[MobilePhoneNumber],Timezone,[PasswordSalt],[HashedPassword],[CreatedAt],[UpdatedAt],
	[FailedAttemptCount],[MustChangePassword], CreatedByUserId)
VALUES('John', 'Smith', 'john', 'jsmith@sightsource.net', '3367824684', 'Eastern Standard Time', 'f72989bf8f9bf84928a33287d4978b2a5b4d5cfd164143eef18a1ef3f7c71f57',
	'7eb11b1d95057ee5f841e62d9eaa702a413fcd14acd61817499ee11d6e53c14f', GETUTCDATE(), GETUTCDATE(), 0, 1, -1)");

            Execute.Sql(@"insert into dbo.UserRoles (UserId, RoleId, CreatedAt, CreatedByUserId) 
select (select min(x.id) from users x where username = 'john'), 
    (select min(x.id) from roles x where x.name = 'Administrator'), getutcdate(), -1");

            Create.Table("UserAuthenticationAttempts")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Username").AsAnsiString(50).NotNullable()
                .WithColumn("IP").AsBinary(16).NotNullable()
                .WithColumn("OccurredAt").AsDateTime().NotNullable()
                .WithColumn("WasSuccessful").AsBoolean().NotNullable()
                .WithColumn("FailureReason").AsAnsiString(200).Nullable();
        }

        public override void Down()
        {
            Delete.Table("UserAuthenticationAttempts");
            Execute.Sql("DROP INDEX dbo.UserRoles.UQ_UserRoles");
            Delete.ForeignKey("FK_UserRoles_Role").OnTable("UserRoles");
            Delete.ForeignKey("FK_UserRoles_User").OnTable("UserRoles");
            Delete.ForeignKey("FK_UserRoles_CreatedByUser").OnTable("UserRoles");
            Delete.ForeignKey("FK_UserRoles_DeactivatedByUser").OnTable("UserRoles");
            Delete.Table("UserRoles");
            Delete.Table("Roles");
            Delete.ForeignKey("fk_Users_CreatedByUser").OnTable("Users");
            Delete.ForeignKey("fk_Users_DeactivatedByUser").OnTable("Users");
            Delete.Table("Users");
        }
    }
}
