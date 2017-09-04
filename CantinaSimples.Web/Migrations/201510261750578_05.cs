namespace CantinaSimples.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _05 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemVendas", "Produto_Id", "dbo.Produtoes");
            AddForeignKey("dbo.ItemVendas", "Produto_Id", "dbo.Produtoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemVendas", "Produto_Id", "dbo.Produtoes");
            AddForeignKey("dbo.ItemVendas", "Produto_Id", "dbo.Produtoes", "Id", cascadeDelete: true);
        }
    }
}
