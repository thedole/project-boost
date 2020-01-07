namespace Assets.Scripts
{
    public class DebugModeText : DebugMessage
    {
        public new readonly DebugMessageType Type = DebugMessageType.DebugMode;

        public DebugModeText() : base("Debug Mode")
        {
        }
    }
}
