using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GitBasic
{
    public static class ControlExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            Paragraph lastBlock = box.Document.Blocks.LastBlock as Paragraph;

            if (lastBlock == null)
            {
                lastBlock = new Paragraph();
                box.Document.Blocks.Add(lastBlock);
            }

            lastBlock.Inlines.Add(new Run(text)
            {
                Foreground = new SolidColorBrush(color)
            });
        }
    }
}
