using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace AroundMe.Pages
{
    public class PlaceCard
    {
        public int PlaceId { get; set; }
        public string PlaceName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public string? ImagePath { get; set; }
        public string CategoryName { get; set; } = "";
        public string Username { get; set; } = "";
        public int LikeCount { get; set; }
    }

    public class CategoryItem
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }

    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        public IndexModel(IConfiguration config) { _config = config; }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CategoryId { get; set; }

        public List<PlaceCard> Places { get; set; } = new();
        public List<CategoryItem> Categories { get; set; } = new();
        public int TotalPlaces { get; set; }
        public int TotalUsers { get; set; }

        public void OnGet()
        {
            using var con = new SqlConnection(_config.GetConnectionString("AroundMeDB"));
            con.Open();

            // Categories
            var catCmd = new SqlCommand("SELECT CategoryId, CategoryName FROM Categories", con);
            using (var dr = catCmd.ExecuteReader())
            {
                while (dr.Read())
                    Categories.Add(new CategoryItem
                    {
                        CategoryId = (int)dr["CategoryId"],
                        CategoryName = dr["CategoryName"].ToString()!
                    });
            }

            // Stats
            TotalPlaces = (int)new SqlCommand("SELECT COUNT(*) FROM Places", con).ExecuteScalar()!;
            TotalUsers = (int)new SqlCommand("SELECT COUNT(*) FROM Users", con).ExecuteScalar()!;

            // Places (search + filter)
            string sql = @"SELECT p.PlaceId, p.PlaceName, p.Description, p.Location, p.ImagePath,
                                  c.CategoryName, u.Username,
                                  (SELECT COUNT(*) FROM Likes l WHERE l.PlaceId = p.PlaceId) AS LikeCount
                           FROM Places p
                           JOIN Categories c ON p.CategoryId = c.CategoryId
                           JOIN Users u ON p.UserId = u.UserId
                           WHERE (@q = '' OR p.PlaceName LIKE '%' + @q + '%'
                                          OR p.Description LIKE '%' + @q + '%'
                                          OR p.Location LIKE '%' + @q + '%')
                             AND (@cat = 0 OR p.CategoryId = @cat)
                           ORDER BY p.CreatedAt DESC";

            var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@q", (Search ?? "").Trim());
            cmd.Parameters.AddWithValue("@cat", CategoryId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Places.Add(new PlaceCard
                {
                    PlaceId = (int)reader["PlaceId"],
                    PlaceName = reader["PlaceName"].ToString()!,
                    Description = reader["Description"].ToString()!,
                    Location = reader["Location"].ToString()!,
                    ImagePath = reader["ImagePath"] == DBNull.Value ? null : reader["ImagePath"].ToString(),
                    CategoryName = reader["CategoryName"].ToString()!,
                    Username = reader["Username"].ToString()!,
                    LikeCount = (int)reader["LikeCount"]
                });
            }
        }
    }
}