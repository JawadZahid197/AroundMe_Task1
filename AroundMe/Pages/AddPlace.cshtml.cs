using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace AroundMe.Pages
{
    public class AddPlaceModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public AddPlaceModel(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        [BindProperty, Required(ErrorMessage = "Place name required")]
        public string PlaceName { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Category select karo")]
        public int? CategoryId { get; set; }

        [BindProperty, Required(ErrorMessage = "Location required")]
        public string Location { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Description required"),
         MaxLength(1000, ErrorMessage = "Max 1000 characters")]
        public string Description { get; set; } = "";

        [BindProperty]
        public IFormFile? Photo { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
        public string Message { get; set; } = "";

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToPage("Login");

            LoadCategories();
            return Page();
        }

        public IActionResult OnPost()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("Login");

            LoadCategories();
            if (!ModelState.IsValid) return Page();

            string imagePath = "";

            if (Photo != null && Photo.Length > 0)
            {
                string ext = Path.GetExtension(Photo.FileName).ToLower();
                string[] allowed = { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowed.Contains(ext))
                {
                    Message = "Sirf jpg, png ya webp photo allowed hai.";
                    return Page();
                }
                if (Photo.Length > 5 * 1024 * 1024)
                {
                    Message = "Photo 5 MB se chhoti honi chahiye.";
                    return Page();
                }

                string folder = Path.Combine(_env.WebRootPath, "Uploads");
                Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + ext;
                using (var fs = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    Photo.CopyTo(fs);
                }
                imagePath = "Uploads/" + fileName;
            }

            using var con = new SqlConnection(_config.GetConnectionString("AroundMeDB"));
            con.Open();
            var cmd = new SqlCommand(
                @"INSERT INTO Places (UserId, CategoryId, PlaceName, Description, Location, ImagePath)
                  VALUES (@u,@c,@n,@d,@l,@i)", con);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@c", CategoryId);
            cmd.Parameters.AddWithValue("@n", PlaceName.Trim());
            cmd.Parameters.AddWithValue("@d", Description.Trim());
            cmd.Parameters.AddWithValue("@l", Location.Trim());
            cmd.Parameters.AddWithValue("@i", imagePath == "" ? DBNull.Value : imagePath);
            cmd.ExecuteNonQuery();

            return RedirectToPage("Index");
        }

        private void LoadCategories()
        {
            using var con = new SqlConnection(_config.GetConnectionString("AroundMeDB"));
            con.Open();
            var cmd = new SqlCommand("SELECT CategoryId, CategoryName FROM Categories", con);
            using var dr = cmd.ExecuteReader();
            Categories.Clear();
            while (dr.Read())
            {
                Categories.Add(new SelectListItem(
                    dr["CategoryName"].ToString(), dr["CategoryId"].ToString()));
            }
        }
    }
}