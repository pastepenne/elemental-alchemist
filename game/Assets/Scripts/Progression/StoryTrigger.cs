using System;

namespace ElementalAlchemist.Progression
{
    public static class StoryTrigger
    {
        public static event Action<string> Fired;

        public static void Fire(string id)
        {
            Fired?.Invoke(id);
        }
    }
}
