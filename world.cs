using Godot;
using System;

public partial class world : Node2D
{
	// Called when the node enters the scene tree for the first time.
    //
    
    
	public override void _Ready()
	{
        var polygon2d = GetNode<Polygon2D>("StaticBody2D/CollisionPolygon2D/Polygon2D");
        var collisionPoly = GetNode<CollisionPolygon2D>("StaticBody2D/CollisionPolygon2D");
        RenderingServer.SetDefaultClearColor(Colors.Black);
        polygon2d.Polygon = collisionPoly.Polygon;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
