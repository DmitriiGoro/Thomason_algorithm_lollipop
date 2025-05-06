namespace ThomasonAlgorithm.Core.Models;

/// <summary>
/// Represents a range (gap) of integer values with a specified lower and upper bound.
/// This is used to define a range of chord lengths to exclude or consider in graph generation.
/// </summary>
/// <param name="LeftBound">The inclusive lower bound of the gap range.</param>
/// <param name="RightBound">The inclusive upper bound of the gap range.</param>
public sealed record Gap(int LeftBound, int RightBound);