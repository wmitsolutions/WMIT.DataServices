namespace WMIT.DataServices.Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contacts", "CreatedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Contacts", "ModifiedAt", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Addresses", "CreatedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Addresses", "ModifiedAt", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "ModifiedAt", c => c.DateTime());
            AlterColumn("dbo.Addresses", "CreatedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Contacts", "ModifiedAt", c => c.DateTime());
            AlterColumn("dbo.Contacts", "CreatedAt", c => c.DateTime(nullable: false));
        }
    }
}
