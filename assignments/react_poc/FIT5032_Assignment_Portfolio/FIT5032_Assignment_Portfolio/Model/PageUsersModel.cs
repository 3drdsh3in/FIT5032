namespace FIT5032_Assignment_Portfolio.Model
{
    public class PageUsersModel
    {
        public int TotalUsers { get; set; }
        public List<UserModel> Users { get; set; }
            = new List<UserModel>();
    }
}
