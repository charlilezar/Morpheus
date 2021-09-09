using Microsoft.EntityFrameworkCore.Migrations;

namespace HengDa.LiZongMing.REAMS.Migrations
{
    public partial class _123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAddSegment",
                table: "AbpRoles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAddSegment",
                table: "AbpRoles");
        }
    }
}
