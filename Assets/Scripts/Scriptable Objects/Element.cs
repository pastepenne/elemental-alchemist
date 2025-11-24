using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "Scriptable Objects/Element")]
public class Element : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite sprite;
}
