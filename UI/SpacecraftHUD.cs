using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpacecraftHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text autoAlignText;
    [SerializeField] private TMP_Text matchVelocityText;

    [SerializeField] private Spacecraft spacecraft;

    [SerializeField] private RectTransform targetReticle;
    [SerializeField] private RectTransform targetLockReticle;

    [SerializeField] private TMP_Text relativeVelocityText;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        autoAlignText.text = "Auto Align: " + (spacecraft.autoAlignActive ? "On" : "Off");
        matchVelocityText.text = "Match Velocity: " + (spacecraft.matchVelocityActive ? "On" : "Off");

        if(spacecraft.target != null && spacecraft.target != spacecraft.lockedTarget)
        {
            targetReticle.gameObject.SetActive(true);

            Vector3 screenPos = cam.WorldToScreenPoint(spacecraft.target.Position);
            screenPos.z = 0;

            targetReticle.position = screenPos;
        }
        else
        {
            targetReticle.gameObject.SetActive(false);
        }

        if(spacecraft.lockedTarget != null)
        {
            targetLockReticle.gameObject.SetActive(true);

            Vector3 screenPos = cam.WorldToScreenPoint(spacecraft.lockedTarget.Position);
            screenPos.z = 0;

            targetLockReticle.position = screenPos;
        }
        else
        {
            targetLockReticle.gameObject.SetActive(false);
        }
    }
}
