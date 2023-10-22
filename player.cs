using Godot;
using System;

public partial class player : CharacterBody2D
{
    public const float Speed = 100.0f;
    public const float JumpVelocity = -300.0f; // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public const float Acceleration = 600.0f;
    public const float Friction = 1000.0f;
    private AnimatedSprite2D playerSprite;

    public override void _Ready()
    {
        playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        ApplyGravity(ref velocity, delta);

        HandleJump(ref velocity);

        Vector2 inputAxis = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (inputAxis != Vector2.Zero)
        {
            HandleAcceleration(ref velocity, inputAxis, delta);
        }
        else
        {
            ApplyFriction(ref velocity, delta);
        }

        UpdateAnimations(inputAxis);
        Velocity = velocity;
        MoveAndSlide();
    }

    private void ApplyGravity(ref Vector2 velocity, double delta)
    {
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;
    }

    private void HandleJump(ref Vector2 velocity)
    {
        if (IsOnFloor())
        {
            if (Input.IsActionJustPressed("ui_up"))
                velocity.Y = JumpVelocity;
        }
        else
        {
            if (Input.IsActionJustReleased("ui_up") && velocity.Y < JumpVelocity / 2)
            {
                velocity.Y = JumpVelocity / 2;
            }
        }
    }

    private void HandleAcceleration(ref Vector2 velocity, Vector2 inputAxis, double delta)
    {
        velocity.X = Mathf.MoveToward(velocity.X,
                Speed * inputAxis.X,
                Acceleration * (float)delta);
        velocity.X = inputAxis.X * Speed;
    }

    private void ApplyFriction(ref Vector2 velocity, double delta)
    {
        velocity.X = Mathf.MoveToward(Velocity.X, 0, Friction * (float)delta);
    }


    private void UpdateAnimations(Vector2 inputAxis)
    {

        if (inputAxis != Vector2.Zero)
        {
            playerSprite.FlipH = inputAxis.X < 0; 
            playerSprite.Play("run");
        }
        else
        {
            playerSprite.Play("idle");
        }

        if(!IsOnFloor()){
            playerSprite.Play("jump");
        }
    }
}
