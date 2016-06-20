namespace SoldiersInfo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Soldiers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        lastName = c.String(nullable: false),
                        middleName = c.String(),
                        firstName = c.String(nullable: false),
                        birthday = c.DateTime(nullable: false),
                        company = c.String(nullable: false),
                        servingDate = c.DateTime(nullable: false),
                        pointDate = c.DateTime(nullable: false),
                        note = c.String(),
                        annouce = c.Int(nullable: false),
                        isDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Soldiers");
        }
    }
}
