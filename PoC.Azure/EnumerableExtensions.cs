namespace PoC.Azure;

public static class EnumerableExtensions
{
  public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
  {
    _ = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
    _ = action ?? throw new ArgumentNullException(nameof(action));

    foreach (var e in enumerable)
    {
      action(e);
    }

    return enumerable;
  }
}
