using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace NextUp.Api.Migrations
{
    public partial class AddRoleValidationConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ADD CONSTRAINT ""CK_Users_Role"" 
                CHECK (""Role"" IN ('Player', 'Coach', 'AthleticDirector'))
            ");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                DROP CONSTRAINT ""CK_Users_Role""
            ");
        }
    }
}
