using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hopeful.Utilities;

/// <summary>
/// <para>Represents a graph data structure, that represent relationships between objects.</para>
/// Provide methods to perform sort operations on the graph.
/// </summary>
/// <typeparam name="T"> The type of the vertices in the graph.</typeparam>
public readonly struct Graph<T>() where T : notnull
{
    private readonly Dictionary<T, List<T>> _values = [];

    /// <summary>
    /// <para>Adds a vertex to the graph.</para>
    /// </summary>
    /// <param name="vertex">The vertex to add.</param>
    public void AddVertex(T vertex)
        => _values.TryAdd(vertex, []);

    public bool ContainsVertex(T vertex)
        => _values.ContainsKey(vertex);

    public IReadOnlyCollection<T> GetVertices()
        => _values.Keys.ToList().AsReadOnly();

    public IReadOnlyCollection<T> GetEdges(T vertex)
        => _values[vertex];

    public IReadOnlyCollection<T> GetVerticesWithoutEdges()
        => _values.Where(v => v.Value.Count == 0).Select(v => v.Key).ToList().AsReadOnly();

    /// <summary>
    /// <para>Adds an edge between two vertices in the graph.</para>
    /// </summary>
    /// <param name="fromVertex">The vertex to add the edge from.</param>
    /// <param name="toVertex">The vertex to add the edge to.</param>
    public void AddEdge(T fromVertex, T toVertex, bool bidirectionality = false)
    {
        AddVertex(fromVertex);
        AddVertex(toVertex);
        _values[fromVertex].Add(toVertex);
        if (bidirectionality) _values[toVertex].Add(fromVertex);
    }

    /// <summary>
    /// <para>Performs a depth-first search sort on the graph, starting from the specified vertex.</para>
    /// </summary>
    /// <param name="startVertex">The vertex to start the search from.</param>
    /// <returns>A list of vertices in the order they were visited.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="startVertex"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="startVertex"/> does not exist in the graph.</exception>
    public List<T> DfsSort(T startVertex)
    {
        ArgumentNullException.ThrowIfNull(startVertex, nameof(startVertex));
        if (!_values.ContainsKey(startVertex)) throw new InvalidOperationException("Vertex not exist in graph.");

        HashSet<T> visited = [];
        HashSet<T> recursionStack = [];
        List<T> result = [];

        foreach (var vertex in _values.Keys)
            if (!visited.Contains(vertex))
                if (!DfsVisit(vertex, visited, recursionStack, result)) throw new InvalidOperationException("Graph contains cycles.");

        result.Reverse();
        return result;
    }

    /// <summary>
    /// <para>Performs a depth-first search sort on the graph, starting from the vertex without edges.</para>
    /// </summary>
    /// <returns>A list of vertices in the order they were visited.</returns>
    public List<T> DfsSort()
        => DfsSort(GetVerticesWithoutEdges().First());

    private bool DfsVisit(T vertex, HashSet<T> visited, HashSet<T> recursionStack, List<T> result)
    {
        if (recursionStack.Contains(vertex)) return false;

        if (visited.Contains(vertex)) return true;

        visited.Add(vertex);
        recursionStack.Add(vertex);

        foreach (var neighbor in _values[vertex])
            if (!DfsVisit(neighbor, visited, recursionStack, result)) return false;

        recursionStack.Remove(vertex);
        result.Add(vertex);
        return true;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine("[GRAPH]");
        foreach (var vertex in GetVertices())
            builder.AppendLine($"Requirement: {vertex} -> Requiring: {string.Join(", ", GetEdges(vertex))}");
        return builder.ToString();
    }
}
