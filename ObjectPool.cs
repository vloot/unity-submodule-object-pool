using UnityEngine;
using System.Collections.Generic;

// TODO keep track of all objects to support DespawnAll method
public class ObjectPool<T> where T : IPoolObject<T>
{
    private Queue<T> _objectQueue;

    private Transform _parent;
    private GameObject _prefab;
    private int _increseFactor;

    public ObjectPool(int initSize, GameObject prefab, Transform parent, int increseFactor = 5)
    {
        _objectQueue = new Queue<T>();

        _prefab = prefab;
        _parent = parent;
        _increseFactor = increseFactor;

        IncreaseSize(initSize);
    }

    public T Spawn(bool enable = true)
    {
        if (_objectQueue.Count == 0)
        {
            IncreaseSize(_increseFactor);
        }

        var obj = _objectQueue.Dequeue();

        if (enable)
        {
            obj.Enable();
        }

        return obj;
    }

    public T Spawn(Vector3 position, bool enable = true)
    {
        var obj = Spawn(enable);
        obj.GetTransform().position = position;
        return obj;
    }

    public void Despawn(T poolObject)
    {
        _objectQueue.Enqueue(poolObject);
        poolObject.Disable();
    }

    private void IncreaseSize(int increaseBy)
    {
        for (int i = 0; i < increaseBy; i++)
        {
            var poolObject = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _parent);

            var objectComponent = poolObject.GetComponent<T>(); // todo check for null?
            objectComponent.Disable();
            _objectQueue.Enqueue(objectComponent);
        }
    }
}
