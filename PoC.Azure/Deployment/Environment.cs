namespace PoC.Azure.Deployment;

public enum Environment
{
  Lcl = 0,
  Dev,
  Uat,
  Xyz
}

public static class EnvironmentExtensions
{
  public static Environment ToEnvironment(this string @value)
  {
    return Enum.TryParse(value, true, out Environment result) ? result : default(Environment);
  }
}
