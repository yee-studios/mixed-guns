using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    #region cached components
    Rigidbody2D rb;
    PlayerInput input;
    #endregion

    [Header("Parameters")]
    [SerializeField] float moveSpeed = 10f;

    #region unity methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Vector2 move = input.actions["move"].ReadValue<Vector2>();
        rb.AddForce(move*moveSpeed);
    }
    #endregion
}
