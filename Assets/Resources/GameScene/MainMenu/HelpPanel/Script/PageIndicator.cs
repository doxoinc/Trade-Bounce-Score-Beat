using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PageIndicator : MonoBehaviour
{
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public List<Image> indicators = new List<Image>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                indicators.Add(img);
            }
        }
        UpdateIndicators(0);
    }

    /// <summary>
    /// Обновляет состояние индикаторов.
    /// </summary>
    /// <param name="activeIndex">Активный индекс.</param>
    public void UpdateIndicators(int activeIndex)
    {
        for (int i = 0; i < indicators.Count; i++)
        {
            if (i == activeIndex)
            {
                indicators[i].sprite = activeSprite;
            }
            else
            {
                indicators[i].sprite = inactiveSprite;
            }
        }
    }
}
