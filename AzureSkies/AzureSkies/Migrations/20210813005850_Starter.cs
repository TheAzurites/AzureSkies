using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureSkies.Migrations
{
    public partial class Starter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightsInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartureAirport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArrivalAirport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightIcao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumbers = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightsInfo", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FlightsInfo",
                columns: new[] { "Id", "AirlineName", "ArrivalAirport", "DepartureAirport", "FlightDate", "FlightIcao", "FlightNumber", "FlightStatus", "PhoneNumbers" },
                values: new object[] { 1, "Delta Airlines", "John F. Kennedy International Airport", "Seattle-Tacoma International Airport", "2021-08-10", "DAL.0001", "0001", "active", "+1234567889" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightsInfo");
        }
    }
}
