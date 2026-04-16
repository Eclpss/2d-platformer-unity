using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoadLevel : MonoBehaviour
{
    public float holdDuration = 1f; // how long u gonna hold it doawn
    public Image fillCircle;

    private float holdTimer = 0;
    private bool isHolding = false;

    public static event Action OnHoldComplete;

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;
            if (holdTimer >= holdDuration)
            {
                //load the next level
                OnHoldComplete.Invoke();
                ResetHold();
            }
        }
    }
    public void Onhold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
        } //it means we hold down the button
        else if (context.canceled)
        {
            //reset the hold 
            ResetHold();
        }

    }
    private void ResetHold()
    {
        isHolding = false;
        holdTimer = 0;
        fillCircle.fillAmount = 0;
    }
}
