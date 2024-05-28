using Godot;
using System;

public partial class UI : Control
{

	private ProgressBar health_bar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		health_bar = GetNode<ProgressBar>("HealthBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Get_health_value() >= 100){
			health_bar.Visible = false;
		}
	}

	public double Get_health_value(){
		return health_bar.Value;
	}

	public void Set_health_value(double value){
		health_bar.Value = value;
	}
}
