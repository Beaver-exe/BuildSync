
using BuildSync.DTOs.Users;
using BuildSync.Models;

namespace BuildSync.Mappings;

public static class UserMapper
{
    public static UserDto ToUserDto(User user)
    {
        return new UserDto
        {
            GUserId = user.GUserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Profession = user.Profession,
            Email = user.Email
        };
    }
}