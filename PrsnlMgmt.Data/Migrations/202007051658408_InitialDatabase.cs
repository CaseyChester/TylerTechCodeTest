namespace PrsnlMgmt.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Employee",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.String(nullable: false, maxLength: 10),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Manager_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employee", t => t.Manager_Id)
                .Index(t => t.EmployeeId, unique: true)
                .Index(t => t.Manager_Id);
            
            CreateTable(
                "dbo.EmployeeEmployeeRole",
                c => new
                    {
                        Employee_Id = c.Int(nullable: false),
                        EmployeeRole_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Employee_Id, t.EmployeeRole_Id })
                .ForeignKey("dbo.Employee", t => t.Employee_Id, cascadeDelete: true)
                .ForeignKey("dbo.EmployeeRole", t => t.EmployeeRole_Id, cascadeDelete: true)
                .Index(t => t.Employee_Id)
                .Index(t => t.EmployeeRole_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeEmployeeRole", "EmployeeRole_Id", "dbo.EmployeeRole");
            DropForeignKey("dbo.EmployeeEmployeeRole", "Employee_Id", "dbo.Employee");
            DropForeignKey("dbo.Employee", "Manager_Id", "dbo.Employee");
            DropIndex("dbo.EmployeeEmployeeRole", new[] { "EmployeeRole_Id" });
            DropIndex("dbo.EmployeeEmployeeRole", new[] { "Employee_Id" });
            DropIndex("dbo.Employee", new[] { "Manager_Id" });
            DropIndex("dbo.Employee", new[] { "EmployeeId" });
            DropIndex("dbo.EmployeeRole", new[] { "Name" });
            DropTable("dbo.EmployeeEmployeeRole");
            DropTable("dbo.Employee");
            DropTable("dbo.EmployeeRole");
        }
    }
}
