namespace InternalRewrite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AspNetUsers", newName: "User");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.User", newName: "AspNetUsers");
        }
    }
}
