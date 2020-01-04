namespace Assets.Scripts
{
    public class DebugModeText : DebugMessages
    {
        public new readonly DebugMessageType Type = DebugMessageType.DebugMode;

        public DebugModeText() : base("Debug Mode")
        {
        }
    }
}
