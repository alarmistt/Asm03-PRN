namespace Services.Interface
{
    public interface IMemberService
    {
        Task<bool> Login(string username, string password);
    }
}
