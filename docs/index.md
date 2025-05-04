# Thomason Algorithm Documentation

Welcome to the documentation for **ThomasonAlgorithm.Core**, a .NET library for analyzing cubic graphs using Thomason's algorithm.

## Overview
ThomasonAlgorithm.Core is a .NET library focused on the generation and analysis of cubic graphs—graphs where each vertex has degree 3. At its core, the library provides tools for experimenting with Thomason's algorithm, particularly for discovering second Hamiltonian cycles using techniques like the Lollipop Algorithm.

Key features include:

- Generation of random cubic graphs with constraints on chord lengths.

- Analysis of Hamiltonian cycles and step-by-step execution of the Lollipop Algorithm.

- Tools for evaluating and recording the lengths of chords (edges not part of the Hamiltonian cycle) and their distribution.

- A flexible experiment pipeline for systematically testing large numbers of graph instances with varying parameters.

- Extension methods and utilities for working with graphs represented via adjacency matrices.

This library is well-suited for research and educational purposes related to Hamiltonian graph theory, and supports customization for testing different configurations and properties of chord sets in cubic graphs.

## Navigation
- [Getting Started](articles/getting-started.md)
- [API Reference](/api/toc.html)