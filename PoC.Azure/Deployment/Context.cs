namespace PoC.Azure.Deployment;

public record ContextState(Region Location, Environment Environment, string Module, string? Description)
{
  public override string ToString()
  {
    return Description == null
      ? $"{Location.ToString().ToLowerInvariant()}/{Environment.ToString().ToLowerInvariant()}/{Module}"
      : $"{Location.ToString().ToLowerInvariant()}/{Environment.ToString().ToLowerInvariant()}/{Module}/{Description}";
  }
}

public class Context : IDisposable
{
  private static readonly AsyncLocal<ContextState> _state = new();
  private readonly ContextState _previousState;

  private Context(Environment environment, Region location, string module)
  {
    _previousState = _state.Value!;
    _state.Value = new ContextState(location, environment, module, null);
  }

  private Context(Environment environment, Region location, string module, string description)
  {
    _previousState = _state.Value!;
    _state.Value = new ContextState(location, environment, module, description);
  }

  public static Context Initialize(Subscription subscription, Environment environment, Region location, string module)
  {
    return new Context(environment, location, module);
  }

  public static Context In(Region location)
  {
#pragma warning disable CS8073
    if (_state.Value == null) throw new InvalidOperationException();
#pragma warning restore CS8073

    var current = _state.Value;

    return new Context(current.Environment, location, current.Module);
  }

  public static Context ForModule(string module, string? description)
  {
#pragma warning disable CS8073
    if (_state.Value == null) throw new InvalidOperationException();
#pragma warning restore CS8073

    var current = _state.Value;

    return description == null
      ? new Context(current.Environment, current.Location, module, current.Description!)
      : new Context(current.Environment, current.Location, module, description);
  }

  public void Dispose()
  {
    _state.Value = _previousState;
    GC.SuppressFinalize(this);
  }

  public static ContextState Current
  {
    get
    {
      if (_state.Value == null)
        throw new InvalidOperationException($"Context is unset. Ensure {nameof(Context.Initialize)} has been invoked");
      return _state.Value!;
    }
  }
}
