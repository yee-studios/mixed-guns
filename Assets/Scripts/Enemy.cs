using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Entity entity;
    public Entity Entity => entity;
    private void Awake()
    {
        entity = GetComponent<Entity>();
    }
}
