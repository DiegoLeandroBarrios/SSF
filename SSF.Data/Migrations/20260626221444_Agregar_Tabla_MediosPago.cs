using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SSF.Data.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Tabla_MediosPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediosPagos",
                schema: "Facturacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreMedio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediosPagos", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Facturacion",
                table: "MediosPagos",
                columns: new[] { "Id", "Activo", "NombreMedio" },
                values: new object[,]
                {
                    { 1, true, "Efectivo" },
                    { 2, true, "Tarjeta de Débito" },
                    { 3, true, "Tarjeta de Crédito" },
                    { 4, true, "Mercado Pago (QR/Transferencia)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediosPagos",
                schema: "Facturacion");
        }
    }
}
