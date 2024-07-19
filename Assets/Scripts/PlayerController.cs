using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Singleton<PlayerController>
{
    #region cached components
    Rigidbody2D rb;
    PlayerInput input;
    Vector2 lastMove = new Vector2(0,1);
    #endregion

    [Header("Parameters")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float dashForce = 1000f;
    [SerializeField] int availableDashes = 0;
    [SerializeField] int maxDashes = 3;
    public int MaxDashes => maxDashes;
    [SerializeField] float dashReloadTime = 1f;
    [SerializeField] float currentDashReload = 0f;

    #region unity methods
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {

        if (availableDashes < maxDashes)
        {
            currentDashReload += Time.deltaTime;
            if (currentDashReload >= dashReloadTime)
            {
                currentDashReload = 0f;
                availableDashes++;
            }
        }
        DashChargeUIController.Instance.UpdateUnits(availableDashes, currentDashReload);

        Vector2 move = input.actions["move"].ReadValue<Vector2>().normalized;
        rb.AddForce(move * moveSpeed * Time.deltaTime);
        lastMove = move;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        float angleRad = Mathf.Atan2(
           transform.position.x - mousePos.x,
            mousePos.y - transform.position.y);
        float angleDeg = 180 / Mathf.PI * angleRad;
        transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        if (availableDashes > 0 && input.actions["dash"].WasPressedThisFrame())
        {
            rb.AddForce(dashForce * lastMove, ForceMode2D.Impulse);
            availableDashes--;
        }
    }
    #endregion
}
