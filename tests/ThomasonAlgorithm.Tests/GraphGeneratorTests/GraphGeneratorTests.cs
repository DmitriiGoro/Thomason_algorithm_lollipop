using ThomasonAlgorithm.Core.GraphGenerators;
using ThomasonAlgorithm.Core.Models;
using Xunit;
using System.Collections.Generic;

namespace ThomasonAlgorithm.Tests.GraphGeneratorTests
{
    public class GenerateGraphLimitedChordsTests
    {
        [Theory]
        [InlineData(20, 2, 5)]
        [InlineData(12, 2, 2)]
        [InlineData(100, 2, 5)]
        [InlineData(22, 3, 6)]
        public void Test_GenerateGraphLimitedChords_ChordLengthsWithinBounds(int n, int kLow, int kUp)
        {
            var graph = RandomCubicGraphGenerator.GenerateGraphLimitedChords(n, kLow, kUp);

            Assert.NotNull(graph);
            Assert.True(graph.IsCubic());
            Assert.True(graph.MaxChordLength == kUp || graph.MaxChordLength == kUp - 1);

            foreach (var kvp in graph.ChordsLengths)
            {
                var chordLength = kvp.Key;
                Assert.InRange(chordLength, kLow, kUp);
            }
        }
        
        [Theory]
        [InlineData(22, 2, 6)]
        [InlineData(24, 3, 7)]
        public void Test_GenerateGraphWithOddLengthChords_OnlyOddLengths(int n, int kLow, int kUp)
        {
            var graph = RandomCubicGraphGenerator.GenerateGraphWithOddLengthChords(n, kLow, kUp);

            Assert.NotNull(graph);
            Assert.True(graph.IsCubic());

            foreach (var chord in graph.ChordsLengths.Keys)
            {
                Assert.InRange(chord, kLow, kUp);
                Assert.True(chord % 2 == 1, $"Chord length {chord} is not odd.");
            }
        }
        
        public class GenerateGraphWithGapInChordLengthsTests
        {
            [Theory]
            [InlineData(20, 2, 10, 5, 7)] // Gap [5–7], valid lengths: 2–4 and 8–10
            [InlineData(30, 3, 9, 4, 6)] // Gap [4–6], valid lengths: 3 and 7–9
            public void Test_GenerateGraphWithGapInChordLengths_ExcludesGap(
                int n, int kLow, int kUp, int gapStart, int gapEnd)
            {
                var gap = new Gap(gapStart, gapEnd);

                var graph = RandomCubicGraphGenerator.GenerateGraphWithGapInChordLengths(n, kLow, kUp, gap);

                Assert.NotNull(graph);
                Assert.True(graph.IsCubic());

                foreach (var chordLength in graph.ChordsLengths.Keys)
                {
                    Assert.InRange(chordLength, kLow, kUp);
                    Assert.False(
                        chordLength >= gap.LeftBound && chordLength <= gap.RightBound,
                        $"Chord length {chordLength} is within the excluded gap range.");
                }
            }
        }
        
        [Theory]
        [InlineData(18, 2, 6, 4, 5)]
        [InlineData(20, 3, 7, 4, 6)]
        public void Test_GenerateGraphWithGapInChordLengths_ExcludesGap_And_WithinHalfN(
            int n, int kLow, int kUp, int gapStart, int gapEnd)
        {
            var gap = new Gap(gapStart, gapEnd);
            var maxAllowedChordLength = n / 2;

            var graph = RandomCubicGraphGenerator.GenerateGraphWithGapInChordLengths(n, kLow, kUp, gap);

            Assert.NotNull(graph);
            Assert.True(graph.IsCubic());

            foreach (var chordLength in graph.ChordsLengths.Keys)
            {
                Assert.InRange(chordLength, kLow, kUp);
                Assert.True(chordLength <= maxAllowedChordLength,
                    $"Chord length {chordLength} exceeds n/2 = {maxAllowedChordLength}");

                Assert.False(
                    chordLength >= gap.LeftBound && chordLength <= gap.RightBound,
                    $"Chord length {chordLength} is within the excluded gap range.");
            }
        }
        
        public static IEnumerable<object[]> ChordSetTestData =>
            new List<object[]>
            {
                new object[]
                {
                    18, 2, 4, new Dictionary<int, int> { [2] = 2, [3] = 3, [4] = 4 }
                },
                new object[]
                {
                    20, 2, 5, new Dictionary<int, int> { [2] = 4, [4] = 4, [5] = 2 }
                },
                new object[]
                {
                    8, 2, 3, new Dictionary<int, int> { [2] = 2, [3] = 2 }
                }
            };

        [Theory]
        [MemberData(nameof(ChordSetTestData))]
        public void GeneratesGraph_WithExactChordDistribution_Theory(
            int n,
            int kLow,
            int kUp,
            Dictionary<int, int> chordsLengthDict)
        {
            var graph = RandomCubicGraphGenerator.GenerateGraphFromExactChordsSet(n, kLow, kUp, chordsLengthDict);

            Assert.NotNull(graph);
            Assert.True(graph.IsCubic(), "Graph must be cubic (3-regular).");

            foreach (var kvp in chordsLengthDict)
            {
                int length = kvp.Key;
                int expectedCount = kvp.Value;

                Assert.True(graph.ChordsLengths.ContainsKey(length),
                    $"Expected chord length {length} not found in graph.");
                Assert.Equal(expectedCount, graph.ChordsLengths[length]);
            }

            var totalChords = 0;
            foreach (var count in graph.ChordsLengths.Values)
                totalChords += count;

            Assert.Equal(n / 2, totalChords);
        }
    }
}
