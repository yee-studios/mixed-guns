using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] SpriteMask mask;
    [SerializeField] HealthBar healthBar;
    float health = 0f;
    public float Health
    {
        get { return health; }
        set
        {
            health = Mathf.Clamp(value, 0f, maxHealth);
            Debug.Log(health);
            float h = health / maxHealth;
            healthBar.UpdateValue(h);
            mask.transform.rotation = Quaternion.identity;
            mask.transform.position = transform.position + new Vector3(0, h, 0);
            if (health < 0f) Die();
        }
    }

    [SerializeField] float maxHealth = 100f;
    public float MaxHealth => maxHealth;

    void Die()
    {

    }
}
