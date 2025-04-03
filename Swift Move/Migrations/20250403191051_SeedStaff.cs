using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Swift_Move.Migrations
{
    /// <inheritdoc />
    public partial class SeedStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedStaffId",
                table: "Services",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "Id", "Email", "FullName" },
                values: new object[,]
                {
                    { 1, "john.smith@swiftmove.com", "John Smith" },
                    { 2, "emily.johnson@swiftmove.com", "Emily Johnson" },
                    { 3, "aliyah.khan@swiftmove.com", "Aliyah Khan" },
                    { 4, "david.chen@swiftmove.com", "David Chen" },
                    { 5, "sophie.brown@swiftmove.com", "Sophie Brown" },
                    { 6, "carlos.garcia@swiftmove.com", "Carlos Garcia" },
                    { 7, "liam.oconnor@swiftmove.com", "Liam O'Connor" },
                    { 8, "zara.patel@swiftmove.com", "Zara Patel" },
                    { 9, "noah.williams@swiftmove.com", "Noah Williams" },
                    { 10, "hannah.lee@swiftmove.com", "Hannah Lee" }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Staff_AssignedStaffId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Services_AssignedStaffId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AssignedStaffId",
                table: "Services");
        }
    }
}
