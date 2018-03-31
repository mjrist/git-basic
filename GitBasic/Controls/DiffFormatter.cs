using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitBasic.Controls
{
    public class DiffFormatter
    {
        public DiffFormatter(RichTextBox richTextBox)
        {
            _textBox = richTextBox;
        }

        public void AddSection(string text, DiffSectionType sectionType = DiffSectionType.Unchanged)
        {
            if (sectionType == DiffSectionType.Added)
            {
                AddNewSection(text, _green);
            }
            else if (sectionType == DiffSectionType.Removed)
            {
                AddNewSection(text, _red);
            }
            else
            {
                AddNewSection(text);
            }
        }

        private void AddNewSection(string text, Brush background = null)
        {
            Paragraph section = new Paragraph(new Run(text));
            RemoveLastNewLine(section);

            if (background != null)
            {
                section.Background = background;
            }

            _textBox.Document.Blocks.Add(section);
        }

        private void RemoveLastNewLine(Paragraph paragraph)
        {
            if (paragraph.Inlines.LastInline is Run lastInline && lastInline.Text.EndsWith("\r\n"))
            {
                lastInline.Text = lastInline.Text.Substring(0, lastInline.Text.Length - 2);
            }
        }

        public void AddPadding(int lineCount)
        {
            string padding = string.Empty;
            for (int i = 0; i < lineCount; i++)
            {
                padding += Environment.NewLine;
            }
            AddNewSection(padding, _paddingBrush);
        }

        public void Clear()
        {
            _textBox.Document.Blocks.Clear();
        }

        private static VisualBrush CreatePaddingBrush()
        {
            Path path = new Path();
            string sData = "M 0 0 l 10 10";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            path.Data = (Geometry)converter.ConvertFrom(sData);
            path.Stroke = new SolidColorBrush(Color.FromRgb(69, 69, 69));

            Canvas canvas = new Canvas();
            canvas.Children.Add(path);

            return new VisualBrush
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 10, 10),
                Viewbox = new Rect(0, 0, 10, 10),
                ViewportUnits = BrushMappingMode.Absolute,
                ViewboxUnits = BrushMappingMode.Absolute,
                Visual = canvas
            };
        }

        private static VisualBrush _paddingBrush = CreatePaddingBrush();
        private static SolidColorBrush _red = new SolidColorBrush(Color.FromArgb(100, 255, 66, 66));
        private static SolidColorBrush _green = new SolidColorBrush(Color.FromArgb(100, 70, 170, 60));
        private RichTextBox _textBox;
    }

    public enum DiffSectionType { Unchanged, Added, Removed }
}
