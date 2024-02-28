using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using Pulumi.AzureNative.Resources;

namespace PoC.Azure.Networking;

public class VirtualNetworkBuilder : AzureResourceBuilder<VirtualNetwork>
{
  public VirtualNetworkBuilder() : base(ResourceType.VirtualNetwork)
  { }

  private string[] _addressPrefixes = default!;

  public VirtualNetworkBuilder WithAddressPrefixes(string[] addressPrefixes)
  {
    _addressPrefixes = addressPrefixes;
    return this;
  }

  public override (ResourceGroup ResourceGroup, VirtualNetwork Resource) Build()
  {
    var resourceGroup = GetResourceGroup();

    var virtualNetwork = new VirtualNetwork(Name, new VirtualNetworkArgs()
    {
      AddressSpace = new AddressSpaceArgs()
      {
        AddressPrefixes = _addressPrefixes ?? throw new InvalidOperationException()
      },
      Location = Context.Current.Location.ToString(),
      VirtualNetworkName = Name,
      ResourceGroupName = resourceGroup.Name,
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });

    return (resourceGroup, virtualNetwork);
  }
}
