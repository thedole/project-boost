namespace Assets.Scripts
{
    public abstract class DebugMessages
    {
        public readonly string Text;
        public readonly DebugMessageType Type;
        protected DebugMessages(string text)
        {
            Text = text;
        }
    }
}
