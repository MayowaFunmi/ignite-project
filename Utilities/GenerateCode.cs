using System.Text;

namespace ignite_project.Utilities
{
  public class GenerateCode
  {
    public static string InvitationCode()
    {
      const string numbers = "123456789";
      const string letters = "ABCDEFGHJKLMNPQRTUVWXY";
      var random = new Random();
  
      var code = new StringBuilder();
      for (int i = 0; i < 2; i++)
      {
        code.Append(letters[random.Next(letters.Length)]);
      }
      for (int i = 0; i < 6; i++)
      {
        code.Append(numbers[random.Next(numbers.Length)]);
      }
      return code.ToString();
    }
  }
}