using BuildSync.Data;
using BuildSync.DTOs.Users;
using BuildSync.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Services;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UserService(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<UserDto?> GetMyInformationAsync()
    {  
        var currentUserId = _currentUser.UserId;

        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);
        
        if (user == null)
        {
            return null;
        }
      
        return UserMapper.ToUserDto(user);
    }

    public async Task<UserDto?> GetUserDtoAsync(Guid userGuid)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.GUserId == userGuid);

        if (user == null)
        {
            return null;
        }
        
        return UserMapper.ToUserDto(user);
    }


}