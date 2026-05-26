using System.Collections.Generic;
using System.Linq;
using ElementalAlchemist.Element;
using UnityEngine;

namespace ElementalAlchemist.Progression
{
    /// <summary>Singleton owning game progression. Persists across scenes.</summary>
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance { get; private set; }

        [SerializeField] private ProgressionStage _initialStage = ProgressionStage.Tutorial;
        [SerializeField] private ElementData[] _initialUnlockedCoreElements;
        
        private readonly HashSet<ElementData> _unlockedCoreElements = new();
        private ProgressionStage _currentStage;
        
        public bool IsFreeplayActive { get; private set; }
        public bool HasBreathFragment { get; private set; }
        public bool HasFleshFragment { get; private set; }
        public bool HasSoulFragment { get; private set; }
        public IReadOnlyCollection<ElementData> UnlockedCoreElements => _unlockedCoreElements;
        public ElementTier CurrentAllowedFusionTier => _currentStage switch
        {
            ProgressionStage.Tutorial => ElementTier.Core,
            ProgressionStage.Novice => ElementTier.Natural,
            ProgressionStage.Apprentice => ElementTier.Refined,
            ProgressionStage.Adept => ElementTier.Advanced,
            ProgressionStage.Master => ElementTier.Exotic,
            _ => ElementTier.Core
        };

        public void OnTutorialCompleted()
        {
            _currentStage = ProgressionStage.Novice;
        }
        
        public void OnBreathFragmentGranted()
        {
            if (HasBreathFragment)
            {
                return;
            }

            HasBreathFragment = true;
            _currentStage = ProgressionStage.Apprentice;
        }

        public void OnFleshFragmentGranted()
        {
            if (HasFleshFragment)
            {
                return;
            }

            HasFleshFragment = true;
            _currentStage = ProgressionStage.Adept;
        }

        public void OnSoulFragmentGranted()
        {
            if (HasSoulFragment)
            {
                return;
            }

            HasSoulFragment = true;
            _currentStage = ProgressionStage.Master;
        }
        
        private void Awake()
        {
            // Ensure only one instance of ProgressionManager exists
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                _currentStage = _initialStage;
                foreach (var element in _initialUnlockedCoreElements)
                {
                    _unlockedCoreElements.Add(element);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [ContextMenu("Debug/Dump State")]
        private void DumpState()
        {
            Debug.Log(
                $"[GameStateManager] Stage={_currentStage} " +
                $"AllowedTier={CurrentAllowedFusionTier} " +
                $"Cores=[{string.Join(" ", _unlockedCoreElements.Select(e => e.Id))}] " +
                $"Fragments=[Breath:{HasBreathFragment} Flesh:{HasFleshFragment} Soul:{HasSoulFragment}] " +
                $"Freeplay={IsFreeplayActive}");
        }
    }
}
