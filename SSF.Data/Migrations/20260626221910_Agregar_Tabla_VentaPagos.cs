using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSF.Data.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Tabla_VentaPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VentaPagos",
                schema: "Facturacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    IdMedioPago = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NroReferencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaPagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentaPagos_MediosPagos_IdMedioPago",
                        column: x => x.IdMedioPago,
                        principalSchema: "Facturacion",
                        principalTable: "MediosPagos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VentaPagos_Ventas_IdVenta",
                        column: x => x.IdVenta,
                        principalSchema: "Facturacion",
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VentaPagos_IdMedioPago",
                schema: "Facturacion",
                table: "VentaPagos",
                column: "IdMedioPago");

            migrationBuilder.CreateIndex(
                name: "IX_VentaPagos_IdVenta",
                schema: "Facturacion",
                table: "VentaPagos",
                column: "IdVenta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VentaPagos",
                schema: "Facturacion");
        }
    }
}
