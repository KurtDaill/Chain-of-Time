using Godot;
using System;

public partial class TimeUI : Control
{
	[Export]
	Label timeOfDay, timeUnits;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(this.GetNode<GameMaster>("/root/GameMaster").GetCurrentTU()){
			case 3:
				timeOfDay.Text = "Morning";
				timeUnits.Text = "3 Time Units Remaining";
				break;
			case 2:
				timeOfDay.Text = "Noon";
				timeUnits.Text = "2 Time Units Remaining";
				break;
			case 1:
				timeOfDay.Text = "Evening";
				timeUnits.Text = "1 Time Unit Remains";
				break;
			case 0:
				timeOfDay.Text = "Sunset";
				timeUnits.Text = "Night Begins Soon. . .";
				break;
		}
	}
}
