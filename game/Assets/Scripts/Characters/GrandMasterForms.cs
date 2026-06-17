using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.Characters
{
    /// <summary>Owns the GrandMaster's swappable form meshes (skinned to one shared rig) and activates the one
    /// matching current progression.</summary>
    public class GrandMasterForms : MonoBehaviour
    {
        private enum Form
        {
            Base,
            Empowered,
            Ultimate
        }

        [SerializeField] private GameObject _baseForm;
        [SerializeField] private GameObject _empoweredForm;
        [SerializeField] private GameObject _ultimateForm;

        /// <summary>Activate the form matching current progression; deactivate the others.</summary>
        public void ApplyCurrentForm()
        {
            Apply(ResolveForm());
        }

        private Form ResolveForm()
        {
            var progression = ProgressionManager.Instance;
            if (!progression)
            {
                return Form.Base;
            }

            // Ultimate: final riddle solved (Soul fragment granted in the last scene).
            if (progression.HasSoulFragment)
            {
                return Form.Ultimate;
            }

            // Empowered: first major realm cleared. Swap to HasFleshFragment if it should be the underground instead.
            if (progression.HasBreathFragment || progression.HasFleshFragment)
            {
                return Form.Empowered;
            }

            return Form.Base;
        }

        private void Apply(Form form)
        {
            SetActive(_baseForm, form == Form.Base);
            SetActive(_empoweredForm, form == Form.Empowered);
            SetActive(_ultimateForm, form == Form.Ultimate);
        }

        private static void SetActive(GameObject form, bool active)
        {
            if (form && form.activeSelf != active)
            {
                form.SetActive(active);
            }
        }
    }
}
