using FluentAssertions;
using PoC.Azure.Deployment;
using Xunit.Abstractions;
using Environment = PoC.Azure.Deployment.Environment;

namespace PoC.Azure.Tests;

public class ContextTests
{
  private readonly ITestOutputHelper _output;

  public ContextTests(ITestOutputHelper output)
  {
    _output = output ?? throw new ArgumentNullException(nameof(output));
  }

  [Fact]
  public void GivenContextIsInitialized_WhenIn_ThenLocationChanged()
  {
    //Context.Current.Should().BeNull();
    const string module = "test";
    const string otherModule = "subtest";
    const string? otherDescription = "01";

    // Initialize the context to the sub/env and primary region + module
    using (Context.Initialize(Subscription.Xyz, Environment.Xyz, Region.WestEurope, module))
    {
      Context.Current.Should().NotBeNull();
      Context.Current.Environment.Should().Be(Environment.Xyz);
      Context.Current.Location.Should().Be(Region.WestEurope);
      Context.Current.Module.Should().Be(module);
      Context.Current.Description.Should().Be(null);

      _output.WriteLine(Context.Current.ToString());

      // change context region
      using (Context.In(Region.PolandCentral))
      {
        // change context module
        using (Context.ForModule(otherModule, otherDescription))
        {
          Context.Current.Should().NotBeNull();
          Context.Current.Environment.Should().Be(Environment.Xyz);
          Context.Current.Location.Should().Be(Region.PolandCentral);
          Context.Current.Module.Should().Be(otherModule);
          Context.Current.Description.Should().Be(otherDescription);

          _output.WriteLine(Context.Current.ToString());
        }
      }

      Context.Current.Should().NotBeNull();
      Context.Current.Environment.Should().Be(Environment.Xyz);
      Context.Current.Location.Should().Be(Region.WestEurope);
      Context.Current.Module.Should().Be(module);
      Context.Current.Description.Should().Be(null);

      _output.WriteLine(Context.Current.ToString());
    }

    //Context.Current.Should().BeNull();
  }
}
