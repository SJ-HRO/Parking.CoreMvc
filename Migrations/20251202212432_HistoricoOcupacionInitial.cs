using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking.CoreMvc.Migrations
{
    /// <inheritdoc />
    public partial class HistoricoOcupacionInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricoOcupaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Momento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlazasTotales = table.Column<int>(type: "int", nullable: false),
                    PlazasOcupadas = table.Column<int>(type: "int", nullable: false),
                    PlazasLibres = table.Column<int>(type: "int", nullable: false),
                    TicketsActivos = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Dia = table.Column<int>(type: "int", nullable: false),
                    Hora = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoOcupaciones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoOcupaciones");
        }
    }
}
