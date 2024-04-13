# RexPool Unity

## Description

- Library used to quickly pool prefabs, RexPool will manage taking from the pool, and returning the pool will be handled by the gameObject itself

## Install package with manifest

- Copy and paste to manifest.json

```json
"com.pirexgames.rexpool": "https://github.com/PirexGames/RexPool-Unity#master",
```

## Requirement

- Requirement:
  - GameObject must be `prefab`, not GameObject in scene
  - GameObject must be constain `RexPoolObject` component

## APIs

### Use static class `RexPool.cs` to get gameObject from pool

- Note Param

```
/// <param name="prefab">Prefab to pooling</param>
/// <param name="activeObject">Set active gameObject to value</param>
/// <param name="addressableKey">Key addressable of prefab</param>
```

- Get GameObject From Pool:

```c#
public static GameObject GetGameObject(RexPoolObject prefab, bool activeObject = false)
```

- Get RexPoolObject From Pool:

```c#
public static RexPoolObject GetRexPoolObject(RexPoolObject prefab, bool activeObject = false)
```

- Get Type T Object From Pool with true false return and out var result:

```c#
public static bool GetGameObject<T>(RexPoolObject prefab, out T result, bool activeObject = false)
where T : RexPoolObject
```

- Get GameObject From Pool With Addressable Key:

```c#
public static async Task<GameObject> GetGameObject(string addressableKey, bool activeObject = false)
```

- Frees the object reference from the pool

```c#
public static void Release(this RexPoolObject go)
```

### How to return object to pool

- You can return object to pool with 2 ways: `SetActive(false)` gameObject or call method `ReturnToPool()` from RexPoolObject
