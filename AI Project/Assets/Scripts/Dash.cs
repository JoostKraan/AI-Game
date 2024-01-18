using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public Image dashFillImage; // Reference to the UI Image element for dash fill
    public PlayerMovement playerMovement; // Reference to your PlayerMovement script

    public float fillSpeed = 2f; // Adjust this to control the fill speed

    private float targetFillAmount = 0f;

    void Update()
    {
        // Check if dashFillImage or playerMovement is null before trying to access them
        if (dashFillImage != null && playerMovement != null)
        {
            // Update the UI image fill based on the remaining dash cooldown
            UpdateDashFill();
        }
        else
        {
            Debug.LogError("Dash script: dashFillImage or playerMovement is not assigned.");
        }
    }

    void UpdateDashFill()
    {
        // Assuming your PlayerMovement script has a property for remaining dash cooldown
        if (playerMovement != null)
        {
            float remainingCooldown = playerMovement.GetRemainingDashCooldown();

            // Calculate the fill amount using Mathf.SmoothDamp to smoothly interpolate between current and target fill amount
            float currentFillAmount = dashFillImage.fillAmount;
            targetFillAmount = 1f - remainingCooldown / playerMovement.sprintCooldown;
            float smoothDampVelocity = 0f;
            float fillAmount = Mathf.SmoothDamp(currentFillAmount, targetFillAmount, ref smoothDampVelocity, 1 / fillSpeed);

            // Update the UI image fill amount
            dashFillImage.fillAmount = fillAmount;
        }
        else
        {
            Debug.LogError("Dash script: playerMovement is not assigned.");
        }
    }
}