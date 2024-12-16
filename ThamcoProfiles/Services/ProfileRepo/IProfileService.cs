
using ThamcoProfiles.Models;
using ThamcoProfiles.Data;


namespace ThamcoProfiles.Services.ProfileRepo;


public interface IProfileService
{
    
    Task<User?> GetUserByIdAsync(int? id);
    Task AddUserAsync(User user);

    Task<bool> UpdateUser(User user);

    bool UserExists(int id);
 

    Task SaveChangesAsync();

    Task<User?> GetUserByAuth0IdAsync(string? id);
}