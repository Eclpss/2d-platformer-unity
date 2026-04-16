using UnityEngine;
using System;

public class HealthItem : MonoBehaviour, iItem
{
    public int healAmount = 1;
    public static event Action<int> onHealthCollect; 
    public void Collect()
    {

        onHealthCollect.Invoke(healAmount);
        Destroy(gameObject);
    }
}
