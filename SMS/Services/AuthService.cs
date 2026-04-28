using SMS.Data;
using SMS.Models;
using System.Text.RegularExpressions;

namespace SMS.Services
{
    /// <summary>
    /// Centralises login, registration, and credential validation logic
    /// so forms only call service methods and never touch the DB directly for auth.
    /// </summary>
    public class AuthService
    {
        // ── Validation rules (single source of truth) ─────────────────────────
        public const int UsernameMinLength = 4;
        public const int PasswordMinLength = 6;

        /// <summary>Validates format only — does NOT check the database.</summary>
        public static ValidationResult ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Fail("Username is required.");
            if (string.IsNullOrWhiteSpace(password))
                return Fail("Password is required.");

            if (username.Length < UsernameMinLength)
                return Fail($"Username must be at least {UsernameMinLength} characters.");
            if (!username.Any(char.IsLetter))
                return Fail("Username must contain at least one letter (a–z).");
            if (!username.Any(char.IsDigit))
                return Fail("Username must contain at least one number (0–9).");
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                return Fail("Username may only use letters, digits, and underscores.");

            if (password.Length < PasswordMinLength)
                return Fail($"Password must be at least {PasswordMinLength} characters.");
            if (!password.Any(char.IsLetter))
                return Fail("Password must contain at least one letter (a–z).");
            if (!password.Any(char.IsDigit))
                return Fail("Password must contain at least one number (0–9).");

            return Ok();
        }

        // ── Login ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Returns the authenticated User, or null if credentials are wrong.
        /// Throws ArgumentException if format validation fails.
        /// </summary>
        public User? Login(string username, string password)
        {
            var v = ValidateCredentials(username, password);
            if (!v.IsValid) throw new ArgumentException(v.Message);

            using var db = new AppDbContext();
            var norm = username.Trim().ToLowerInvariant();
            return db.Users.FirstOrDefault(
                u => u.Username.ToLower() == norm && u.Password == password);
        }

        // ── Register ─────────────────────────────────────────────────────────
        /// <summary>
        /// Creates a new user. Throws ArgumentException on any validation failure.
        /// </summary>
        public void Register(string username, string password, string confirmPassword, string role)
        {
            var v = ValidateCredentials(username, password);
            if (!v.IsValid) throw new ArgumentException(v.Message);

            if (password != confirmPassword)
                throw new ArgumentException("Passwords do not match.");

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required.");

            using var db = new AppDbContext();
            if (db.Users.Any(u => u.Username == username))
                throw new ArgumentException("Username already exists. Choose a different one.");

            db.Users.Add(new User { Username = username, Password = password, Role = role });
            db.SaveChanges();
        }

        // ── Change password ───────────────────────────────────────────────────
        public void ChangePassword(int userId, string currentPassword, string newPassword)
        {
            using var db = new AppDbContext();
            var user = db.Users.Find(userId)
                ?? throw new ArgumentException("User not found.");

            if (user.Password != currentPassword)
                throw new ArgumentException("Current password is incorrect.");

            if (newPassword.Length < PasswordMinLength ||
                !newPassword.Any(char.IsLetter) ||
                !newPassword.Any(char.IsDigit))
                throw new ArgumentException(
                    $"New password must be ≥{PasswordMinLength} chars with letters and digits.");

            user.Password = newPassword;
            db.SaveChanges();
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static ValidationResult Ok()   => new(true,  "");
        private static ValidationResult Fail(string msg) => new(false, msg);
    }

    public record ValidationResult(bool IsValid, string Message);
}
