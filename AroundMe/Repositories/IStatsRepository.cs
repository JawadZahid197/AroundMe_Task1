namespace AroundMe.Repositories;

public interface IStatsRepository
{
    Task<int> CountPlacesAsync();
    Task<int> CountUsersAsync();
}
