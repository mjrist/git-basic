using System.Windows;
using System.Windows.Media;

namespace GitBasic
{
    public static class UiExtensions
    {
        public static T FindAnchestor<T>(this DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T currentAsT)
                {
                    return currentAsT;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }
}
