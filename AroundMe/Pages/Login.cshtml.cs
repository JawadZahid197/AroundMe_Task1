using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace AroundMe.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _config;
        public LoginModel(IConfiguration config) { _config = config; }

        [BindProperty, Required(ErrorMessage = "Email required")]
        public string Email { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Password required")]
        public string Password { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            using var con = new SqlConnection(_config.GetConnectionString("AroundMeDB"));
            con.Open();

            var cmd = new SqlCommand(
                "SELECT UserId, Username, PasswordHash FROM Users WHERE Email=@e", con);
            cmd.Parameters.AddWithValue("@e", Email.Trim());

            using var dr = cmd.ExecuteReader();
            if (dr.Read() && BCrypt.Net.BCrypt.Verify(Password, dr["PasswordHash"].ToString()))
            {
                HttpContext.Session.SetInt32("UserId", (int)dr["UserId"]);
                HttpContext.Session.SetString("Username", dr["Username"].ToString()!);
                return RedirectToPage("Index");
            }

            Message = "Email ya password ghalat hai.";
            return Page();
        }
    }
}