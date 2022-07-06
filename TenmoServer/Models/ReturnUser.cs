namespace TenmoServer.Models
{
    /// <summary>
    /// Model to return upon successful login
    /// </summary>
    public class ReturnUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        //public string Role { get; set; } this app has no roles, so we don't need to worry about this
        public string Token { get; set; }
    }
}
