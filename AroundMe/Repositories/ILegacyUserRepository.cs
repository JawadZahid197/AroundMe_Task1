namespace AroundMe.Repositories;

public interface ILegacyUserRepository
{
    Task<int?> FindByEmailAsync(string email);
    Task<int> CreateAsync(string fullName, string username, string email);
}
