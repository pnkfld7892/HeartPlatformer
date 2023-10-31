using Godot;
using System;

public partial class player : CharacterBody2D
{

    [Export]
    public PlayerMovementData MovementData { get; set; }


    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private AnimatedSprite2D playerSprite;
    private Timer coyoteJumpTimer;
    private bool airJump = false;
    private bool justWallJumped = false;

    public override void _Ready()
    {
        playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        coyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        ApplyGravity(ref velocity, delta);

        HandleWallJump(ref velocity);
        HandleJump(ref velocity);

        Vector2 inputAxis = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (inputAxis != Vector2.Zero)
        {
            HandleAcceleration(ref velocity, inputAxis, delta);
            HandleAirAcceleration(ref velocity, inputAxis, delta);
        }
        else
        {
            if (IsOnFloor())
            {
                ApplyFriction(ref velocity, delta);
            }
            else
            {
                ApplyAirResistance(ref velocity, delta);
            }
        }



        UpdateAnimations(inputAxis);
        Velocity = velocity;
        var wasOnFloor = IsOnFloor();
        MoveAndSlide();
        var justLeftLedge = wasOnFloor && !IsOnFloor() && velocity.Y >= 0;
        if (justLeftLedge)
        {
            coyoteJumpTimer.Start();
        }
        justWallJumped = false;
    }

    private void ApplyAirResistance(ref Vector2 velocity, double delta)
    {
        velocity.X = Mathf.MoveToward(velocity.X, 0, MovementData.AirResistance * (float)delta);
    }

    private void ApplyGravity(ref Vector2 velocity, double delta)
    {
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta * MovementData.GravityScale;
    }

    private void HandleWallJump(ref Vector2 velocity)
    {
        if (!IsOnWallOnly())
        {
            return;
        }
        var wallNormal = GetWallNormal();
        if (Input.IsActionJustPressed("ui_left") && wallNormal == Vector2.Left)
        {
            velocity.X = wallNormal.X * MovementData.Speed;
            velocity.Y = MovementData.JumpVelocity;
        }

        if (Input.IsActionJustPressed("ui_right") && wallNormal == Vector2.Right)
        {
            velocity.X = wallNormal.X * MovementData.Speed;
            velocity.Y = MovementData.JumpVelocity;
        }
        justWallJumped = true;

    }

    private void HandleJump(ref Vector2 velocity)
    {
        if (IsOnFloor())
        {
            airJump = true;
        }
        if (IsOnFloor() || coyoteJumpTimer.TimeLeft > 0.0)
        {
            if (Input.IsActionJustPressed("ui_up"))
                velocity.Y = MovementData.JumpVelocity;
        }

        if (!IsOnFloor())
        {
            if (Input.IsActionJustReleased("ui_up") && velocity.Y < MovementData.JumpVelocity / 2)
            {
                velocity.Y = MovementData.JumpVelocity / 2;
            }
            if (Input.IsActionJustPressed("ui_up") && airJump && !justWallJumped)
            {
                velocity.Y = MovementData.JumpVelocity * MovementData.AirJumpMultiplier;
                airJump = false;
            }
        }
    }

    private void HandleAcceleration(ref Vector2 velocity, Vector2 inputAxis, double delta)
    {
        if (!IsOnFloor())
            return;
        velocity.X = Mathf.MoveToward(velocity.X,
                MovementData.Speed * inputAxis.X,
                MovementData.Acceleration * (float)delta);
        velocity.X = inputAxis.X * MovementData.Speed;
    }

    private void HandleAirAcceleration(ref Vector2 velocity, Vector2 inputAxis, double delta)
    {
        if (IsOnFloor())
            return;
        velocity.X = Mathf.MoveToward(velocity.X,
                MovementData.Speed * inputAxis.X,
                MovementData.AirAcceleration * (float)delta);
        velocity.X = inputAxis.X * MovementData.Speed;
    }

    private void ApplyFriction(ref Vector2 velocity, double delta)
    {
        velocity.X = Mathf.MoveToward(Velocity.X, 0, MovementData.Friction * (float)delta);
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

        if (!IsOnFloor())
        {
            playerSprite.Play("jump");
        }
    }
}
