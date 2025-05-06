# Thomason Algorithm (lollipop)
ThomasonAlgorithm.Core is a .NET library designed for the analysis of Hamiltonian cubic graphs (3-regular graphs containing at least one Hamiltonian cycle) with a focus on Thomason's algorithmic approaches. The library specializes in graphs where:

The primary Hamiltonian cycle follows a sequential path:
0 → 1 → 2 → ... → n-1 → 0
(where n is the number of vertices).

Cubic constraints are enforced: every vertex has exactly three edges.

Key features include:

- Generation of random cubic graphs with constraints on chord lengths.

- Analysis of Hamiltonian cycles and step-by-step execution of the Lollipop Algorithm.

- Tools for evaluating and recording the lengths of chords (edges not part of the Hamiltonian cycle) and their distribution.

- A flexible experiment pipeline for systematically testing large numbers of graph instances with varying parameters.

- Extension methods and utilities for working with graphs represented via adjacency matrices.

This library is well-suited for research and educational purposes related to Hamiltonian graph theory, and supports customization for testing different configurations and properties of chord sets in cubic graphs.
