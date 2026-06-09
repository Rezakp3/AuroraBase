using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changecascadedeletetonoaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuService_Service_ServiceId",
                schema: "Auth",
                table: "MenuService");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                schema: "Auth",
                table: "RoleClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenu_Role_RoleId",
                schema: "Auth",
                table: "RoleMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleService_Role_RoleId",
                schema: "Auth",
                table: "RoleService");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleService_Service_ServiceId",
                schema: "Auth",
                table: "RoleService");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_Users_UserId",
                schema: "Auth",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_Users_UserId",
                schema: "Auth",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "Auth",
                table: "UserRole");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuService_Service_ServiceId",
                schema: "Auth",
                table: "MenuService",
                column: "ServiceId",
                principalSchema: "Auth",
                principalTable: "Service",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                schema: "Auth",
                table: "RoleClaim",
                column: "RoleId",
                principalSchema: "Auth",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenu_Role_RoleId",
                schema: "Auth",
                table: "RoleMenu",
                column: "RoleId",
                principalSchema: "Auth",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleService_Role_RoleId",
                schema: "Auth",
                table: "RoleService",
                column: "RoleId",
                principalSchema: "Auth",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleService_Service_ServiceId",
                schema: "Auth",
                table: "RoleService",
                column: "ServiceId",
                principalSchema: "Auth",
                principalTable: "Service",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_Users_UserId",
                schema: "Auth",
                table: "Session",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_Users_UserId",
                schema: "Auth",
                table: "UserClaim",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "Auth",
                table: "UserRole",
                column: "RoleId",
                principalSchema: "Auth",
                principalTable: "Role",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuService_Service_ServiceId",
                schema: "Auth",
                table: "MenuService");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                schema: "Auth",
                table: "RoleClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenu_Role_RoleId",
                schema: "Auth",
                table: "RoleMenu");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleService_Role_RoleId",
                schema: "Auth",
                table: "RoleService");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleService_Service_ServiceId",
                schema: "Auth",
                table: "RoleService");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_Users_UserId",
                schema: "Auth",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_Users_UserId",
                schema: "Auth",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "Auth",
                table: "UserRole");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuService_Service_ServiceId",
                schema: "Auth",
                table: "MenuService",
                column: "ServiceId",
                principalSchema: "Auth",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                schema: "Auth",
                table: "RoleClaim",
                column: "RoleId",
                principalSchema: "Auth",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenu_Role_RoleId",
                schema: "Auth",
                table: "RoleMenu",
                column: "RoleId",
                principalSchema: "Auth",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleService_Role_RoleId",
                schema: "Auth",
                table: "RoleService",
                column: "RoleId",
                principalSchema: "Auth",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleService_Service_ServiceId",
                schema: "Auth",
                table: "RoleService",
                column: "ServiceId",
                principalSchema: "Auth",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Session_Users_UserId",
                schema: "Auth",
                table: "Session",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_Users_UserId",
                schema: "Auth",
                table: "UserClaim",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "Auth",
                table: "UserRole",
                column: "RoleId",
                principalSchema: "Auth",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
