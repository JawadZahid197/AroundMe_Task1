using AroundMe.Models;
using AroundMe.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AroundMe.Services;
public class PlaceRepository(IConfiguration config)
{
    private SqlConnection Open()
    {
        var c = new SqlConnection(config.GetConnectionString("AroundMeDB"));
        c.Open();
        return c;
    }
    public List<CategoryItem> Categories()
    {
        using var c=Open();
        using var cmd=new SqlCommand("SELECT CategoryId, CategoryName FROM Categories ORDER BY CategoryName",c);
        using var r=cmd.ExecuteReader();
        var list=new List<CategoryItem>();
        while(r.Read()) list.Add(new(){CategoryId=r.GetInt32(0),CategoryName=r.GetString(1)});
        return list;
    }
    public HomeViewModel Home(string? search,int categoryId)
    {
        using var c=Open();
        var m=new HomeViewModel
        {
            Search=search,CategoryId=categoryId
        };
        using(var cmd=new SqlCommand("SELECT CategoryId,CategoryName FROM Categories ORDER BY CategoryName",c)) using(var r=cmd.ExecuteReader()) while(r.Read()) m.Categories.Add(new(){CategoryId=r.GetInt32(0),CategoryName=r.GetString(1)});
        m.TotalPlaces=Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM Places",c).ExecuteScalar());
        m.TotalUsers=Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM Users",c).ExecuteScalar());
        const string sql=@"SELECT p.PlaceId,p.PlaceName,p.Description,p.Location,p.ImagePath,c.CategoryName,u.Username,(SELECT COUNT(*) FROM Likes l WHERE l.PlaceId=p.PlaceId) LikeCount FROM Places p JOIN Categories c ON p.CategoryId=c.CategoryId JOIN Users u ON p.UserId=u.UserId WHERE (@q='' OR p.PlaceName LIKE '%'+@q+'%' OR p.Description LIKE '%'+@q+'%' OR p.Location LIKE '%'+@q+'%') AND (@cat=0 OR p.CategoryId=@cat) ORDER BY p.CreatedAt DESC";
        using var q=new SqlCommand(sql,c);
        q.Parameters.Add("@q",SqlDbType.NVarChar,250).Value=(search??"").Trim();
        q.Parameters.Add("@cat",SqlDbType.Int).Value=categoryId;
        using var reader=q.ExecuteReader();
        while(reader.Read()) m.Places.Add(new(){PlaceId=reader.GetInt32(0),PlaceName=reader.GetString(1),Description=reader.GetString(2),Location=reader.GetString(3),ImagePath=reader.IsDBNull(4)?null:reader.GetString(4),CategoryName=reader.GetString(5),Username=reader.GetString(6),LikeCount=reader.GetInt32(7)});
        return m;
    }
    public int? FindLegacyUser(string email)
    {
        using var c=Open();
        using var cmd=new SqlCommand("SELECT UserId FROM Users WHERE Email=@email",c);
        cmd.Parameters.Add("@email",SqlDbType.NVarChar,256).Value=email;
        var result=cmd.ExecuteScalar();
        return result==null?null:Convert.ToInt32(result);
    }
    public int CreateLegacyUser(string fullName,string username,string email)
    {
        using var c=Open();
        using var cmd=new SqlCommand("INSERT INTO Users(FullName,Username,Email,PasswordHash) OUTPUT INSERTED.UserId VALUES(@n,@u,@e,@p)",c);
        cmd.Parameters.Add("@n",SqlDbType.NVarChar,200).Value=fullName;
        cmd.Parameters.Add("@u",SqlDbType.NVarChar,100).Value=username;
        cmd.Parameters.Add("@e",SqlDbType.NVarChar,256).Value=email;
        cmd.Parameters.Add("@p",SqlDbType.NVarChar,255).Value="IDENTITY_MANAGED";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
    public void Add(int userId,PlaceCreateViewModel model,string? imagePath)
    {
        using var c=Open();
        using var cmd=new SqlCommand("INSERT INTO Places(UserId,CategoryId,PlaceName,Description,Location,ImagePath) VALUES(@u,@c,@n,@d,@l,@i)",c);
        cmd.Parameters.Add("@u",SqlDbType.Int).Value=userId;
        cmd.Parameters.Add("@c",SqlDbType.Int).Value=model.CategoryId!.Value;
        cmd.Parameters.Add("@n",SqlDbType.NVarChar,150).Value=model.PlaceName.Trim();
        cmd.Parameters.Add("@d",SqlDbType.NVarChar,1000).Value=model.Description.Trim();
        cmd.Parameters.Add("@l",SqlDbType.NVarChar,250).Value=model.Location.Trim();
        cmd.Parameters.Add("@i",SqlDbType.NVarChar,500).Value=(object?)imagePath??DBNull.Value;
        cmd.ExecuteNonQuery();
    }
}
