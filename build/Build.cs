using System.Linq;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Pulumi;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Pulumi.PulumiTasks;
class Build : NukeBuild
{
  const string PulumiPassphraseEnvVar = "PULUMI_CONFIG_PASSPHRASE";

  static readonly string[] Stacks = new[]
  {
    "PoC.Deployment.Network",
    "PoC.Deployment.KeyVault",
    "PoC.Deployment.Storage"
  };

  public static int Main() => Execute<Build>(x => x.Compile);

  [Solution]
  readonly Solution Solution;

  [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
  readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

  Target Clean => _ => _
      .Before(Restore)
      .Executes(() =>
      {
      });

  Target Restore => _ => _
      .Executes(() =>
      {
        DotNetRestore(_ => _.SetProjectFile(Solution));
      });

  Target Compile => _ => _
      .DependsOn(Restore)
      .Executes(() =>
      {
        DotNetBuild(_ => _
          .SetProjectFile(Solution)
          .SetConfiguration(Configuration)
          .EnableNoRestore());
      });

   Target Up => _ => _
      .DependsOn(Compile)
      .Executes(() =>
      {

        Stacks.ForEach(stack =>
        {
          PulumiStackSelect(_ => _
            .SetStackName("dev")
            .SetCwd(Solution.Directory / stack)
          );
          PulumiUp(_ => _
            .SetYes(true)
            .SetCwd(Solution.Directory / stack)
            .SetProcessEnvironmentVariable(PulumiPassphraseEnvVar, string.Empty)
          );
        });
      });

    Target Destroy => _ => _
      .DependsOn(Compile)
      .Executes(() =>
      {

        Stacks.Reverse().ForEach(stack =>
        {
          PulumiStackSelect(_ => _
            .SetStackName("dev")
            .SetCwd(Solution.Directory / stack)
          );
          PulumiDestroy(_ => _
            .SetYes(true)
            .SetCwd(Solution.Directory / stack)
            .SetProcessEnvironmentVariable(PulumiPassphraseEnvVar, string.Empty)
          );
        });
      });
}
