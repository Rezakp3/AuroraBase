using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResetPasswordTokenAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdateDate",
                schema: "Auth",
                table: "PasswordLogin",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordToken",
                schema: "Auth",
                table: "PasswordLogin",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordTokenExpireDate",
                schema: "Auth",
                table: "PasswordLogin",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordToken",
                schema: "Auth",
                table: "PasswordLogin");

            migrationBuilder.DropColumn(
                name: "ResetPasswordTokenExpireDate",
                schema: "Auth",
                table: "PasswordLogin");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdateDate",
                schema: "Auth",
                table: "PasswordLogin",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
