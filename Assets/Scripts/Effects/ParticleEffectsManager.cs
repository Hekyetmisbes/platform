using UnityEngine;

public enum ParticleEffectType
{
    Jump,
    Land,
    Death,
    Finish,
    Star
}

/// <summary>
/// Centralized particle spawning with optional ObjectPool usage.
/// Assign effect prefabs for each key gameplay event and call Play().
/// </summary>
public class ParticleEffectsManager : MonoBehaviour
{
    public static ParticleEffectsManager Instance { get; private set; }

    [Header("Effect Prefabs")]
    [SerializeField] private GameObject jumpEffect;
    [SerializeField] private GameObject landEffect;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject finishEffect;
    [SerializeField] private GameObject starEffect;

    [Header("Settings")]
    [SerializeField] private Transform defaultParent;
    [SerializeField] private float fallbackDespawnTime = 2f;

    private ObjectPool pool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        pool = ObjectPool.Instance != null ? ObjectPool.Instance : FindObjectOfType<ObjectPool>();
    }

    public void Play(ParticleEffectType type, Vector3 position, Vector3? normal = null)
    {
        GameObject prefab = GetPrefab(type);
        if (prefab == null) return;

        string tag = type.ToString();
        Quaternion rotation = normal.HasValue ? Quaternion.LookRotation(Vector3.forward, normal.Value) : prefab.transform.rotation;

        // Prefer pooled effect if pool and tag exist; otherwise fallback to instantiation.
        GameObject effectObject = pool != null ? pool.Spawn(tag, position, rotation) : null;
        if (effectObject == null)
        {
            effectObject = Instantiate(prefab, position, rotation, defaultParent);
        }

        var particleSystem = effectObject.GetComponent<ParticleSystem>();
        float lifetime = fallbackDespawnTime;
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            lifetime = main.duration + main.startLifetime.constantMax;
            particleSystem.Play();
        }

        if (pool != null && effectObject.activeSelf)
        {
            pool.DespawnAfterDelay(effectObject, lifetime, tag);
        }
        else
        {
            Destroy(effectObject, lifetime);
        }
    }

    private GameObject GetPrefab(ParticleEffectType type)
    {
        return type switch
        {
            ParticleEffectType.Jump => jumpEffect,
            ParticleEffectType.Land => landEffect,
            ParticleEffectType.Death => deathEffect,
            ParticleEffectType.Finish => finishEffect,
            ParticleEffectType.Star => starEffect,
            _ => null
        };
    }
}
