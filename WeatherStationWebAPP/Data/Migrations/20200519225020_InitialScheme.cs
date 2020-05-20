using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherStationWebAPP.Data.Migrations
{
    public partial class InitialScheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Temperature = table.Column<float>(nullable: false),
                    Humidity = table.Column<int>(nullable: false),
                    Pressure = table.Column<float>(nullable: false),
                    PlaceId = table.Column<int>(nullable: false),
                    PlaceId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observations_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Observations_Places_PlaceId1",
                        column: x => x.PlaceId1,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Observations_PlaceId",
                table: "Observations",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_PlaceId1",
                table: "Observations",
                column: "PlaceId1");

            migrationBuilder.CreateIndex(
                name: "IX_Places_Latitude_Longitude",
                table: "Places",
                columns: new[] { "Latitude", "Longitude" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropTable(
                name: "Places");
        }
    }
}
