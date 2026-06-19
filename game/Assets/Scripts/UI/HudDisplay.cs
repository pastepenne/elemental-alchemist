using ElementalAlchemist.GameInput;
using ElementalAlchemist.Progression;
using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI
{
    /// <summary>Canvas-level HUD controller: shows the current objective and collected realm fragments, and hides
    /// the whole HUD whenever the player is not in control (Grand Master cutscene / dialogue).</summary>
    public class HudDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _objectivePanel;
        [SerializeField] private TMP_Text _objectiveText;
        [SerializeField] private GameObject _fragmentsPanel;
        [SerializeField] private GameObject _breathIcon;
        [SerializeField] private GameObject _fleshIcon;
        [SerializeField] private GameObject _soulIcon;

        private bool _hasObjective;

        private void Awake()
        {
            _objectivePanel.SetActive(false);
        }

        private void OnEnable()
        {
            StoryDirector.StepStarted += OnStepStarted;
            StoryDirector.StepCompleted += OnStepCompleted;
            StoryDirector.SequenceCompleted += OnSequenceCompleted;
            ActionMapController.Changed += OnActionMapChanged;
        }

        private void OnDisable()
        {
            StoryDirector.StepStarted -= OnStepStarted;
            StoryDirector.StepCompleted -= OnStepCompleted;
            StoryDirector.SequenceCompleted -= OnSequenceCompleted;
            ActionMapController.Changed -= OnActionMapChanged;
        }

        // Start, not OnEnable: a loaded save applies progression in SaveManager's sceneLoaded callback,
        // which runs after OnEnable but before Start, so Start sees the restored fragments.
        private void Start()
        {
            RefreshFragments();
            ApplyVisibility();
        }

        private void OnStepStarted(StoryStep step)
        {
            _hasObjective = !string.IsNullOrEmpty(step.ObjectiveText);
            if (_hasObjective)
            {
                _objectiveText.text = step.ObjectiveText;
            }

            ApplyVisibility();
        }

        private void OnStepCompleted(StoryStep step)
        {
            _hasObjective = false;
            ApplyVisibility();
        }

        private void OnSequenceCompleted(string sequenceId)
        {
            _hasObjective = false;
            RefreshFragments();
            ApplyVisibility();
        }

        private void OnActionMapChanged(string actionMap)
        {
            ApplyVisibility();
        }

        private void ApplyVisibility()
        {
            // The HUD is for the player at large; hide it whenever they have no control.
            var playerInControl = ActionMapController.Current == ActionMaps.Player;
            SetActive(_fragmentsPanel, playerInControl);
            SetActive(_objectivePanel, playerInControl && _hasObjective);
        }

        private void RefreshFragments()
        {
            var progression = ProgressionManager.Instance;
            if (!progression)
            {
                return;
            }

            SetActive(_breathIcon, progression.HasBreathFragment);
            SetActive(_fleshIcon, progression.HasFleshFragment);
            SetActive(_soulIcon, progression.HasSoulFragment);
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
