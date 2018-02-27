using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace GitBasic
{
    public class HotKeyHelper : IDisposable
    {
        public HotKeyHelper(UIElement element)
        {
            _element = element;
            _element.PreviewKeyDown += _element_PreviewKeyDown;
        }

        private void _element_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            foreach (var hotKey in _hotKeys)
            {
                if (e.Key == hotKey.Key && Keyboard.Modifiers == hotKey.ModifierKeys)
                {
                    hotKey.Action();
                    e.Handled = true;
                }
            }
        }

        public void RegisterHotKey(HotKey hotKey)
        {
            _hotKeys.Add(hotKey);
        }

        public void Dispose()
        {
            _element.PreviewKeyDown -= _element_PreviewKeyDown;
        }

        private List<HotKey> _hotKeys = new List<HotKey>();
        private UIElement _element;
    }
}
