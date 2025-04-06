using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Swift_Move.Migrations
{
    /// <inheritdoc />
    public partial class InitServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Staff_AssignedStaffId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_AssignedStaffId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AssignedStaffId",
                table: "Services");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedStaffId",
                table: "Services",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_AssignedStaffId",
                table: "Services",
                column: "AssignedStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Staff_AssignedStaffId",
                table: "Services",
                column: "AssignedStaffId",
                principalTable: "Staff",
                principalColumn: "Id");
        }
    }
}
