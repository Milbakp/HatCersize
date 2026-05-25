using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedSizeStack<T> : IEnumerable<T>
{
    private readonly LinkedList<T> _list = new LinkedList<T>();
    private readonly int _capacity;

    public int Count => _list.Count;
    public int Capacity => _capacity;

    public FixedSizeStack(int capacity = 10)
    {
        if (capacity <= 0) 
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
            
        _capacity = capacity;
    }

    /// <summary>
    /// Pushes an item onto the stack. Discards the oldest item if the stack is full.
    /// </summary>
    public void Push(T item)
    {
        // Add to the front (top of the stack)
        _list.AddFirst(item);

        // If we exceed capacity, remove from the back (bottom of the stack / oldest)
        //Debug.Log(_list.Count + "" + _capacity + "");
        if (_list.Count > _capacity)
        {
            _list.RemoveLast();
        }
    }

    /// <summary>
    /// Pops the newest item off the stack.
    /// </summary>
    public T Pop()
    {
        if (_list.Count == 0)
            throw new InvalidOperationException("Stack is empty.");

        T value = _list.First.Value;
        _list.RemoveFirst();
        return value;
    }

    /// <summary>
    /// Looks at the newest item without removing it.
    /// </summary>
    public T Peek()
    {
        if (_list.Count == 0)
            throw new InvalidOperationException("Stack is empty.");

        return _list.First.Value;
    }

    public T lastItem()
    {
        return _list.Last.Value;
    }
    
    public bool atCapacity()
    {
        return _list.Count == _capacity;
    }

    public bool isEmpty()
    {
        return _list.Count == 0;
    }

    public void Clear() => _list.Clear();

    // Allows iterating through the stack from newest to oldest
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}