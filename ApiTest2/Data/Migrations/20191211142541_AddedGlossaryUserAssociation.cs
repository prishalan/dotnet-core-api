using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiTest2.Data.Migrations
{
    public partial class AddedGlossaryUserAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Glossary",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Glossary_UserId",
                table: "Glossary",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Glossary_AspNetUsers_UserId",
                table: "Glossary",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Glossary_AspNetUsers_UserId",
                table: "Glossary");

            migrationBuilder.DropIndex(
                name: "IX_Glossary_UserId",
                table: "Glossary");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Glossary");
        }
    }
}
