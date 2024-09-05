using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MvcShop.Web.ValueProviders
{
    public class SessionValueProvider(BindingSource bindingSource, ISession session) : BindingSourceValueProvider(bindingSource)
    {
        private readonly ISession _session = session;

        public override bool ContainsPrefix(string prefix)
        {
            return _session.Get(prefix)?.Any() ?? false;
        }

        public override ValueProviderResult GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return ValueProviderResult.None;
            }

            if (_session.Keys.Contains(key))
            {
                return new ValueProviderResult(_session.GetString(key));
            }

            return ValueProviderResult.None;
        }
    }
}