namespace CantinaSimples.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clientes", "Saldo", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0, defaultValueSql: "0"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clientes", "Saldo");
        }
    }
}
