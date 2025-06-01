using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Object pool for reusing MonoBehaviour instances to optimize performance.
/// Manages a pool of objects instantiated from a prefab, optionally expandable.
/// </summary>
/// <typeparam name="T">Type of the pooled objects, must be a MonoBehaviour.</typeparam>
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

    /// <summary>
    /// Creates a new empty GameObject to act as a container for the pooled objects.
    /// </summary>
    private void CreateContainer()
    {
        _ownContainer = new GameObject();
        _ownContainer.name = $"{_prefab.name} Pool Container";
    }

    /// <summary>
    /// Creates a new object, adds it to the pool, and optionally activates it.
    /// </summary>
    /// <param name="isActive">Dictates whether the object will be active at creation.</param>
    /// <returns>The created pooled object.</returns>
    private T CreateObject( bool isActive = false)
    {
        var createdObject = Object.Instantiate(_prefab, _ownContainer.transform);
        createdObject.GameObject().SetActive(isActive);
        _pool.Add(createdObject);
        return createdObject;
    }

    /// <summary>
    /// Checks if there is a free element in the pool.
    /// </summary>
    /// <param name="element">Outputs the available object if found, otherwise null.</param>
    /// <returns>True if a free object was found, otherwise false.</returns>
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

    /// <summary>
    /// Gets an active object from the pool at the specified position.
    /// If none are available and pool is expandable, creates a new object.
    /// </summary>
    /// <param name="position">World position to place the object.</param>
    /// <returns>Active pooled object ready for use.</returns>
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
