using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	
	private int _score;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	
	private void NewGame() {
		GetNode<AudioStreamPlayer>("Music").Play();
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		_score = 0;
		Hud hud = GetNode<Hud>("HUD");
		hud.UpdateScore(_score);
			
		Player player = GetNode<Player>("Player");
		Marker2D startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);
		player.Show();
		
		hud.ShowMessage("Get Ready!");
		GetNode<Timer>("StartTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
	
	private void GameOver() {
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<Hud>("HUD").ShowGameOver();
	}
	
	private void OnScoreTimerTimeout() {
		_score++;
		GetNode<Hud>("HUD").UpdateScore(_score);
	}
	
	private void OnStartTimerTimeout() {
		GetNode<Timer>("MobTimer").Start();
	}
	
	private void OnMobTimerTimeout() {
		Mob mob = MobScene.Instantiate<Mob>();
		
		PathFollow2D mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();
		
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;
		
		mob.Position = mobSpawnLocation.Position;
		
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;
		
		Vector2 velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);
		
		AddChild(mob);
	}
}
