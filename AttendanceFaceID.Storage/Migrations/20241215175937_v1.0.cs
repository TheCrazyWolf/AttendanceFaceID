using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceFaceID.Storage.Migrations
{
    /// <inheritdoc />
    public partial class v10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Students");

            migrationBuilder.CreateTable(
                name: "UnificationProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MainProfileId = table.Column<long>(type: "INTEGER", nullable: true),
                    SubProfileId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnificationProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnificationProfiles_Students_MainProfileId",
                        column: x => x.MainProfileId,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnificationProfiles_Students_SubProfileId",
                        column: x => x.SubProfileId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnificationProfiles_MainProfileId",
                table: "UnificationProfiles",
                column: "MainProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UnificationProfiles_SubProfileId",
                table: "UnificationProfiles",
                column: "SubProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnificationProfiles");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Students",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Students",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Students",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
