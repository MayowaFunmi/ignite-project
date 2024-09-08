using System.Text.RegularExpressions;

namespace ignite_project.Utilities
{
  public class VerifyWallet
  {
    private static readonly Regex BitcoinAddressRegex = new("^(1|3|bc1)[a-zA-HJ-NP-Z0-9]{25,39}$");
    private static readonly Regex EthereumAddressRegex = new("^0x[a-fA-F0-9]{40}$");
    private static readonly Regex LitecoinAddressRegex = new("^(L|M|ltc1)[a-zA-HJ-NP-Z0-9]{25,39}$");
    private static readonly Regex DogecoinAddressRegex = new("^(D|A)[a-zA-HJ-NP-Z0-9]{25,39}$");

    public class VerifyCoin
    {
      public bool IsValid { get; set; } = false;
      public string CoinType { get; set; } = string.Empty;
    }

    public static VerifyCoin? ValidateWalletAddress(string walletAddress)
    {
      walletAddress = walletAddress.Trim();

      if (BitcoinAddressRegex.IsMatch(walletAddress))
      {
        return new VerifyCoin
        {
          IsValid = true,
          CoinType = "Bitcoin"
        };
      }

      else if (EthereumAddressRegex.IsMatch(walletAddress))
      {
        return new VerifyCoin
        {
          IsValid = true,
          CoinType = "Ethereum"
        };
      }

      else if (LitecoinAddressRegex.IsMatch(walletAddress))
      {
        return new VerifyCoin
        {
          IsValid = true,
          CoinType = "Litecoin"
        };
      }

      else if (DogecoinAddressRegex.IsMatch(walletAddress))
      {
        return new VerifyCoin
        {
          IsValid = true,
          CoinType = "Dogecoin"
        };
      }
      else return null;
    }

  }
}