using Assets.Scripts;

internal class CollisionsOffText : DebugMessage
{
    public new readonly DebugMessageType Type = DebugMessageType.CollisionsOff;

    public CollisionsOffText() : base("Collisions off")
    {
    }
}