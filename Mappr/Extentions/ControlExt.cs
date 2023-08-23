using System.ComponentModel;

namespace Mappr.Extentions
{
    public static class ControlExt
    {
        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj.InvokeRequired)
            {
                var args = new object[0];
                obj.Invoke(action, args);
            }
            else
            {
                action();
            }
        }

        public static T InvokeIfRequired<T>(this ISynchronizeInvoke obj, Func<T> action)
        {
            if (obj.InvokeRequired)
            {
                var args = new object[0];
                return (T)obj.Invoke(action, args);
            }
            else
            {
                return action();
            }
        }

    }

}

