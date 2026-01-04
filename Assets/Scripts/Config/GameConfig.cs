using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    [Header("Player Movement")]
    [Tooltip("Horizontal movement speed")]
    public float moveSpeed = 5f;

    [Tooltip("Jump force applied to player")]
    public float jumpForce = 250f;

    [Tooltip("Cooldown between jumps in seconds")]
    public float jumpCooldown = 0.8f;

    [Tooltip("Radius for ground detection")]
    public float groundCheckRadius = 0.2f;

    [Header("Power-Ups")]
    [Tooltip("Jump force multiplier for ExtraJump power-up")]
    public float extraJumpMultiplier = 1.5f;

    [Tooltip("Jump force multiplier for LessJump power-up")]
    public float lessJumpMultiplier = 0.5f;

    [Tooltip("Duration of power-up effects in seconds")]
    public float powerUpDuration = 5f;

    [Header("Camera")]
    [Tooltip("Camera follow smoothing speed")]
    public float cameraFollowSpeed = 5f;

    [Tooltip("Camera offset from player")]
    public Vector3 cameraOffset = new Vector3(0, 0, -10);

    [Tooltip("Distance camera looks ahead in movement direction")]
    public float lookAheadDistance = 3f;

    [Tooltip("Speed of look-ahead transition")]
    public float lookAheadSpeed = 2f;

    [Tooltip("Smooth time for camera movement")]
    public float cameraSmoothTime = 0.3f;

    [Header("Mobile Controls")]
    [Tooltip("Virtual joystick radius")]
    public float joystickRadius = 100f;

    [Tooltip("Joystick sensitivity multiplier")]
    public float joystickSensitivity = 1f;

    [Tooltip("Enable haptic feedback on mobile")]
    public bool enableHapticFeedback = true;

    [Header("Audio")]
    [Tooltip("Background music volume (0-1)")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;

    [Tooltip("Sound effects volume (0-1)")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("Game Balance")]
    [Tooltip("Star rating time multipliers")]
    public float threeStarMultiplier = 1.1f;
    public float twoStarMultiplier = 1.4f;
    public float oneStarMultiplier = 2.0f;
}
