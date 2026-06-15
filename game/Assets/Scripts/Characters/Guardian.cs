using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Element;
using UnityEngine;

namespace ElementalAlchemist.Characters
{
    public class Guardian : MonoBehaviour
    {
        [SerializeField] private string _guardianName;
        [SerializeField] private DialogueData _riddleDialogue;
        [SerializeField] private DialogueData _completedDialogue;
        [SerializeField] private DialogueData _idleDialogue;
        [SerializeField] private ElementData _requiredElement;
    }
}
