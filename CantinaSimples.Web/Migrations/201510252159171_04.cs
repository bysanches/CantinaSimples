namespace CantinaSimples.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _04 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Vendas", "Cliente_Id", "dbo.Clientes");
            DropIndex("dbo.Vendas", new[] { "Cliente_Id" });
            AlterColumn("dbo.Vendas", "Cliente_Id", c => c.Int());
            CreateIndex("dbo.Vendas", "Cliente_Id");
            AddForeignKey("dbo.Vendas", "Cliente_Id", "dbo.Clientes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vendas", "Cliente_Id", "dbo.Clientes");
            DropIndex("dbo.Vendas", new[] { "Cliente_Id" });
            AlterColumn("dbo.Vendas", "Cliente_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Vendas", "Cliente_Id");
            AddForeignKey("dbo.Vendas", "Cliente_Id", "dbo.Clientes", "Id", cascadeDelete: true);
        }
    }
}
