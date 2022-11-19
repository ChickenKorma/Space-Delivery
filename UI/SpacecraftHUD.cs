using UnityEngine;
using TMPro;

public class SpacecraftHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _autoAlignText;
    [SerializeField] private TMP_Text _matchVelocityText;
    [SerializeField] private TMP_Text relativeVelocityText;

    [SerializeField] private Spacecraft _spacecraft;

    [SerializeField] private RectTransform _crosshairsRT;

    [SerializeField] private RectTransform _targetReticleRT;
    [SerializeField] private RectTransform _targetLockReticleRT;

    [SerializeField] private RectTransform _horizontalVelocityLineRT;
    [SerializeField] private RectTransform _verticalVelocityLineRT;

    private float _lineWidth;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;

        _lineWidth = _horizontalVelocityLineRT.sizeDelta.y;
    }

    private void Update()
    {
        UpdateSystems();

        UpdateReticles();

        UpdateCrosshairs();
    }

    // Updates the text of the systems status
    private void UpdateSystems()
    {
        _autoAlignText.text = "Auto Align: " + (_spacecraft.AutoAlignActive ? "On" : "Off");
        _matchVelocityText.text = "Match Velocity: " + (_spacecraft.MatchVelocityActive ? "On" : "Off");
    }

    // Hides or shows the correct reticle transforms and updates their position and size, also updates velocity markers when target is locked
    private void UpdateReticles()
    {
        if (_spacecraft.Target != null && _spacecraft.Target != _spacecraft.LockedTarget)
        {
            _targetReticleRT.gameObject.SetActive(true);

            UpdateReticle(_targetReticleRT, _spacecraft.Target);
        }
        else
        {
            _targetReticleRT.gameObject.SetActive(false);
        }

        if (_spacecraft.LockedTarget != null)
        {
            _targetLockReticleRT.gameObject.SetActive(true);

            UpdateReticle(_targetLockReticleRT, _spacecraft.LockedTarget);

            UpdateVelocityMarkers();
        }
        else
        {
            _targetLockReticleRT.gameObject.SetActive(false);
        }
    }

    // Sets the given rect transform position and size to cover the target body on the screen
    private void UpdateReticle(RectTransform reticleRT, CelestialBody targetBody)
    {
        Vector3 screenPos = _cam.WorldToScreenPoint(targetBody.Position);
        screenPos.z = 0;

        reticleRT.position = screenPos;

        // Calculate reticle size as proportional to the angular size of the target body as seen from the camera
        float size = Mathf.Clamp(Mathf.Atan2(targetBody.Radius, Vector3.Distance(targetBody.Position, _cam.transform.position)) * 60f * Mathf.Rad2Deg, 80, 900);

        reticleRT.sizeDelta = new Vector2(size, size);
    }

    // Updates horizontal and vertical arrows and forward velocity text depending on current relative velocity of spacecraft
    private void UpdateVelocityMarkers()
    {
        UpdateMarker(_horizontalVelocityLineRT, _spacecraft.RelativeVelocity.x);
        UpdateMarker(_verticalVelocityLineRT, _spacecraft.RelativeVelocity.y);

        relativeVelocityText.text = _spacecraft.RelativeVelocity.z.ToString("F0") + "m/s";
    }

    // Sets the size and direction of the given line transform depending on the given value
    private void UpdateMarker(RectTransform lineRT, float value)
    {
        if (Mathf.Abs(value) > 0.2f)
        {
            lineRT.gameObject.SetActive(true);

            lineRT.sizeDelta = new Vector2(Mathf.Abs(value) * 5, _lineWidth);
            lineRT.localScale = new Vector3(Mathf.Sign(value), 1, 1);
        }
        else
        {
            lineRT.gameObject.SetActive(false);
        }
    }

    // Calculates and sets the crosshair position to be 100 units in front of the spacecraft in the forward direction
    private void UpdateCrosshairs()
    {
        Vector3 screenPos = _cam.WorldToScreenPoint(_spacecraft.transform.position + (_spacecraft.transform.forward * 100));

        _crosshairsRT.position = screenPos;
    }
}
