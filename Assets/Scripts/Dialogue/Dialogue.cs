using UnityEngine;

namespace ElementalAlchemist.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Elemental Alchemist/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private string _speaker;
        [SerializeField] [TextArea] private string[] _lines;
        
        public string Speaker => _speaker;
        public string[] Lines => _lines;
    }
}