using System;
using UnityEngine;

public class Gems : MonoBehaviour, iItem
{
    public static event Action<int> onGemCollect;
    public int worth = 5;
    public void Collect()
    {
        onGemCollect.Invoke(worth);//so when we pick up this game another scirip will handle it 
        Destroy(gameObject);
    }
}
