using MvcShop.Infrastructure.Repositories;

namespace MvcShop.Web.Repositories
{
    public class SessionStateRepository(IHttpContextAccessor httpContextAccessor) : IStateRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string GetValue(string key)
        {
            return _httpContextAccessor.HttpContext?.Session?.GetString(key)
                ?? string.Empty;
        }

        public void Remove(string key)
        {
            _httpContextAccessor.HttpContext?.Session?.Remove(key);
        }

        public void SetValue(string key, string value)
        {
            _httpContextAccessor.HttpContext?.Session?.SetString(key, value);
        }
    }
}