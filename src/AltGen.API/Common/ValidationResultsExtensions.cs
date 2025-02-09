namespace AltGen.API.Common;

static class ValidationResultsExtensions
{
  public static Dictionary<string, string[]> ToErrors(this IEnumerable<ValidationResult> results)
  {
    // group by the first member name and select the error message
    return results.GroupBy(static r => r.MemberNames.First())
      .ToDictionary(
        static r => r.Key,
        static r => r.Select(static e => e.ErrorMessage!).ToArray()
      );
  }
}