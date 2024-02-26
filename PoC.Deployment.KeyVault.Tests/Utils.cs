namespace PoC.Deployment.KeyVault.Tests;

internal class Utils
{
  internal static string RandomStackName()
  {
    const string chars = "abcdefghijklmnopqrstuvwxyz";
    return new string(Enumerable.Range(1, 8).Select(_ => chars[new Random().Next(chars.Length)]).ToArray());
  }
}
