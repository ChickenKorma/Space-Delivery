using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _freeLookCam;

    private void OnEnable()
    {
        InputManager.Instance.recenterCameraEvent += RecenterCam;
    }

    private void OnDisable()
    {
        InputManager.Instance.recenterCameraEvent -= RecenterCam;
    }

    // Sets the recententering functions for x and y axes of the free look camera depending on given bool
    private void RecenterCam(bool recenter)
    {
        _freeLookCam.m_RecenterToTargetHeading.m_enabled = recenter;
        _freeLookCam.m_YAxisRecentering.m_enabled = recenter;
    }
}
