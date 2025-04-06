using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Swift_Move.Migrations
{
    /// <inheritdoc />
    public partial class updatedservicestaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "ServiceStaff",
                columns: table => new
                {
                    ServiceModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    StaffId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStaff", x => new { x.ServiceModelId, x.StaffId });
                    table.ForeignKey(
                        name: "FK_ServiceStaff_Services_ServiceModelId",
                        column: x => x.ServiceModelId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceStaff_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ServiceStaff_StaffId",
                table: "ServiceStaff",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceStaff");

            migrationBuilder.DropTable(
                name: "Staff");
        }
    }
}
