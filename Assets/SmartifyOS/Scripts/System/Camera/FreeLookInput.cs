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

    private Vector2 startRotation;

    private Coroutine rotateCoroutine;
    private Coroutine radiusCoroutine;

    [SerializeField] private float triggerTime = 0.2f;
    private float triggerTimer;

    private void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        freeLookCamera.m_XAxis.m_InputAxisName = "";
        freeLookCamera.m_YAxis.m_InputAxisName = "";

        startRotation = new Vector2(freeLookCamera.m_XAxis.Value, freeLookCamera.m_YAxis.Value);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !Utilities.IsOverUI())
        {
            if (triggerTimer > 0)
            {
                triggerTimer -= Time.deltaTime;
                freeLookCamera.m_XAxis.m_InputAxisValue = 0;
                freeLookCamera.m_YAxis.m_InputAxisValue = 0;
            }
            else
            {
                freeLookCamera.m_XAxis.m_InputAxisValue = -Input.GetAxis(XAxisName);
                freeLookCamera.m_YAxis.m_InputAxisValue = -Input.GetAxis(YAxisName);
            }
        }
        else
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = 0;
            freeLookCamera.m_YAxis.m_InputAxisValue = 0;
        }

        if (Input.GetMouseButtonUp(0))
        {
            triggerTimer = triggerTime;
        }
    }

    /// <summary>
    /// Sets the orbit radius of the camera
    /// </summary>
    /// <param name="radius">Radius of the orbit</param>
    /// <param name="index">Index of the orbit (0: Top, 1: Middle, 2: Bottom)</param>
    /// <param name="duration">Time to change the radius in seconds</param>
    public void SetOrbitRadius(float radius, int index, float duration = 0.5f)
    {
        if (radiusCoroutine != null)
            StopCoroutine(radiusCoroutine);

        radiusCoroutine = StartCoroutine(SetOrbitRadiusSmoothly(radius, index, duration));
    }

    /// <summary>
    /// Sets the camera view to follow the car
    /// </summary>
    public void FollowCamView()
    {
        SetRotation(new Vector2(180f, 0.67f));
    }

    /// <summary>
    /// Resets the camera view to the start position
    /// </summary>
    public void ResetCamView()
    {
        SetRotation(startRotation);
    }

    /// <summary>
    /// Smoothly rotates the camera to the target rotation
    /// </summary>
    /// <param name="targetRotation">CinemachineFreeLook Rotation XAxis and YAxis values</param>
    /// <param name="duration">Time to rotate in seconds</param>
    public void SetRotation(Vector2 targetRotation, float duration = 0.5f)
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(SetRotationSmoothly(targetRotation, duration));
    }

    private IEnumerator SetRotationSmoothly(Vector2 targetRotation, float duration = 0.5f)
    {
        float elapsedTime = 0f;
        Vector2 startRotation = new Vector2(freeLookCamera.m_XAxis.Value, freeLookCamera.m_YAxis.Value);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            freeLookCamera.m_XAxis.Value = Mathf.Lerp(startRotation.x, targetRotation.x, t);
            freeLookCamera.m_YAxis.Value = Mathf.Lerp(startRotation.y, targetRotation.y, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure exact target values at the end
        freeLookCamera.m_XAxis.Value = targetRotation.x;
        freeLookCamera.m_YAxis.Value = targetRotation.y;
    }

    private IEnumerator SetOrbitRadiusSmoothly(float targetRadius, int index, float duration = 0.5f)
    {
        float elapsedTime = 0f;
        float startRadius = freeLookCamera.m_Orbits[index].m_Radius;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            freeLookCamera.m_Orbits[index].m_Radius = Mathf.Lerp(startRadius, targetRadius, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure exact target values at the end
        freeLookCamera.m_Orbits[index].m_Radius = targetRadius;
    }
}