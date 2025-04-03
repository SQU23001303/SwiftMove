using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Swift_Move.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceStafftable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
