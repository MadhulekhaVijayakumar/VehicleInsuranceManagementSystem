using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; } = string.Empty;
        public byte[] Password { get; set; }
    
        public byte[] HashKey { get; set; }
        public string Role { get; set; }=string.Empty;

        public Client? Client { get; set; }//navigation
        public Admin? Admin { get; set; }
    }
}
