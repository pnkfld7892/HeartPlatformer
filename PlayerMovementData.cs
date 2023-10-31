using Godot;
using System;

[GodotClassName("PlayerMovementData")]
[Tool]
public partial class PlayerMovementData : Resource
{
    [Export]
    public  float Speed = 100.0f;
    [Export]
    public  float Acceleration = 600.0f;
    [Export]
    public  float Friction = 1000.0f;
    [Export]
    public  float JumpVelocity = -300.0f; 
    [Export]
    public float GravityScale = 1.0f;
    [Export]
    public float AirResistance = 200.0f;
    [Export]
    public float AirAcceleration = 400.0f;
    [Export]
    public float AirJumpMultiplier = 0.8f;
    

}
