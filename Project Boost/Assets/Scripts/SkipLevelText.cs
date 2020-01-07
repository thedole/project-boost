using Assets.Scripts;

internal class SkipLevelText : DebugMessage
{
    public new readonly DebugMessageType Type = DebugMessageType.SkipLevel;

    public SkipLevelText() : base("Skip Level")
    {
    }
}