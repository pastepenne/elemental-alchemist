using ElementalAlchemist.GameInput;
using ElementalAlchemist.Progression;
using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI
{
    /// <summary>Canvas-level HUD: mirrors the current objective and collected fragments, and hides everything
    /// while the player has no control (Grand Master cutscene / dialogue).</summary>
    public class HudDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _objectivePanel;
        [SerializeField] private TMP_Text _objectiveText;
        [SerializeField] private GameObject _fragmentsPanel;
        [SerializeField] private GameObject _breathIcon;
        [SerializeField] private GameObject _fleshIcon;
        [SerializeField] private GameObject _soulIcon;

        private void OnEnable()
        {
            StoryDirector.StepStarted += OnStepChanged;
            StoryDirector.StepCompleted += OnStepChanged;
            StoryDirector.SequenceCompleted += OnSequenceChanged;
            ActionMapController.Changed += OnMapChanged;
        }

        private void OnDisable()
        {
            StoryDirector.StepStarted -= OnStepChanged;
            StoryDirector.StepCompleted -= OnStepChanged;
            StoryDirector.SequenceCompleted -= OnSequenceChanged;
            ActionMapController.Changed -= OnMapChanged;
        }

        // Start, not OnEnable: a loaded save applies progression in SaveManager's sceneLoaded callback,
        // which runs after OnEnable but before Start.
        private void Start() => Refresh();

        private void OnStepChanged(StoryStep step) => Refresh();
        private void OnSequenceChanged(string sequenceId) => Refresh();
        private void OnMapChanged(string actionMap) => Refresh();

        /// <summary>Re-renders the HUD from current game state.</summary>
        private void Refresh()
        {
            var inControl = ActionMapController.Current == ActionMaps.Player;

            var step = StoryDirector.Instance ? StoryDirector.Instance.CurrentStep : null;
            var objective = step ? step.ObjectiveText : null;
            var hasObjective = !string.IsNullOrEmpty(objective);
            if (hasObjective)
            {
                _objectiveText.text = objective;
            }

            // TEMP diagnostic — remove once the scene-change objective issue is confirmed fixed.
            Debug.Log($"[HudDisplay] {gameObject.scene.name}: map={ActionMapController.Current}, " +
                      $"step={(step ? step.name : "null")}, objective='{objective}'");

            _objectivePanel.SetActive(inControl && hasObjective);
            _fragmentsPanel.SetActive(inControl);

            var progression = ProgressionManager.Instance;
            if (progression)
            {
                SetActive(_breathIcon, progression.HasBreathFragment);
                SetActive(_fleshIcon, progression.HasFleshFragment);
                SetActive(_soulIcon, progression.HasSoulFragment);
            }
        }

        private static void SetActive(GameObject go, bool active)
        {
            if (go && go.activeSelf != active)
            {
                go.SetActive(active);
            }
        }
    }
}
