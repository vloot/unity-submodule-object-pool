using System;
using System.Collections.Generic;
using Game.Core.Diagnostics;
using UnityEngine;

public sealed class ObjectPool<T> where T : Component, IPoolObject
{
    private readonly Queue<T> _idle = new();
    private readonly HashSet<T> _spawned = new();
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly int _increaseFactor;
    private readonly Action<T> _onCreated;

    public ObjectPool(int initialSize, T prefab, Transform parent, int increaseFactor = 5, Action<T> onCreated = null)
    {
        _prefab = prefab;
        _parent = parent;
        _increaseFactor = increaseFactor;
        _onCreated = onCreated;

        IncreaseSize(initialSize);
    }

    public T Spawn(bool enable = true)
    {
        if (_idle.Count == 0) IncreaseSize(_increaseFactor);

        var poolObject = _idle.Dequeue();
        _spawned.Add(poolObject);

        if (enable) poolObject.Enable();

        return poolObject;
    }

    public T Spawn(Vector3 position, bool enable = true)
    {
        var poolObject = Spawn(enable);
        poolObject.transform.position = position;

        return poolObject;
    }

    public void Despawn(T poolObject)
    {
        if (!_spawned.Remove(poolObject))
        {
            return;
        }

        Recycle(poolObject);
    }

    public void DespawnAll()
    {
        foreach (var poolObject in _spawned)
        {
            Recycle(poolObject);
        }

        _spawned.Clear();
    }

    private void Recycle(T poolObject)
    {
        poolObject.Disable();
        poolObject.transform.SetParent(_parent, false);

        _idle.Enqueue(poolObject);
    }

    private void IncreaseSize(int increaseBy)
    {
        for (var i = 0; i < increaseBy; i++)
        {
            var poolObject = UnityEngine.Object.Instantiate(_prefab, _parent);

            _onCreated?.Invoke(poolObject);
            poolObject.Disable();

            _idle.Enqueue(poolObject);
        }
    }
}
