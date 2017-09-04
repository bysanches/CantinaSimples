namespace CantinaSimples.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _03 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recargas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Data = c.DateTime(nullable: false),
                        Cliente_Id = c.Int(),
                        Usuario_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clientes", t => t.Cliente_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Usuario_Id)
                .Index(t => t.Cliente_Id)
                .Index(t => t.Usuario_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recargas", "Usuario_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Recargas", "Cliente_Id", "dbo.Clientes");
            DropIndex("dbo.Recargas", new[] { "Usuario_Id" });
            DropIndex("dbo.Recargas", new[] { "Cliente_Id" });
            DropTable("dbo.Recargas");
        }
    }
}
