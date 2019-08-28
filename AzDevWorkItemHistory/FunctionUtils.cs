using System;

namespace WorkItemHistory
{
    public static class FunctionUtils
    {
        public static T2 Then<T1, T2>(this T1 x, Func<T1, T2> f)
        {
            return f(x);
        }
    }
}