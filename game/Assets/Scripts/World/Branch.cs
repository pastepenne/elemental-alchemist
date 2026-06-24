using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.World
{
    public class Branch : ElementSource
    {
        [SerializeField] private int _despawnThreshold = 3;

        private void OnEnable()
        {
            if (PlayerManager.Instance)
            {
                PlayerManager.Instance.Inventory.ElementAdded += OnElementAdded;
            }

            DespawnIfHeld();
        }

        private void OnDisable()
        {
            if (PlayerManager.Instance)
            {
                PlayerManager.Instance.Inventory.ElementAdded -= OnElementAdded;
            }
        }

        private void OnElementAdded(ElementData element)
        {
            if (element == Element)
            {
                DespawnIfHeld();
            }
        }

        private void DespawnIfHeld()
        {
            if (ProgressionManager.Instance.CurrentStage > ProgressionStage.Tutorial || PlayerManager.Instance && PlayerManager.Instance.Inventory.HasElement(Element, _despawnThreshold))
            {
                Destroy(gameObject);
            }
        }
    }
}
