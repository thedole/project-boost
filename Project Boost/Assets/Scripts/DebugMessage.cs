namespace Assets.Scripts
{
    public abstract class DebugMessage
    {
        public readonly string Text;
        public readonly DebugMessageType Type;

        protected DebugMessage(string text)
        {
            Text = text;
        }
    }
}
