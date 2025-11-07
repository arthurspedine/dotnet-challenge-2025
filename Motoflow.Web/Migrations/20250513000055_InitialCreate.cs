using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motoflow.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MOTOFLOW_MOTOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Type = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: false),
                    Placa = table.Column<string>(type: "NVARCHAR2(7)", maxLength: 7, nullable: true),
                    Chassi = table.Column<string>(type: "NVARCHAR2(17)", maxLength: 17, nullable: true),
                    QRCode = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTOFLOW_MOTOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MOTOFLOW_PATIOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Localizacao = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTOFLOW_PATIOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MOTOFLOW_AREAS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Identificador = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    PatioId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CapacidadeMaxima = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTOFLOW_AREAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MOTOFLOW_AREAS_MOTOFLOW_PATIOS_PatioId",
                        column: x => x.PatioId,
                        principalTable: "MOTOFLOW_PATIOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MOTOFLOW_HISTORICOS_MOTOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MotoId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    AreaId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DataSaida = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ObservacaoEntrada = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ObservacaoSaida = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTOFLOW_HISTORICOS_MOTOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MOTOFLOW_HISTORICOS_MOTOS_MOTOFLOW_AREAS_AreaId",
                        column: x => x.AreaId,
                        principalTable: "MOTOFLOW_AREAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MOTOFLOW_HISTORICOS_MOTOS_MOTOFLOW_MOTOS_MotoId",
                        column: x => x.MotoId,
                        principalTable: "MOTOFLOW_MOTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MOTOFLOW_AREAS_PatioId",
                table: "MOTOFLOW_AREAS",
                column: "PatioId");

            migrationBuilder.CreateIndex(
                name: "IX_MOTOFLOW_HISTORICOS_MOTOS_AreaId",
                table: "MOTOFLOW_HISTORICOS_MOTOS",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_MOTOFLOW_HISTORICOS_MOTOS_MotoId_DataSaida",
                table: "MOTOFLOW_HISTORICOS_MOTOS",
                columns: new[] { "MotoId", "DataSaida" },
                unique: true,
                filter: "\"DataSaida\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MOTOFLOW_MOTOS_Chassi",
                table: "MOTOFLOW_MOTOS",
                column: "Chassi",
                unique: true,
                filter: "\"Chassi\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MOTOFLOW_MOTOS_Placa",
                table: "MOTOFLOW_MOTOS",
                column: "Placa",
                unique: true,
                filter: "\"Placa\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MOTOFLOW_MOTOS_QRCode",
                table: "MOTOFLOW_MOTOS",
                column: "QRCode",
                unique: true,
                filter: "\"QRCode\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MOTOFLOW_HISTORICOS_MOTOS");

            migrationBuilder.DropTable(
                name: "MOTOFLOW_AREAS");

            migrationBuilder.DropTable(
                name: "MOTOFLOW_MOTOS");

            migrationBuilder.DropTable(
                name: "MOTOFLOW_PATIOS");
        }
    }
}
