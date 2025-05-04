namespace ThomasonAlgorithm.Core.Extensions;

public static class ListExtensions
{
    /// <summary>
    /// Returns the first element in the list that satisfies the specified predicate, or <c>null</c> if no such element exists.
    /// </summary>
    /// <param name="source">The list of integers to search through.</param>
    /// <param name="predicate">The predicate function used to test each element.</param>
    /// <returns>
    /// The first element that matches the predicate, or <c>null</c> if no matching element is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="predicate"/> is <c>null</c>.
    /// </exception>
    public static int? FirstPossibleNeighborOrNull(this List<int> source, Func<int, bool> predicate)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        foreach (int element in source)
        {
            if (predicate(element))
            {
                return element;
            }
        }

        return null;
    }
}