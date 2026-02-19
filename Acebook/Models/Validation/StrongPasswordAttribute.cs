using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace acebook.Models.Validation;

public class StrongPasswordAttribute : ValidationAttribute
{
    public int MinLength { get; }

    public StrongPasswordAttribute(int minLength = 8)
    {
        MinLength = minLength;
        ErrorMessage = $"Password must be at least {MinLength} characters and include uppercase, lowercase, a number, and a symbol.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var password = value as string;

        if (string.IsNullOrWhiteSpace(password))
            return ValidationResult.Success; // [Required] handles empty

        if (password.Length < MinLength)
            return new ValidationResult(ErrorMessage);

        bool hasLower = Regex.IsMatch(password, "[a-z]");
        bool hasUpper = Regex.IsMatch(password, "[A-Z]");
        bool hasDigit = Regex.IsMatch(password, "[0-9]");
        bool hasSymbol = Regex.IsMatch(password, @"[^a-zA-Z0-9]");

        if (!hasLower || !hasUpper || !hasDigit || !hasSymbol)
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}