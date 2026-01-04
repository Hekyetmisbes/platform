using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pooling system to avoid expensive Instantiate/Destroy calls.
/// Reduces garbage collection and improves performance for frequently spawned objects.
/// </summary>
/// <remarks>
/// Usage:
/// - Configure pools in the inspector with tags, prefabs, and initial sizes
/// - Spawn objects: ObjectPool.Instance.Spawn("PoolTag", position, rotation)
/// - Return objects: ObjectPool.Instance.Despawn(gameObject, "PoolTag")
/// - Objects are automatically parented to this pool's transform for organization
/// </remarks>
public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        [Tooltip("Unique identifier for this pool (e.g., 'JumpDust', 'DeathEffect')")]
        public string tag;

        [Tooltip("The prefab to pool")]
        public GameObject prefab;

        [Tooltip("Number of instances to pre-create on startup")]
        public int size = 10;

        [Tooltip("If true, pool can grow beyond initial size when needed")]
        public bool canGrow = true;

        [Tooltip("Maximum size the pool can grow to (0 = unlimited)")]
        public int maxSize = 0;
    }

    public static ObjectPool Instance { get; private set; }

    [Header("Pool Configuration")]
    [SerializeField]
    [Tooltip("List of object pools to create on startup")]
    private List<Pool> pools = new List<Pool>();

    [Header("Organization")]
    [SerializeField]
    [Tooltip("If true, pooled objects will be organized under category parent objects")]
    private bool organizeByCategory = true;

    // Dictionary to quickly lookup pools by tag
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    // Dictionary to track pool configurations
    private Dictionary<string, Pool> poolConfigs;

    // Dictionary to store category parent transforms for organization
    private Dictionary<string, Transform> categoryParents;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple ObjectPool instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePools();
    }

    /// <summary>
    /// Creates all configured pools and pre-instantiates objects.
    /// </summary>
    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolConfigs = new Dictionary<string, Pool>();
        categoryParents = new Dictionary<string, Transform>();

        foreach (Pool pool in pools)
        {
            // Validate pool configuration
            if (string.IsNullOrEmpty(pool.tag))
            {
                Debug.LogError("ObjectPool: Pool has no tag assigned. Skipping.");
                continue;
            }

            if (pool.prefab == null)
            {
                Debug.LogError($"ObjectPool: Pool '{pool.tag}' has no prefab assigned. Skipping.");
                continue;
            }

            // Create category parent if organized mode is enabled
            Transform parent = transform;
            if (organizeByCategory)
            {
                GameObject categoryObj = new GameObject($"[{pool.tag}]");
                categoryObj.transform.SetParent(transform);
                parent = categoryObj.transform;
                categoryParents[pool.tag] = parent;
            }

            // Initialize the pool queue
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Pre-instantiate objects
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreatePooledObject(pool.prefab, parent);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            poolConfigs.Add(pool.tag, pool);

            Debug.Log($"ObjectPool: Initialized pool '{pool.tag}' with {pool.size} objects.");
        }
    }

    /// <summary>
    /// Creates a new pooled object instance.
    /// </summary>
    private GameObject CreatePooledObject(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);

        // Add PooledObject component to track which pool it belongs to
        var pooledObj = obj.GetComponent<PooledObject>();
        if (pooledObj == null)
        {
            pooledObj = obj.AddComponent<PooledObject>();
        }

        return obj;
    }

    /// <summary>
    /// Spawns an object from the pool at the specified position and rotation.
    /// </summary>
    /// <param name="tag">The pool tag to spawn from</param>
    /// <param name="position">World position</param>
    /// <param name="rotation">World rotation</param>
    /// <returns>The spawned GameObject, or null if pool doesn't exist</returns>
    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"ObjectPool: Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        Pool config = poolConfigs[tag];

        GameObject objectToSpawn = null;

        // Try to get an object from the pool
        if (pool.Count > 0)
        {
            objectToSpawn = pool.Dequeue();
        }
        // If pool is empty, try to grow if allowed
        else if (config.canGrow)
        {
            if (config.maxSize == 0 || GetActiveCount(tag) < config.maxSize)
            {
                Transform parent = organizeByCategory ? categoryParents[tag] : transform;
                objectToSpawn = CreatePooledObject(config.prefab, parent);
                Debug.Log($"ObjectPool: Pool '{tag}' grew by 1 object (active: {GetActiveCount(tag) + 1})");
            }
            else
            {
                Debug.LogWarning($"ObjectPool: Pool '{tag}' reached max size ({config.maxSize})");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"ObjectPool: Pool '{tag}' is empty and cannot grow.");
            return null;
        }

        // Configure and activate the object
        if (objectToSpawn != null)
        {
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            // Store which pool this object came from
            var pooledObj = objectToSpawn.GetComponent<PooledObject>();
            if (pooledObj != null)
            {
                pooledObj.poolTag = tag;
            }
        }

        return objectToSpawn;
    }

    /// <summary>
    /// Returns an object to its pool.
    /// </summary>
    /// <param name="obj">The GameObject to return</param>
    /// <param name="tag">Optional: The pool tag. If not provided, will use PooledObject component</param>
    public void Despawn(GameObject obj, string tag = null)
    {
        if (obj == null)
        {
            Debug.LogWarning("ObjectPool: Attempted to despawn null object.");
            return;
        }

        // Try to get tag from PooledObject component if not provided
        if (string.IsNullOrEmpty(tag))
        {
            var pooledObj = obj.GetComponent<PooledObject>();
            if (pooledObj != null)
            {
                tag = pooledObj.poolTag;
            }
        }

        if (string.IsNullOrEmpty(tag))
        {
            Debug.LogWarning("ObjectPool: Cannot despawn object - no pool tag specified.");
            return;
        }

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"ObjectPool: Pool with tag '{tag}' doesn't exist. Destroying object instead.");
            Destroy(obj);
            return;
        }

        // Deactivate and return to pool
        obj.SetActive(false);

        // Ensure object is parented correctly
        if (organizeByCategory && categoryParents.ContainsKey(tag))
        {
            obj.transform.SetParent(categoryParents[tag]);
        }
        else
        {
            obj.transform.SetParent(transform);
        }

        poolDictionary[tag].Enqueue(obj);
    }

    /// <summary>
    /// Returns an object to its pool after a delay.
    /// Useful for particle effects that need time to complete.
    /// </summary>
    public void DespawnAfterDelay(GameObject obj, float delay, string tag = null)
    {
        if (obj != null)
        {
            StartCoroutine(DespawnCoroutine(obj, delay, tag));
        }
    }

    private System.Collections.IEnumerator DespawnCoroutine(GameObject obj, float delay, string tag)
    {
        yield return new WaitForSeconds(delay);
        Despawn(obj, tag);
    }

    /// <summary>
    /// Gets the number of active (spawned) objects for a pool.
    /// </summary>
    private int GetActiveCount(string tag)
    {
        if (!poolDictionary.ContainsKey(tag) || !categoryParents.ContainsKey(tag))
            return 0;

        int count = 0;
        foreach (Transform child in categoryParents[tag])
        {
            if (child.gameObject.activeSelf)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Gets statistics about a specific pool.
    /// </summary>
    public PoolStats GetPoolStats(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
            return new PoolStats();

        return new PoolStats
        {
            tag = tag,
            totalCount = categoryParents.ContainsKey(tag) ? categoryParents[tag].childCount : 0,
            availableCount = poolDictionary[tag].Count,
            activeCount = GetActiveCount(tag)
        };
    }

    /// <summary>
    /// Clears all pools and destroys all pooled objects.
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var kvp in poolDictionary)
        {
            while (kvp.Value.Count > 0)
            {
                GameObject obj = kvp.Value.Dequeue();
                if (obj != null)
                    Destroy(obj);
            }
        }

        poolDictionary.Clear();
        poolConfigs.Clear();

        Debug.Log("ObjectPool: All pools cleared.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Statistics for a pool.
    /// </summary>
    public struct PoolStats
    {
        public string tag;
        public int totalCount;
        public int availableCount;
        public int activeCount;
    }
}

/// <summary>
/// Component attached to pooled objects to track which pool they belong to.
/// Automatically added by ObjectPool.
/// </summary>
public class PooledObject : MonoBehaviour
{
    [HideInInspector]
    public string poolTag;

    /// <summary>
    /// Returns this object to its pool.
    /// </summary>
    public void ReturnToPool()
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Despawn(gameObject, poolTag);
        }
        else
        {
            Debug.LogWarning("PooledObject: ObjectPool instance not found. Destroying object instead.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Returns this object to its pool after a delay.
    /// </summary>
    public void ReturnToPoolAfterDelay(float delay)
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.DespawnAfterDelay(gameObject, delay, poolTag);
        }
    }
}
