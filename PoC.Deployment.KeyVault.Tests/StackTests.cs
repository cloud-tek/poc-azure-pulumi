using System.Reflection;
using FluentAssertions;
using PoC.Deployment.KeyVault.Tests.Synchronization;
using Pulumi;
using Pulumi.Automation;
using Xunit.Abstractions;

namespace PoC.Deployment.KeyVault.Tests;

public class StackTests : IAsyncDisposable
{
  private readonly string _stackName;
  private readonly ProjectSettings _projectSettings;
  private LocalWorkspace? _workspace;
  private static readonly AsyncLock asyncLock = new AsyncLock();
  private readonly ITestOutputHelper _output;

  private static readonly IDictionary<string, string> Environment = new Dictionary<string, string?>
  {
    ["PULUMI_CONFIG_PASSPHRASE"] = "",
  }!;

  public StackTests(ITestOutputHelper output)
  {
    _output = output ?? throw new ArgumentNullException(nameof(output));
    _stackName = Utils.RandomStackName();
    _projectSettings = new ProjectSettings(Assembly.GetExecutingAssembly().GetName().Name!, ProjectRuntimeName.Dotnet);
  }

  public async ValueTask DisposeAsync()
  {
    await _workspace?.RemoveStackAsync(_stackName)!;
    _workspace?.Dispose();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task GivenValidWorkspaceStack_WhenPulumiUp_ThenUpdateSucceededAndValidOutputs()
  {
    WorkspaceStack? stack = null;
    try
    {
      stack = await CreateWorkspaceStack<KeyVaultStack>(Utils.RandomStackName());
      var result = await stack.UpAsync();

      result.Summary.Kind.Should().Be(UpdateKind.Update);
      result.Summary.Result.Should().Be(UpdateState.Succeeded);

      var name = result.Outputs[nameof(KeyVaultStack.Name)].Value.ToString();
      var rgp = result.Outputs[nameof(KeyVaultStack.ResourceGroupName)].Value.ToString();

      name.Should().NotBeNullOrEmpty();
      rgp.Should().NotBeNullOrEmpty();

      _output.WriteLine($"{rgp}/{name}");
    }
    finally
    {
      await stack?.DestroyAsync()!;
      stack?.Dispose();
    }
  }

  [Fact]
  public async Task GivenValidLocalStack_WhenCreated_ThenIsCurrent()
  {
    var stack = await EnsureStackIsCreated();
    stack.Should().NotBeNull();
    stack.IsCurrent.Should().BeTrue();
  }

  private async Task<WorkspaceStack> CreateWorkspaceStack<TStack>(string name)
    where TStack: Stack, new()
  {
    var program = PulumiFn.Create<TStack>();
    var stack = await LocalWorkspace.CreateStackAsync(new InlineProgramArgs(
      projectName:Assembly.GetExecutingAssembly().GetName().Name!,
      stackName: name,
      program: program)
    {
      EnvironmentVariables = Environment!
    });

    await stack.SetConfigAsync("azure.tenantId", new ConfigValue("d0564bc7-e5fa-4c0f-8dee-6fd0fd39bf49"));

    return stack;
  }

  private async Task<StackSummary> EnsureStackIsCreated(PulumiFn? program = null)
  {
    using var @lock = await asyncLock.LockAsync();

    _workspace ??= await LocalWorkspace.CreateAsync(new LocalWorkspaceOptions
    {
      ProjectSettings = _projectSettings,
      EnvironmentVariables = Environment!,
    });

    var stacks = await _workspace.ListStacksAsync()
      .ConfigureAwait(false);

    if (stacks.All(s => s.Name != _stackName))
    {
        await _workspace.CreateStackAsync(_stackName);
    }

    stacks = await _workspace.ListStacksAsync()
      .ConfigureAwait(false);

    var stack = stacks.FirstOrDefault(s => s.Name == _stackName);

    stack.Should().NotBeNull();
    stack!.IsCurrent.Should().BeTrue();

    return stack;
  }
}
