#if !UNITY_6000_0_OR_NEWER
using System;
namespace Unity.Pipeline.Editor
{
    // Local diagnostics contract only. Unity 2022 has no IAnalytic/SendAnalytic API.
    internal interface IAnalytic
    {
        public interface IData { }
        bool TryGatherData(out IData data, out Exception error);
    }
}
#endif
