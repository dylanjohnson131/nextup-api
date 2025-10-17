using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextUp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleValidationConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add check constraint to ensure only valid roles are allowed
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ADD CONSTRAINT ""CK_Users_Role"" 
                CHECK (""Role"" IN ('Player', 'Coach', 'AthleticDirector'))
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the role validation constraint
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                DROP CONSTRAINT ""CK_Users_Role""
            ");
        }
    }
}
