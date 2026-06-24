using ElementalAlchemist.Element;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>Fills a fixed set of Image slots with an element's tag sprites, activating only the ones used.
    /// Shared by the pouch/tome/fusion icon panels so the resolve-and-assign logic lives in one place.</summary>
    public static class ElementIcons
    {
        public static void Apply(Image[] icons, ElementData element, TagSpriteLibrary tagSprites)
        {
            var sprites = tagSprites.GetSprites(element.Tags, icons.Length);
            for (int i = 0; i < icons.Length; i++)
            {
                bool has = i < sprites.Count;
                icons[i].gameObject.SetActive(has);
                if (has)
                {
                    icons[i].sprite = sprites[i];
                }
            }
        }

        public static void Clear(Image[] icons)
        {
            foreach (var icon in icons)
            {
                icon.gameObject.SetActive(false);
            }
        }
    }
}
