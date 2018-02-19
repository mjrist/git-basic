using System;
using System.Windows.Input;

namespace GitBasic
{
    public class HotKey
    {
        public HotKey (Key key, ModifierKeys modifierKeys, Action action)
        {
            Key = key;
            ModifierKeys = modifierKeys;
            Action = action;
        }

        public Key Key { get; }
        public ModifierKeys ModifierKeys { get; }
        public Action Action { get; }
    }
}
