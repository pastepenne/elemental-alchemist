using System;
using System.Collections.Generic;
using ElementalAlchemist.Element;
using ElementalAlchemist.Save;
using ElementalAlchemist.Shared;
using UnityEngine;

namespace ElementalAlchemist.Progression
{
    /// <summary>Singleton owning game progression. Persists across scenes.</summary>
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance { get; private set; }

        public static event Action<ElementData> CoreUnlocked;

        [SerializeField] private ProgressionStage _initialStage = ProgressionStage.Tutorial;
        [SerializeField] private bool _startInFreeplay;

        [SerializeField] private ElementData _water;
        [SerializeField] private ElementData _air;
        [SerializeField] private ElementData _earth;
        [SerializeField] private ElementData _fire;

        private ProgressionStage _currentStage;

        public ProgressionStage CurrentStage => _currentStage;
        public bool IsFreeplayActive { get; private set; }
        public bool HasBreathFragment { get; private set; }
        public bool HasFleshFragment { get; private set; }
        public bool HasSoulFragment { get; private set; }
        public bool HasWater { get; private set; }
        public bool HasAir { get; private set; }
        public bool HasEarth { get; private set; }
        public bool HasFire { get; private set; }
        public bool HasAllCores => HasWater && HasAir && HasEarth && HasFire;

        public IEnumerable<ElementData> UnlockedCoreElements
        {
            get
            {
                if (HasWater) yield return _water;
                if (HasAir) yield return _air;
                if (HasEarth) yield return _earth;
                if (HasFire) yield return _fire;
            }
        }

        public ElementTier CurrentAllowedFusionTier => _currentStage switch
        {
            ProgressionStage.Tutorial => ElementTier.Natural,
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

        public void OnWaterUnlocked()
        {
            if (HasWater) { return; }
            HasWater = true;
            CoreUnlocked?.Invoke(_water);
        }

        public void OnAirUnlocked()
        {
            if (HasAir) { return; }
            HasAir = true;
            CoreUnlocked?.Invoke(_air);
        }

        public void OnEarthUnlocked()
        {
            if (HasEarth) { return; }
            HasEarth = true;
            CoreUnlocked?.Invoke(_earth);
        }

        public void OnFireUnlocked()
        {
            if (HasFire) { return; }
            HasFire = true;
            CoreUnlocked?.Invoke(_fire);
        }

        public void OnFreeplayActivated()
        {
            IsFreeplayActive = true;
        }

        public ProgressionData CaptureState() => new()
        {
            stage = (int)_currentStage,
            hasBreath = HasBreathFragment,
            hasFlesh = HasFleshFragment,
            hasSoul = HasSoulFragment,
            hasWater = HasWater,
            hasAir = HasAir,
            hasEarth = HasEarth,
            hasFire = HasFire,
            isFreeplay = IsFreeplayActive
        };

        public void RestoreState(ProgressionData data)
        {
            if (data == null)
            {
                return;
            }

            _currentStage = (ProgressionStage)data.stage;
            HasBreathFragment = data.hasBreath;
            HasFleshFragment = data.hasFlesh;
            HasSoulFragment = data.hasSoul;
            HasWater = data.hasWater;
            HasAir = data.hasAir;
            HasEarth = data.hasEarth;
            HasFire = data.hasFire;
            IsFreeplayActive = data.isFreeplay;
        }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                _currentStage = _initialStage;
                ApplyInitialStageUnlocks();

                if (_startInFreeplay)
                {
                    OnFreeplayActivated();
                    Debug.LogWarning("[ProgressionManager] Freeplay forced on at startup via _startInFreeplay (testing flag).", this);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>Grants everything the starting stage implies, so the game can boot mid-progression without
        /// replaying earlier stages (Tutorial grants nothing).</summary>
        private void ApplyInitialStageUnlocks()
        {
            if (_currentStage >= ProgressionStage.Novice)
            {
                OnWaterUnlocked();
                OnAirUnlocked();
                OnEarthUnlocked();
                OnFireUnlocked();
            }

            if (_currentStage >= ProgressionStage.Apprentice)
            {
                HasBreathFragment = true;
            }

            if (_currentStage >= ProgressionStage.Adept)
            {
                HasFleshFragment = true;
            }

            if (_currentStage >= ProgressionStage.Master)
            {
                HasSoulFragment = true;
            }
        }

        [ContextMenu("Debug/Dump State")]
        private void DumpState()
        {
            Debug.Log(
                $"[ProgressionManager] Stage={_currentStage} " +
                $"AllowedTier={CurrentAllowedFusionTier} " +
                $"Cores=[Water:{HasWater} Air:{HasAir} Earth:{HasEarth} Fire:{HasFire}] " +
                $"Fragments=[Breath:{HasBreathFragment} Flesh:{HasFleshFragment} Soul:{HasSoulFragment}] " +
                $"Freeplay={IsFreeplayActive}");
        }

        [ContextMenu("Debug/Unlock All Cores")]
        private void DebugUnlockAllCores()
        {
            OnWaterUnlocked();
            OnAirUnlocked();
            OnEarthUnlocked();
            OnFireUnlocked();
        }
    }
}
