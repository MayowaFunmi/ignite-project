namespace ignite_project.Configurations
{
  public class JwtCredentials
  {
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime Lifetime { get; set; }
    public string Audience { get; set; } = string.Empty;
  }
}