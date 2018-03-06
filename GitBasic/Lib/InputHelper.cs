using System;
using System.Windows.Input;

namespace GitBasic
{
    public static class InputHelper
    {
        public static void SendRightClick(object sender)
        {
            var rightClickEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Right)
            {
                RoutedEvent = Mouse.MouseUpEvent,
                Source = sender
            };
            InputManager.Current.ProcessInput(rightClickEvent);
        }
    }
}
