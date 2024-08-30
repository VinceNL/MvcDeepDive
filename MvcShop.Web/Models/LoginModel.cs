using System.ComponentModel.DataAnnotations;

namespace MvcShop.Web.Models
{
    public class LoginModel
    {
        [EmailAddress]
        public required string Username { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
