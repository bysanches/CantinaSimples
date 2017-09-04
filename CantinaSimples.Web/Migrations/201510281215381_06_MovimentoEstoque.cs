namespace CantinaSimples.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _06_MovimentoEstoque : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MovimentosEstoque",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantidade = c.Int(nullable: false),
                        Data = c.DateTime(nullable: false),
                        Observacao = c.String(),
                        Produto_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Produtoes", t => t.Produto_Id, cascadeDelete: true)
                .Index(t => t.Produto_Id);
            
            AddColumn("dbo.ItemVendas", "MovimentoEstoque_Id", c => c.Int());
            CreateIndex("dbo.ItemVendas", "MovimentoEstoque_Id");
            AddForeignKey("dbo.ItemVendas", "MovimentoEstoque_Id", "dbo.MovimentosEstoque", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemVendas", "MovimentoEstoque_Id", "dbo.MovimentosEstoque");
            DropForeignKey("dbo.MovimentosEstoque", "Produto_Id", "dbo.Produtoes");
            DropIndex("dbo.MovimentosEstoque", new[] { "Produto_Id" });
            DropIndex("dbo.ItemVendas", new[] { "MovimentoEstoque_Id" });
            DropColumn("dbo.ItemVendas", "MovimentoEstoque_Id");
            DropTable("dbo.MovimentosEstoque");
        }
    }
}
