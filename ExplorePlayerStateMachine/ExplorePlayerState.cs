using Godot;
using System;

public abstract class ExplorePlayerState
{
    public abstract void HandleInput(ExplorePlayer self);
    public abstract void Process(ExplorePlayer self);
}
