using System;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.World
{
    /// <summary>The Sanctuary altar. Becomes usable once the Soul fragment is claimed and goes dark for good once the
    /// player turns away from the Life fusion - its collider and particles track that window.</summary>
    [RequireComponent(typeof(Collider))]
    public class SanctuaryAltar : MonoBehaviour, IInteractable
    {
        public static event Action Interacted;

        [SerializeField] private GameObject _particles;
        private Collider _collider;

        public string Prompt => "Fuse";

        public void Interact()
        {
            if (IsActive)
            {
                Interacted?.Invoke();
            }
        }

        private static bool IsActive
        {
            get
            {
                var progression = ProgressionManager.Instance;
                return progression && progression.HasSoulFragment && !progression.IsFreeplayActive;
            }
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            StoryDirector.SequenceCompleted += OnSequenceCompleted;
        }

        private void OnDisable()
        {
            StoryDirector.SequenceCompleted -= OnSequenceCompleted;
        }

        // The two moments that flip the window both end a sequence: "ruins" grants the Soul fragment (altar lights
        // up), "ruins-epilogue" finishes the refusal (altar goes dark). Start covers a fresh scene/save load.
        private void Start()
        {
            Refresh();
        }

        private void OnSequenceCompleted(string sequenceId)
        {
            Refresh();
        }

        private void Refresh()
        {
            var active = IsActive;
            _collider.enabled = active;

            if (_particles)
            {
                _particles.SetActive(active);
            }
        }
    }
}
