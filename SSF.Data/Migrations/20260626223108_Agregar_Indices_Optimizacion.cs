using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSF.Data.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Indices_Optimizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ventas_IdCaja",
                schema: "Facturacion",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_VentaPagos_IdVenta",
                schema: "Facturacion",
                table: "VentaPagos");

            migrationBuilder.DropIndex(
                name: "IX_VentaDetalles_IdVenta",
                schema: "Facturacion",
                table: "VentaDetalles");

            migrationBuilder.DropIndex(
                name: "IX_StockSucursal_IdSucursal",
                schema: "Stock",
                table: "StockSucursal");

            migrationBuilder.CreateIndex(
                name: "IDX_Ventas_CajaUsuario",
                schema: "Facturacion",
                table: "Ventas",
                columns: new[] { "IdCaja", "IdUsuario" });

            migrationBuilder.CreateIndex(
                name: "IDX_VentaPagos_VentaMedio",
                schema: "Facturacion",
                table: "VentaPagos",
                columns: new[] { "IdVenta", "IdMedioPago" });

            migrationBuilder.CreateIndex(
                name: "IDX_VentaDetalles_VentaProducto",
                schema: "Facturacion",
                table: "VentaDetalles",
                columns: new[] { "IdVenta", "IdProducto" });

            migrationBuilder.CreateIndex(
                name: "IDX_StockSucursal_Combinado",
                schema: "Stock",
                table: "StockSucursal",
                columns: new[] { "IdSucursal", "IdProducto" });

            migrationBuilder.CreateIndex(
                name: "IDX_Productos_CodigoBarra",
                schema: "Stock",
                table: "Productos",
                column: "CodigoBarra",
                filter: "[CodigoBarra] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_Productos_CodigoPLU",
                schema: "Stock",
                table: "Productos",
                column: "CodigoPLU",
                filter: "[CodigoPLU] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_Ventas_CajaUsuario",
                schema: "Facturacion",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IDX_VentaPagos_VentaMedio",
                schema: "Facturacion",
                table: "VentaPagos");

            migrationBuilder.DropIndex(
                name: "IDX_VentaDetalles_VentaProducto",
                schema: "Facturacion",
                table: "VentaDetalles");

            migrationBuilder.DropIndex(
                name: "IDX_StockSucursal_Combinado",
                schema: "Stock",
                table: "StockSucursal");

            migrationBuilder.DropIndex(
                name: "IDX_Productos_CodigoBarra",
                schema: "Stock",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IDX_Productos_CodigoPLU",
                schema: "Stock",
                table: "Productos");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdCaja",
                schema: "Facturacion",
                table: "Ventas",
                column: "IdCaja");

            migrationBuilder.CreateIndex(
                name: "IX_VentaPagos_IdVenta",
                schema: "Facturacion",
                table: "VentaPagos",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_VentaDetalles_IdVenta",
                schema: "Facturacion",
                table: "VentaDetalles",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_StockSucursal_IdSucursal",
                schema: "Stock",
                table: "StockSucursal",
                column: "IdSucursal");
        }
    }
}
