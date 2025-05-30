using System;
using System.Collections.Generic;

namespace Hopeful.Utilities;

public readonly struct Graph<T>() where T : notnull
{
    private readonly Dictionary<T, List<T>> _values = [];

    public void AddVertex(T vertex)
        => _values.TryAdd(vertex, []);

    public void AddEdge(T fromVertex, T toVertex)
    {
        AddVertex(fromVertex);
        AddVertex(toVertex);
        _values[fromVertex].Add(toVertex);
        _values[toVertex].Add(fromVertex);
    }

    public List<T> DfsSort(T startVertex)
    {
        ArgumentNullException.ThrowIfNull(startVertex, nameof(startVertex));
        if (!_values.ContainsKey(startVertex)) throw new InvalidOperationException("Vertex not exist in Graph!");

        HashSet<T> visited = [];
        List<T> result = [];

        foreach (var vertex in _values.Keys)
            if (!visited.Contains(vertex))
                DfsVisit(vertex, visited, result);

        return result;
    }

    private void DfsVisit(T vertex, HashSet<T> visited, List<T> result)
    {
        visited.Add(vertex);
        result.Add(vertex);

        foreach (var neighbor in _values[vertex])
            if (!visited.Contains(neighbor))
                DfsVisit(neighbor, visited, result);
    }
}
