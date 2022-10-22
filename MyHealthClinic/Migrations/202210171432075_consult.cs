namespace MyHealthClinic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class consult : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Consults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        AvailableTime_Id = c.Int(nullable: false),
                        Patient_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AvailableTimes", t => t.AvailableTime_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Patient_Id, cascadeDelete: false)
                .Index(t => t.AvailableTime_Id)
                .Index(t => t.Patient_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Consults", "Patient_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Consults", "AvailableTime_Id", "dbo.AvailableTimes");
            DropIndex("dbo.Consults", new[] { "Patient_Id" });
            DropIndex("dbo.Consults", new[] { "AvailableTime_Id" });
            DropTable("dbo.Consults");
        }
    }
}
