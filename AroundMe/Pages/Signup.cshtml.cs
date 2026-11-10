using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace AroundMe.Pages
{
    public class SignupModel : PageModel
    {
        private readonly IConfiguration _config;
        public SignupModel(IConfiguration config) { _config = config; }

        [BindProperty, Required(ErrorMessage = "Name required")]
        public string FullName { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Username required")]
        public string Username { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Email required"), EmailAddress(ErrorMessage = "Valid email likho")]
        public string Email { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Password required"), MinLength(6, ErrorMessage = "Kam az kam 6 characters")]
        public string Password { get; set; } = "";

        [BindProperty, Compare("Password", ErrorMessage = "Passwords match nahi karte")]
        public string ConfirmPassword { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            using var con = new SqlConnection(_config.GetConnectionString("AroundMeDB"));
            con.Open();

            var check = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Email=@e OR Username=@u", con);
            check.Parameters.AddWithValue("@e", Email.Trim());
            check.Parameters.AddWithValue("@u", Username.Trim());
            if ((int)check.ExecuteScalar()! > 0)
            {
                Message = "Email ya username pehle se maujood hai.";
                return Page();
            }

            var cmd = new SqlCommand(
                "INSERT INTO Users (FullName, Username, Email, PasswordHash) VALUES (@n,@u,@e,@p)", con);
            cmd.Parameters.AddWithValue("@n", FullName.Trim());
            cmd.Parameters.AddWithValue("@u", Username.Trim());
            cmd.Parameters.AddWithValue("@e", Email.Trim());
            cmd.Parameters.AddWithValue("@p", BCrypt.Net.BCrypt.HashPassword(Password));
            cmd.ExecuteNonQuery();

            return RedirectToPage("Login");
        }
    }
}