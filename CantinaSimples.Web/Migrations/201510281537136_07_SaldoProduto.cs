namespace CantinaSimples.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _07_SaldoProduto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Produtoes", "Saldo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Produtoes", "Saldo");
        }
    }
}
