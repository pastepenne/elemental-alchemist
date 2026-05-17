#if GRIFFIN
#if !JUPITER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Griffin.JupiterPreview
{
    public static class JEditorMenus
    {
        //[MenuItem("GameObject/3D Object/Polaris/Sky (Jupiter Preview)/Day Sky")]
        public static JupiterSkyPreview CreateDaySky(MenuCommand cmd)
        {
            GameObject g = new GameObject("Day Sky - Jupiter Preview");
            if (cmd != null && cmd.context != null)
            {
                GameObject root = cmd.context as GameObject;
                GameObjectUtility.SetParentAndAlign(g, root);
            }

            JupiterSkyPreview skyComponent = g.AddComponent<JupiterSkyPreview>();
            skyComponent.ApplyDayPreset();

            GNetUtils.TrackClick("add_day_sky", GUILocation.Menus);

            return skyComponent;
        }

        //[MenuItem("GameObject/3D Object/Polaris/Sky (Jupiter Preview)/Night Sky")]
        public static JupiterSkyPreview CreateNighSky(MenuCommand cmd)
        {
            GameObject g = new GameObject("Night Sky - Jupiter Preview");
            if (cmd != null && cmd.context != null)
            {
                GameObject root = cmd.context as GameObject;
                GameObjectUtility.SetParentAndAlign(g, root);
            }

            JupiterSkyPreview skyComponent = g.AddComponent<JupiterSkyPreview>();
            skyComponent.ApplyNightPreset();

            GNetUtils.TrackClick("add_night_sky", GUILocation.Menus);

            return skyComponent;
        }
    }
}
#endif  
#endif