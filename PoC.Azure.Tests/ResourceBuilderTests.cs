using FluentAssertions;
using PoC.Azure.Deployment;
using Action = System.Action;
using Environment = PoC.Azure.Deployment.Environment;

namespace PoC.Azure.Tests;

public class ResourceBuilderTests
{
  [Fact]
  public void GivenResourceGroupBuilder_WhenNoContext_ThenThrowsInvalidOperationException()
  {
    var action = new Action(() =>
    {
      var rgp = new ResourceGroupBuilder()
        .SetProtection()
        .Build();
    });

    action.Should().Throw<InvalidOperationException>().And.Message.Should().Contain($"{nameof(Context.Initialize)}");
  }

  [Fact]
  public void GivenResourceBuilder_WhenNoResourceGroup_ThenThrowsInvalidOperationException()
  {
    var action = new Action(() =>
    {
      using (Context.Initialize(Environment.Dev, Region.PolandCentral, "data"))
      {
        var resource = new TestResourceBuilder()
          .SetProtection()
          .Build();
      }
    });

    action.Should().Throw<InvalidOperationException>()
      .And.Message.Should()
      .Contain($"{nameof(ResourceBuilderExtensions.In)}")
      .And
      .Contain($"{nameof(ResourceBuilderExtensions.WithResourceGroupName)}");
  }
}
