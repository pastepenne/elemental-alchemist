#if GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Griffin.PoseidonPreview
{
    [System.Serializable]
    public struct AreaMeshDesc
    {
        public const int MESH_RES_CAP = 30;

        [SerializeField]
        private int m_resolution;
        public int resolution
        {
            get
            {
                return m_resolution;
            }
            set
            {
                m_resolution = Mathf.Clamp(value, 2, MESH_RES_CAP);
                if (m_resolution % 2 == 1)
                {
                    m_resolution -= 1;
                }
            }
        }
    }
}

#endif