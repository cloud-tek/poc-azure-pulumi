using System.Linq.Expressions;
using Google.Protobuf;
using Pulumi;

namespace PoC.Azure;

public class StackReference<T> : StackReference where T : Stack
{
  public static StackReference<T> Create(string? stackName = null) {

    var type = typeof(T).Name.Replace("Stack", string.Empty).ToLowerInvariant();
    // var lastDotIndex = Deployment.Instance.StackName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
    // var environment = stackName ?? Deployment.Instance.StackName.Substring(lastDotIndex + 1).ToLowerInvariant();

    return new StackReference<T>(typeof(T).Name, new StackReferenceArgs
    {
      // WHEN Using cloud provider stack reference can't contain organization and type
      // We assuming last section of the stack is environment
      Name = $"organization/{typeof(T).Assembly.GetName().Name}/{stackName}"
    });
  }

  public StackReference(string name, StackReferenceArgs args, CustomResourceOptions? options = null)
    : base(name, args, options)
  {
  }

  public Output<G> RequireOutput<G>(Expression<Func<T, Output<G>>> action) =>
    RequireOutput(((MemberExpression)action.Body).Member.Name).Apply(_ => (G)_);
}
