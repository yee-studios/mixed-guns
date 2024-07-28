using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Entity entity;
    public Entity Entity => entity;
    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    private void Start()
    {
        GetComponent<AIDestinationSetter>().target = PlayerController.Instance.transform;
    }
}
