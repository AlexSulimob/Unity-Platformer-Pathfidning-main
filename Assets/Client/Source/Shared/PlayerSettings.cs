using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Assets/Player settings")]
public class PlayerSettings : ScriptableObject 
{	
    [Header("Horizontal movement")]
    public float groundSpeed = 10f;
    public float groundDrag = 20f;
    public float slopeSpeed = 100f;
    public float airSpeed = 10f;
    public float airDrag = 20f;


    [Header("Jump")]
    public float JumpForce = 60f;
    public float LowJumpForce = 40f;
    [Range(0, 1)]
    public float JumpCutValue = 0.5f;
    public float coyoteTime = 0.2f;
    public float jumpBuffer = 0.2f;
    public float jumpCd = 0.4f;
    public int amountOfAirJumps = 1;

    [Header("Falling")]
    public float maxFallingSpeed = -50f;
    public float fallingGravity = 5f;
    public float jumpGravity = 4f;
    public float gravityThreshold = 0f;
    public float speedUpFallingGravity = 6f;

    [Header("Wall sliding")]
    public float wallSlideSpeed = -5f;
    public Vector2 wallJumpAngle;
    public float x_wallJumpforce = 50f;
    public float y_wallJumpforce = 50f;

    public float wallHangRange = 0.2f;
}

[CreateAssetMenu(menuName ="Assets/Physics settings")]
public class PhysSettings : ScriptableObject
{
    public LayerMask collisionLayer;
}