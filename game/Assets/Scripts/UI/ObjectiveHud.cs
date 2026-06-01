using ElementalAlchemist.Progression;
using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI
{
    public class ObjectiveHud : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _objectiveText;

        private void Awake()
        {
            _root.SetActive(false);
        }

        private void OnEnable()
        {
            StoryDirector.StepStarted += OnStepStarted;
            StoryDirector.StepCompleted += OnStepCompleted;
            StoryDirector.SequenceCompleted += OnSequenceCompleted;
        }

        private void OnDisable()
        {
            StoryDirector.StepStarted -= OnStepStarted;
            StoryDirector.StepCompleted -= OnStepCompleted;
            StoryDirector.SequenceCompleted -= OnSequenceCompleted;
        }

        private void OnStepStarted(StoryStep step)
        {
            if (string.IsNullOrEmpty(step.ObjectiveText))
            {
                _root.SetActive(false);
                return;
            }

            _objectiveText.text = step.ObjectiveText;
            _root.SetActive(true);
        }

        private void OnStepCompleted(StoryStep step)
        {
            _root.SetActive(false);
        }

        private void OnSequenceCompleted(string sequenceId)
        {
            _root.SetActive(false);
        }
    }
}
