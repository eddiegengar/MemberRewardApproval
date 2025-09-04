using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Options;
using MemberRewardApproval.WebApi.Options;

namespace MemberRewardApproval.WebApi.Services
{
    public class GraphUserService
    {
        private readonly GraphServiceClient _graph;

        public GraphUserService(IOptions<AzureAdOptions> options)
        {
            var tenantId = options.Value.TenantId;
            var clientId = options.Value.ClientId;
            var clientSecret = options.Value.ClientSecret;

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _graph = new GraphServiceClient(credential);
        }

        /// <summary>
        /// Get Azure AD Object ID (AAD ID) from user email
        /// </summary>
        public async Task<string> GetUserAadIdByEmailAsync(string email)
        {
            var user = await _graph.Users[email].GetAsync();
            return user.Id;
        }
    }
}
