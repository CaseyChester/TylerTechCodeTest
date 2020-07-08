namespace PrsnlMgmt.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEmployeeManagerId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Employee", name: "Manager_Id", newName: "ManagerId");
            RenameIndex(table: "dbo.Employee", name: "IX_Manager_Id", newName: "IX_ManagerId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Employee", name: "IX_ManagerId", newName: "IX_Manager_Id");
            RenameColumn(table: "dbo.Employee", name: "ManagerId", newName: "Manager_Id");
        }
    }
}
