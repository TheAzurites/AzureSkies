using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureSkies.Migrations
{
    public partial class AutoMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Airline = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightInfo", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FlightInfo",
                columns: new[] { "Id", "Airline", "FlightDate", "FlightNumber", "FlightStatus" },
                values: new object[] { 1, "American Airlines", "2021-08-19", "2206", "Active" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightInfo");
        }
    }
}
