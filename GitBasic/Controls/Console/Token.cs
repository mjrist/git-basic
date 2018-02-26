namespace GitBasic.Controls
{
    public class Token
    {
        public string Text { get; set; } = string.Empty;
        public int StartIndex { get; set; } = 0;

        public void Reset()
        {
            Text = string.Empty;
            StartIndex = 0;
        }
    }
}
