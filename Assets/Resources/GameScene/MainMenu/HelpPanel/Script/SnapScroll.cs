using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SnapScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public List<RectTransform> slides = new List<RectTransform>();
    public float snapSpeed = 10f;
    public PageIndicator pageIndicator; // Ссылка на PageIndicator

    private bool isSnapping = false;
    private int currentIndex = 0;

    void Start()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        foreach (RectTransform child in content)
        {
            slides.Add(child);
        }

        if (pageIndicator != null)
        {
            pageIndicator.UpdateIndicators(currentIndex);
        }
    }

    void Update()
    {
        if (!isSnapping && Mathf.Abs(scrollRect.velocity.y) < 10f)
        {
            StartCoroutine(SnapToClosest());
        }
    }

    IEnumerator SnapToClosest()
    {
        isSnapping = true;

        float closestDistance = Mathf.Infinity;
        int closestIndex = currentIndex;

        for (int i = 0; i < slides.Count; i++)
        {
            float distance = Mathf.Abs(slides[i].anchoredPosition.y - content.anchoredPosition.y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        currentIndex = closestIndex;

        if (pageIndicator != null)
        {
            pageIndicator.UpdateIndicators(currentIndex);
        }

        Vector2 targetPosition = new Vector2(content.anchoredPosition.x, slides[currentIndex].anchoredPosition.y);
        while (Vector2.Distance(content.anchoredPosition, targetPosition) > 0.1f)
        {
            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPosition, snapSpeed * Time.deltaTime);
            yield return null;
        }

        content.anchoredPosition = targetPosition;
        isSnapping = false;
    }

    /// <summary>
    /// Метод для программного перехода к слайду по индексу.
    /// </summary>
    /// <param name="index">Индекс слайда.</param>
    public void GoToSlide(int index)
    {
        if (index < 0 || index >= slides.Count)
        {
            Debug.LogWarning("Invalid slide index");
            return;
        }

        StopAllCoroutines();
        currentIndex = index;
        StartCoroutine(SnapToClosest());
    }
}
