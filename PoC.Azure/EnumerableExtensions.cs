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

  public static IDictionary<TKey, TValue> ReverseLookup<TKey, TValue>(this IEnumerable<TValue> values,
    IDictionary<TKey, TValue> source)
  {
    var result = new Dictionary<TKey, TValue>();

    foreach (var value in values)
    {
      var key = source.FirstOrDefault(x => x.Value!.Equals(value)).Key;
      if (key != null)
      {
        result[key] = value;
      }
    }

    return result;
  }
}
