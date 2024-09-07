using MvcShop.Infrastructure.Repositories;

namespace MvcShop.Web.Tests.Repositories;

public class InMemoryStateRepository : IStateRepository
{
    public IDictionary<string, string> States { get; } = new Dictionary<string, string>();

    public string GetValue(string key)
    {
        return States[key];
    }

    public void Remove(string key)
    {
        States.Remove(key);
    }

    public void SetValue(string key, string value)
    {
        States[key] = value;
    }
}
