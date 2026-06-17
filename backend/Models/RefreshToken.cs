namespace BuildSync.Models;

public class RefreshToken
{
    public int RefreshTokenId { set; get; } 
    public string Token { set; get; }  = string.Empty;
    public DateTime Created { set; get; }
    public DateTime Expiry { set; get; }
    public bool IsRevoked { set; get; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}