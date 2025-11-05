using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motoflow.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_USERS",
                columns: table => new
                {
                    USER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERNAME = table.Column<string>(type: "VARCHAR2(50)", maxLength: 50, nullable: false),
                    EMAIL = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "VARCHAR2(255)", maxLength: 255, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USERS", x => x.USER_ID);
                });

            migrationBuilder.CreateIndex(
                name: "UK_USERS_EMAIL",
                table: "TB_USERS",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_USERS_USERNAME",
                table: "TB_USERS",
                column: "USERNAME",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_USERS");
        }
    }
}
