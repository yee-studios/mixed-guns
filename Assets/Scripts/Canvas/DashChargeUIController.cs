using System.Collections.Generic;
using UnityEngine;

public class DashChargeUIController : Singleton<DashChargeUIController>
{
    [Header("Prefabs")]
    [SerializeField] ChargeUIUnit dashChargePrefab;
    [Header("Parameters")]
    [SerializeField] Vector2 offset = new Vector2(10, 0);
    [SerializeField] Vector2 startPos = new Vector2(25, 25);

    #region Cache
    List<ChargeUIUnit> chargeUnits = new();
    #endregion

    public void UpdateUnits(int currentDash, float amount)
    {
        for (int i = 0; i < chargeUnits.Count; i++)
        {
            if(i == currentDash)
                chargeUnits[i].UpdateIndicator(amount);
            else
            chargeUnits[i].UpdateIndicator(i < currentDash ? 1 : 0);
        }
    }


    protected void Start()
    {
        for (int i = 0; i < PlayerController.Instance.MaxDashes; i++)
        {
            ChargeUIUnit unit = Instantiate(dashChargePrefab, transform);
            unit.transform.position = startPos + (offset * i);
            chargeUnits.Add(unit);
        }
    }
}
