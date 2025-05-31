using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectPool<T> where T : MonoBehaviour
{
    private T _prefab;
    private bool _isExpandable;
    private Transform _container;

    private GameObject _ownContainer;
    private List<T> _pool;
    
    public ObjectPool(T prefab, bool isExpandable, int count)
    {
        _prefab = prefab;
        _isExpandable = isExpandable;

        _pool = new List<T>();
        CreateContainer();
        for (int i = 0; i < count; i++)
        {
            CreateObject();
        }
    }

    private void CreateContainer()
    {
        _ownContainer = new GameObject();
        _ownContainer.name = $"{_prefab.name} Pool Container";
    }

    private T CreateObject( bool isActive = false)
    {
        var createdObject = Object.Instantiate(_prefab, _ownContainer.transform);
        createdObject.GameObject().SetActive(isActive);
        _pool.Add(createdObject);
        return createdObject;
    }

    private bool HasFreeElement(out T element)
    {
        foreach (var obj in _pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                element = obj;
                return true;
            }
        }

        element = null;
        return false;
    }

    public T GetElement(Vector3 position)
    {
        if (HasFreeElement(out var element))
        {
            element.gameObject.transform.position = position;
            element.gameObject.SetActive(true);
            return element;
        }

        if (_isExpandable)
        {
            T createdObject = CreateObject(true);
            createdObject.transform.position = position;
            return createdObject;
        }

        throw new Exception($"Pool {_prefab.name} is out of free elements!");
    }
}
