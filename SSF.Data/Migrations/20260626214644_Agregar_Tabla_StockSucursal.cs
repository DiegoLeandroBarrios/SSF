using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSF.Data.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Tabla_StockSucursal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockSucursal",
                schema: "Stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdSucursal = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 0.000m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockSucursal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockSucursal_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalSchema: "Stock",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockSucursal_Sucursales_IdSucursal",
                        column: x => x.IdSucursal,
                        principalSchema: "Seguridad",
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockSucursal_IdProducto_IdSucursal",
                schema: "Stock",
                table: "StockSucursal",
                columns: new[] { "IdProducto", "IdSucursal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockSucursal_IdSucursal",
                schema: "Stock",
                table: "StockSucursal",
                column: "IdSucursal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockSucursal",
                schema: "Stock");
        }
    }
}
