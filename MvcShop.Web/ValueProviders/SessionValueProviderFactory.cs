using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MvcShop.Web.ValueProviders
{
    public class SessionValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            ArgumentNullException.ThrowIfNull(nameof(context));

            var session = context.ActionContext.HttpContext.Session;

            if (session is not null && session.Keys.Any())
            {
                var valueProvider = new SessionValueProvider(BindingSource.ModelBinding,
                    session);

                context.ValueProviders.Add(valueProvider);
            }

            return Task.CompletedTask;
        }
    }
}