using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleUserTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByUserId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Sales",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CancelledByUserId",
                table: "Sales",
                column: "CancelledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CreatedByUserId",
                table: "Sales",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_UpdatedByUserId",
                table: "Sales",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Users_CancelledByUserId",
                table: "Sales",
                column: "CancelledByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Users_CreatedByUserId",
                table: "Sales",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Users_UpdatedByUserId",
                table: "Sales",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Users_CancelledByUserId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Users_CreatedByUserId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Users_UpdatedByUserId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CancelledByUserId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CreatedByUserId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_UpdatedByUserId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Sales");
        }
    }
}
