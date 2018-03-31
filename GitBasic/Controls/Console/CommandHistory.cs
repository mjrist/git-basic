using System.Collections.Generic;

namespace GitBasic.Controls
{
    /// <summary>
    /// Keeps a history of the last 100 commands entered.
    /// </summary>
    public class CommandHistory
    {
        public CommandHistory()
        {
            _commands = new LinkedList<string>(new[] { string.Empty });
            ResetCurrentCommand();
        }

        public void AddCommand(string command)
        {
            if (!string.IsNullOrEmpty(command) && (_commands.First.Next == null || command != _commands.First.Next.Value))
            {                
                _commands.AddAfter(_commands.First, command);
                TrimToLast100();
            }            
            ResetCurrentCommand();
        }

        public string GetOlderCommand()
        {
            if (_current.Next != null)
            {
                _current = _current.Next;
            }
            return _current.Value;
        }

        public string GetNewerCommand()
        {
            if (_current.Previous != null)
            {
                _current = _current.Previous;
            }
            return _current.Value;            
        }

        private void TrimToLast100()
        {
            // Compare against 101 - the first command is just the empty string.
            while (_commands.Count > 101)
            {
                _commands.RemoveLast();
            }
        }

        private void ResetCurrentCommand()
        {
            _current = _commands.First;
        }

        private LinkedListNode<string> _current;
        private LinkedList<string> _commands;
    }
}
