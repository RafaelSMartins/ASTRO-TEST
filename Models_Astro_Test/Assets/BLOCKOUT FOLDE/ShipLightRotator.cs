using UnityEngine;

/// <summary>
/// Rotates a Directional Light around the X axis over time to fake the sensation
/// that the spaceship is moving/rotating (the "sun" sweeping or swaying as the
/// ship pitches/rolls), without actually moving the ship or the camera.
///
/// Attach this to the Directional Light GameObject.
/// </summary>
[DisallowMultipleComponent]
public class ShipLightRotator : MonoBehaviour
{
    public enum Mode
    {
        // Light spins continuously around X, like a ship in a slow endless roll.
        Continuous,
        // Light sways back and forth between -angle and +angle, like a ship
        // gently pitching/banking (recommended for a subtle "we're moving" feel).
        Oscillate
    }

    [Header("Mode")]
    [Tooltip("Continuous = keeps spinning forever. Oscillate = sways back and forth between -Angle and +Angle.")]
    public Mode mode = Mode.Oscillate;

    [Header("Speed")]
    [Tooltip("How fast the rotation happens. Degrees/second in Continuous mode, swing speed in Oscillate mode.")]
    [Min(0f)]
    public float speed = 10f;

    [Header("Oscillate Settings")]
    [Tooltip("Maximum degrees away from the starting X rotation, used only in Oscillate mode.")]
    [Min(0f)]
    public float angle = 15f;

    [Tooltip("If true, the sway follows a smooth sine wave. If false, it linearly ping-pongs (sharper direction change).")]
    public bool smoothSway = true;

    [Header("Axis")]
    [Tooltip("Rotate in local space (relative to the light's current rotation) instead of world space.")]
    public bool useLocalSpace = true;

    private float startX;
    private float currentOffset;

    private void Start()
    {
        // Remember the initial X rotation so Oscillate mode sways around it,
        // and Continuous mode keeps the original Y/Z framing.
        Vector3 startEuler = useLocalSpace ? transform.localEulerAngles : transform.eulerAngles;
        startX = startEuler.x;
    }

    private void Update()
    {
        switch (mode)
        {
            case Mode.Continuous:
                currentOffset += speed * Time.deltaTime;
                break;

            case Mode.Oscillate:
                if (smoothSway)
                {
                    // Sine wave: smooth, ship-like drifting motion.
                    currentOffset = Mathf.Sin(Time.time * speed * Mathf.Deg2Rad) * angle;
                }
                else
                {
                    // PingPong: linear back-and-forth sweep.
                    currentOffset = Mathf.PingPong(Time.time * speed, angle * 2f) - angle;
                }
                break;
        }

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        Vector3 euler = useLocalSpace ? transform.localEulerAngles : transform.eulerAngles;
        euler.x = startX + currentOffset;

        if (useLocalSpace)
            transform.localEulerAngles = euler;
        else
            transform.eulerAngles = euler;
    }
}
