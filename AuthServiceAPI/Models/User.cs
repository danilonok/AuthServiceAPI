namespace AuthServiceAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public UserCredential Credential { get; set; }
    }
}
