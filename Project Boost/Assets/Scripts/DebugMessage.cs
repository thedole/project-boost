using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DebugMessage
    {
        public Text TextField{ get; set; }
        public DebugMessageType Type { get; }
        public string Text { get; }

        public DebugMessage(DebugMessageType type, Text field, string text)
        {
            Type = type;
            TextField = field;
            Text = text;
        }

        public void ClearMessage()
        {
            TextField.text = string.Empty;
        }

        public void SetMessage()
        {
            TextField.text = Text;
        }
    }
}
