#if GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Griffin.PoseidonPreview
{
    public static class WaterUtilities
    {
        private static List<MeshRenderer> tempRendererContainer = new List<MeshRenderer>();
        public static Bounds GetSuperBoundsWS(this PoseidonWaterBodyPreview water)
        {
            water.GetRenderers(tempRendererContainer);
            Bounds b = new Bounds();
            if (tempRendererContainer.Count > 0)
            {
                b = tempRendererContainer[0].bounds;
                foreach (MeshRenderer mr in tempRendererContainer)
                {
                    b.Encapsulate(mr.bounds);
                }
            }

            return b;
        }

        public static Bounds GetAnchorBoundsWS(this PoseidonAreaWaterPreview water)
        {
            Bounds b = new Bounds();
            if (water.anchors.Count > 0)
            {
                b = new Bounds(water.transform.TransformPoint(water.anchors[0]), Vector3.zero);
                foreach (Vector3 a in water.anchors)
                {

                    b.Encapsulate(water.transform.TransformPoint(a));
                }
            }

            return b;
        }
    }
}
#endif
