using System;
using UnityEngine;

public class SpeedItem : MonoBehaviour, iItem
{
    public static event Action<float> onSpeedCollected;
    public float speedMultiplier = 1.5f;
    public void Collect()
    {
        //we want to know the player movement script to know that we picked up the speed bost
        onSpeedCollected.Invoke(speedMultiplier);
        Destroy(gameObject);
    }
}
