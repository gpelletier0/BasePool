# Unity Object Pooler

Abstract class to simplifying the new object pooling workflow in Unity 2021.

## How to use:
```csharp
public class MySpawner : BasePool<MyObject> 
{
    [SerializeField] 
    private MyObject prefab;
    [SerializeField]
    private Transform parent;

    private void Start() 
    {
        parent ??= transform;

        Init(prefab, parent); // Initialize the pool
        
        var obj = Get(); // Pull from the pool
        Release(obj); // Release to the pool
    }
}
```

### Or

```csharp
public class MyObject : BasePool<MyObject>
{
    [SerializeField] 
    private MyObject prefab;
    [SerializeField]
    private Transform parent;

    private void Start() 
    {
        parent ??= transform;

        Init(prefab, parent); // Initialize the pool
        
        var obj = Get(); // Pull from the pool
        Release(obj); // Release to the pool
    }
}
```

## Overrides:

```csharp
    // Override Example
    protected override void ActionOnGet(MyObject obj) 
    {
        base.ActionOnGet(obj);
        obj.transform.position = Vector3.zero;
    }
```
