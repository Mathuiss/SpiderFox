using Microsoft.EntityFrameworkCore.Migrations;

namespace InternetExplorer.Migrations
{
    public partial class removedgenval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Discoverys",
                nullable: false,
                oldClrType: typeof(long))
                .OldAnnotation("Sqlite:Autoincrement", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Discoverys",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
