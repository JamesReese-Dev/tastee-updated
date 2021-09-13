using System;
using System.Collections.Generic;
using FluentMigrator;

namespace E2.Tastee.Persistence.Migrations
{
    internal static class MigrationHelper
    {
        internal static void LookupTable(this FluentMigrator.Builders.Create.ICreateExpressionRoot Create, 
            string tableName, int nameSize)
        {
            Create.Table(tableName)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(nameSize).NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("UpdatedAt").AsDateTime().NotNullable()
                .WithColumn("DeactivatedAt").AsDateTime().Nullable()
                .WithColumn("DeactivatedById").AsInt32().Nullable();
            Create.ForeignKey(tableName, "Users", "DeactivatedById", $"fk_{tableName}_DeactivatedByUser");
        }

        internal static void SeedSystemManagedLookupTable(this FluentMigrator.Builders.Execute.IExecuteExpressionRoot Execute, 
            string tableName, IList<(int Id, string Name)> seedValues, string now)
        {
            foreach (var seedValue in seedValues)
            {
                Execute.Sql($"insert into {tableName}(Id, Name, CreatedAt, UpdatedAt) values ({seedValue.Id},'{seedValue.Name}','{now}','{now}')");
            }
        }

        internal static void SeedLookupTable(this FluentMigrator.Builders.Execute.IExecuteExpressionRoot Execute,
            string tableName, IList<string> seedValues, string now)
        {
            foreach (var seedValue in seedValues)
            {
                Execute.Sql($"insert into {tableName}(Name, CreatedAt, UpdatedAt) values ('{seedValue}','{now}','{now}')");
            }
        }

        internal static void ForeignKey(this FluentMigrator.Builders.Delete.IDeleteExpressionRoot Delete, 
            string fromTable, string toTable, string explicitKey = null)
        {
            if (String.IsNullOrEmpty(explicitKey))
            {
                Delete.ForeignKey($"fk_{fromTable}_{toTable}").OnTable(fromTable);
            }
            else
            {
                Delete.ForeignKey(explicitKey).OnTable(fromTable);
            }
        }

        internal static void ForeignKey(this FluentMigrator.Builders.Create.ICreateExpressionRoot Create, 
            string fromTable, string toTable, string fromKey, string explicitKey = null)
        {
            if (String.IsNullOrEmpty(explicitKey))
            {
                Create.ForeignKey($"fk_{fromTable}_{toTable}")
                    .FromTable(fromTable).ForeignColumn(fromKey)
                    .ToTable(toTable).PrimaryColumn("Id");
            }
            else
            {
                Create.ForeignKey(explicitKey)
                    .FromTable(fromTable).ForeignColumn(fromKey)
                    .ToTable(toTable).PrimaryColumn("Id");
            }
        }

        internal static void EnumTable(this FluentMigrator.Builders.Create.ICreateExpressionRoot Create,
            string tableName, int nameSize)
        {
            Create.Table(tableName)
                .WithColumn("Id").AsInt32().PrimaryKey()
                .WithColumn("Name").AsString(nameSize).NotNullable();
        }

        internal static void SystemManagedLookupTable(this FluentMigrator.Builders.Create.ICreateExpressionRoot Create, 
            string tableName, int nameSize)
        {
            Create.Table(tableName)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(nameSize).NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithColumn("CreatedByUserId").AsInt32().NotNullable()
                .WithColumn("DeactivatedAt").AsDateTime().Nullable()
                .WithColumn("DeactivatedByUserId").AsInt32().Nullable();
            Create.ForeignKey(tableName, "Users", "CreatedByUserId", $"fk_{tableName}_CreatedByUser");
            Create.ForeignKey(tableName, "Users", "DeactivatedByUserId", $"fk_{tableName}_DeactivatedByUser");
        }
    }
}