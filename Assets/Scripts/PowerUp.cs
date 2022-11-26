using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private string power;

    [SerializeField]
    private int timer;

    public string GetPowerUpType()
    {
        return power;
    }
    private void OnDestroy()
    {
        GameManager.instance.PowerUp(timer);
        
    }
}
