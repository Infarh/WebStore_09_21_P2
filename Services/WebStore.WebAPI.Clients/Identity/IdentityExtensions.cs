using System;
using System.Net.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using WebStore.Domain.Entities.Identity;

namespace WebStore.WebAPI.Clients.Identity
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityWebStoreWebAPIClients(this IServiceCollection services)
        {
            services.AddHttpClient("WebStoreAPIIdentity", (s, client) => client.BaseAddress = new(s.GetRequiredService<IConfiguration>()["WebAPI"]))
               .AddTypedClient<IUserStore<User>, UsersClient>()
               .AddTypedClient<IUserRoleStore<User>, UsersClient>()
               .AddTypedClient<IUserPasswordStore<User>, UsersClient>()
               .AddTypedClient<IUserEmailStore<User>, UsersClient>()
               .AddTypedClient<IUserPhoneNumberStore<User>, UsersClient>()
               .AddTypedClient<IUserTwoFactorStore<User>, UsersClient>()
               .AddTypedClient<IUserClaimStore<User>, UsersClient>()
               .AddTypedClient<IUserLoginStore<User>, UsersClient>()
               .AddTypedClient<IRoleStore<Role>, RolesClient>()
               .SetHandlerLifetime(TimeSpan.FromMinutes(5))
               .AddPolicyHandler(GetRetryPolicy())
               .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int RetryMaxCount = 6, int MaxJitter = 1000)
        {
            var jitterer = new Random();
            return HttpPolicyExtensions
               .HandleTransientHttpError()
                //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
               .WaitAndRetryAsync(RetryMaxCount, RetryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, RetryAttempt)) +
                    TimeSpan.FromMilliseconds(jitterer.Next(0, MaxJitter))
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
            HttpPolicyExtensions
               .HandleTransientHttpError()
               .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, TimeSpan.FromSeconds(30));

        public static IdentityBuilder AddIdentityWebStoreWebAPIClients(this IdentityBuilder builder)
        {
            builder.Services.AddIdentityWebStoreWebAPIClients();

            return builder;
        }
    }
}
