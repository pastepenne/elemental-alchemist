using ElementalAlchemist.Dialogue;

namespace ElementalAlchemist.Progression
{
    public static class StoryGate
    {
        public static bool TryProceed(string triggerId, DialogueData gateDialogue)
        {
            var director = StoryDirector.Instance;
            if (!director || director.IsCurrentTrigger(triggerId))
            {
                return true;
            }
            
            if (gateDialogue)
            {
                DialogueManager.Instance.StartDialogue(gateDialogue);
            }
            
            return false;

        }
    }
}
