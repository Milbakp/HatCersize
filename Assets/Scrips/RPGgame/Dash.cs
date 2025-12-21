using UnityEngine;
using System.Collections;

public class Dash : MonoBehaviour
{
    public CharacterController controller;
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    private bool isDashing = false;

    public void DoDash()
    {
        if (!isDashing)
            StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        float time = 0f;

        while (time < dashDuration)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
    }
}