using System;
using System.IO;
using System.Threading;

namespace GitBasic
{
    public class IOHelper
    {
        public static bool TryRepeatIOAction<T>(Func<T> ioAction, out T returnValue)
        {
            for (int tries = 10; tries > 0; tries--)
            {
                try
                {
                    returnValue = ioAction();
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }                
            }
            returnValue = default(T);
            return false;
        }
    }
}
