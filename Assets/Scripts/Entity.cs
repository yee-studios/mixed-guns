using System;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    [SerializeField] SpriteMask mask;
    [SerializeField] HealthBar healthBar;
    [SerializeField] float health = 0f;
    [SerializeField] float regenRate = 1f;
    public bool invencibility = false;
    public float Health
    {
        get { return health; }
        set
        {
            if (invencibility && value < health) return;
            health = Mathf.Clamp(value, 0f, maxHealth);
            float h = health / maxHealth;
            if(healthBar) healthBar.UpdateValue(h);
            // TODO remove mask
            if (mask)
            {
                mask.transform.rotation = Quaternion.identity;
                mask.transform.position = transform.position + new Vector3(0, h, 0);
            }
            if (health <= 0f) Die();
        }
    }

    [SerializeField] float maxHealth = 100f;
    public float MaxHealth => maxHealth;

    private void Start()
    {
        if (healthBar == null) {
            healthBar = Instantiate(PrefabHolder.Instance.HealthBarPrefab);
            healthBar.target = this;
        }
        if (health <= 0f) health = maxHealth;
    }

    private void Update()
    {
        Health += regenRate * Time.deltaTime;
    }

    void Die()
    {
        OnDied?.Invoke();
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    public UnityEvent OnDied;
}
