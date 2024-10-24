using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    [SerializeField] private SpriteLibrary library;
    [SerializeField] private SpriteResolver resolver;
    [SerializeField] private Image image;

    [SerializeField] private float swapInterval = 0.2f;

    private Coroutine swapAnimCoroutine;
    private string currentCategory = string.Empty;

    private void Awake()
    {
        if (library == null)
            library = GetComponentInChildren<SpriteLibrary>();
        
        if (resolver == null)
            resolver = GetComponentInChildren<SpriteResolver>();
    }

    private void OnDisable()
    {
        currentCategory = string.Empty;
        if (swapAnimCoroutine != null)
        {
            StopCoroutine(swapAnimCoroutine);
        }
    }

    public IEnumerable<string> GetLabels(string category)
    {
        return library.spriteLibraryAsset.GetCategoryLabelNames(category);
    }

    public float GetDuration(string categoryName)
    {
        return GetLabels(categoryName).Count() * swapInterval;
    }
    
    public void Play(string categoryName, bool playLoop = false, float speedMultiplier = 1, Action<int> onPlayFrame = null, Action onAnimationEnd = null)
    {
        if(currentCategory.Equals(categoryName)) return;

        currentCategory = categoryName;
        
        if(swapAnimCoroutine != null)
            StopCoroutine(swapAnimCoroutine);

        var labels = GetLabels(categoryName).ToList();
        if (labels.Count == 0)
            throw new ArgumentException($"category {categoryName} not found", nameof(categoryName));
        
        int currentIndex = 0;

        swapAnimCoroutine = StartCoroutine(swapRoutine());
        
        IEnumerator swapRoutine()
        {
            while (categoryName.Equals(currentCategory) && (playLoop || currentIndex < labels.Count))
            {
                currentIndex %= labels.Count;

                var label = labels[currentIndex];
                resolver.SetCategoryAndLabel(categoryName, label);

                if (image != null)
                {
                    image.sprite = resolver.spriteLibrary.GetSprite(categoryName, label);
                }
                
                onPlayFrame?.Invoke(currentIndex);

                yield return new WaitForSeconds(swapInterval / speedMultiplier);

                if (!playLoop && currentIndex == labels.Count - 1)
                {
                    onAnimationEnd?.Invoke();
                    yield break;
                }
                
                currentIndex++;
            }
        }
    }
}
