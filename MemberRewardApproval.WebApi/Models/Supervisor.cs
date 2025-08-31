using System.ComponentModel.DataAnnotations;

namespace MemberRewardApproval.WebApi.Models
{
    public class Supervisor
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        // Add this property to store Azure AD ID
        public string AadId { get; set; }
    }
}
