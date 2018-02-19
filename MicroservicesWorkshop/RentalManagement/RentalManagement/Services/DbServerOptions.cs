namespace RentalManagement.Services
{
    public class DbServerOptions
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string DbPassword { get; set; }
        public string DbUser { get; set; }
        public string Protocol { get; set; }
        public int? TcpPort { get; set; }
    }
}
