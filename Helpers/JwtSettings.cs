namespace Library.Api.Helpers {
    public class JwtSettings {
        public string Key { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public int DurationMinutes { get; set; } = 1440;
    }
}
