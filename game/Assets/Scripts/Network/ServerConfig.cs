using UnityEngine;

namespace ElementalAlchemist.Network
{
    [CreateAssetMenu(fileName = "New Server Config", menuName = "Elemental Alchemist/Server Config")]
    public class ServerConfig : ScriptableObject
    {
        [SerializeField] private string _baseUrl = "http://localhost:5000";

        public string BaseUrl => _baseUrl;
    }
}
