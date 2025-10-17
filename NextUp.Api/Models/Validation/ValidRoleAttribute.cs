using System.ComponentModel.DataAnnotations;

namespace NextUp.Models.Validation
{
    public class ValidRoleAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string role)
            {
                return User.IsValidRole(role);
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must be one of: {string.Join(", ", User.ValidRoles)}";
        }
    }
}