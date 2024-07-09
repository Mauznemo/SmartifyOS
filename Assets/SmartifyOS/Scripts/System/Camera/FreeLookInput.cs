using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class FreeLookInput : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;

    private string XAxisName = "Mouse X";
    private string YAxisName = "Mouse Y";

    public float resetDuration = 0.5f; // Duration for resetting the rotation

    private Coroutine resetCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        freeLookCamera.m_XAxis.m_InputAxisName = "";
        freeLookCamera.m_YAxis.m_InputAxisName = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !Utilities.IsOverUI())
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = -Input.GetAxis(XAxisName);
            freeLookCamera.m_YAxis.m_InputAxisValue = -Input.GetAxis(YAxisName);
        }
        else
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = 0;
            freeLookCamera.m_YAxis.m_InputAxisValue = 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {

                SetRotation(new Vector2(180f, 0.6f));
            }
        }
    }

    public void FollowCamView()
    {
        SetRotation(new Vector2(180f, 0.67f));
    }

    public void SetRotation(Vector2 targetRotation)
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetRotationSmoothly(targetRotation));
    }

    IEnumerator ResetRotationSmoothly(Vector2 targetRotation)
    {
        float elapsedTime = 0f;
        Vector2 startRotation = new Vector2(freeLookCamera.m_XAxis.Value, freeLookCamera.m_YAxis.Value);

        while (elapsedTime < resetDuration)
        {
            float t = elapsedTime / resetDuration;
            freeLookCamera.m_XAxis.Value = Mathf.Lerp(startRotation.x, targetRotation.x, t);
            freeLookCamera.m_YAxis.Value = Mathf.Lerp(startRotation.y, targetRotation.y, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure exact target values at the end
        freeLookCamera.m_XAxis.Value = targetRotation.x;
        freeLookCamera.m_YAxis.Value = targetRotation.y;
    }
}