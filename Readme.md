# RexPool Unity

## Description

- Library used to quickly pool prefabs, RexPool will manage taking from the pool, and returning the pool will be handled by the gameObject itself

## Install package with manifest

- Copy and paste to manifest.json

```json
"com.pirexgames.rexpool": "https://github.com/PirexGames/RexPool-Unity.git#1.0.5",
```

## Requirement

- Requirement:
  - GameObject must be `prefab`, not GameObject in scene
  - GameObject must be constain `GameObject` component
- Addressable Pool GameObject:
  - Must be add script define `REXPOOL_ADDRESSABLE` to use API Get Addressable Pool GameObject

## APIs

### Use static class `RexPoolManager.cs` to get gameObject from pool

- Note Param

```
/// <param name="prefab">Prefab to pooling</param>
/// <param name="activeObject">Set active gameObject to value</param>
/// <param name="addressableKey">Key addressable of prefab</param>
```

- Prepair gameObject for pool with amount:

```c#
// with prefab
public static void Prepair(GameObject prefab, int amount);

// with addressable key
public static async Task Prepair(string addressableKey, int amount);
```

- Clean up type prefab in pool:

```c#
// with prefab
public static void CleanUp(GameObject prefab);

// with addressable key
public static void CleanUp(string addressableKey);
```

- Get GameObject From Pool:

```c#
// with prefab
public static GameObject Take(GameObject prefab, bool activeObject = false);

// with addressable key
public static async Task<GameObject> Take(string addressableKey, bool activeObject = false)
```

- Get GameObject From Pool:

```c#
public static GameObject TakeRPO(GameObject prefab, bool activeObject = false);
```

- Get Type T Object From Pool:

```c#
// with prefab
public static T Take<T>(GameObject prefab, bool activeObject = false)
where T : MonoBehaviour;

//with addressable
public static async Task<T> Take<T>(string addressableKey, bool activeObject = false)
where T : MonoBehaviour;
```

### How to return object to pool

- You can return object to pool with 2 ways: `SetActive(false)` gameObject or call method `ReturnToPool()` from GameObject

### How to release object

- When gameObject is destroy, it will auto release in pool
