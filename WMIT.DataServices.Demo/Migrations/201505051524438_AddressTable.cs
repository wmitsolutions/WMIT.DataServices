namespace WMIT.DataServices.Demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ContactId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId, cascadeDelete: true)
                .Index(t => t.ContactId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "ContactId", "dbo.Contacts");
            DropIndex("dbo.Addresses", new[] { "ContactId" });
            DropTable("dbo.Addresses");
        }
    }
}
