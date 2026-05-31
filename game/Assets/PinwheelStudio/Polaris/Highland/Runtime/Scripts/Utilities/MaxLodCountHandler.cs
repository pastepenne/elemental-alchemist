#if GRIFFIN && !GRIFFIN_EXCLUDE_HIGHLAND
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Griffin
{
    public static class MaxLodCountHandler
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Init()
        {
            GCommon.getMaxLodCountCallback += OnGetMaxLodCount;
        }

        private static void OnGetMaxLodCount(ref int lodCount)
        {
            lodCount = Mathf.Max(lodCount, 4);
        }
    }
}
#endif