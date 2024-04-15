# RexPool Unity

## Description

- Library used to quickly pool prefabs, RexPool will manage taking from the pool, and returning the pool will be handled by the gameObject itself

## Install package with manifest

- Copy and paste to manifest.json

```json
"com.RexPool": "https://github.com/PirexGames/RexPool-Unity.git#1.0.3",
```

## Requirement

- Requirement:
  - GameObject must be `prefab`, not GameObject in scene
  - GameObject must be constain `RexPoolObject` component
- Addressable Pool GameObject:
  - Must be add script define `REXPOOL_ADDRESSABLE` to use API Get Addressable Pool GameObject

## APIs

### Use static class `RexPool.cs` to get gameObject from pool

- Note Param

```
/// <param name="prefab">Prefab to pooling</param>
/// <param name="activeObject">Set active gameObject to value</param>
/// <param name="addressableKey">Key addressable of prefab</param>
```

- Prepair gameObject for pool with amount:

```c#
// with prefab
public static void Prepair(RexPoolObject prefab, int amount);

// with addressable key
public static async Task Prepair(string addressableKey, int amount);
```

- Clean up type prefab in pool:

```c#
// with prefab
public static void CleanUp(RexPoolObject prefab);

// with addressable key
public static void CleanUp(string addressableKey);
```

- Get GameObject From Pool:

```c#
// with prefab
public static GameObject Take(RexPoolObject prefab, bool activeObject = false);

// with addressable key
public static async Task<GameObject> Take(string addressableKey, bool activeObject = false)
```

- Get RexPoolObject From Pool:

```c#
public static RexPoolObject TakeRPO(RexPoolObject prefab, bool activeObject = false);
```

- Get Type T Object From Pool:

```c#
// with prefab
public static T Take<T>(RexPoolObject prefab, bool activeObject = false)
where T : RexPoolObject;

//with addressable
public static async Task<T> Take<T>(string addressableKey, bool activeObject = false)
where T : RexPoolObject;
```

- Frees the object reference from the pool

```c#
public static void Release(this RexPoolObject go);
```

### How to return object to pool

- You can return object to pool with 2 ways: `SetActive(false)` gameObject or call method `ReturnToPool()` from RexPoolObject
