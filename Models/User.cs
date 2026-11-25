using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EngageTrack.Models
{
    public class User : IValidatableObject
    {
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        public string UserRole { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.Equals(UserRole, "Student", StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(UserEmail) ||
                 !Regex.IsMatch(UserEmail, @"^[A-Za-z0-9._%+-]+@mail\.shu\.ac\.uk$", RegexOptions.IgnoreCase)))
            {
                yield return new ValidationResult(
                    "Student email addresses must use the @mail.shu.ac.uk domain.",
                    new[] { nameof(UserEmail) });
            }
        }
    }
}
