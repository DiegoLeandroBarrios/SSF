using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSF.Data.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Tabla_LogsAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogsAuditoria",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdSucursal = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpDireccion = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAuditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsAuditoria_Sucursales_IdSucursal",
                        column: x => x.IdSucursal,
                        principalSchema: "Seguridad",
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogsAuditoria_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "Seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogsAuditoria_IdSucursal",
                schema: "Seguridad",
                table: "LogsAuditoria",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAuditoria_IdUsuario",
                schema: "Seguridad",
                table: "LogsAuditoria",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogsAuditoria",
                schema: "Seguridad");
        }
    }
}
