using System.Security.Claims;

namespace FlowerShop.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Идентификатор пользователя в токене отсутствует или невалиден.");
            }
            return userId;
        }
    }
}