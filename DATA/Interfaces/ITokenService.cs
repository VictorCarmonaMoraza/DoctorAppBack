using MODEL.Entity;

namespace DATA.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
