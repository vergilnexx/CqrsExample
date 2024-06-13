using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meta.Example.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class _111_AddColumnForecastIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WeatherForecast",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WeatherForecast");
        }
    }
}
