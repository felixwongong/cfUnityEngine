using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private SpriteLibrary library;
    [SerializeField] private SpriteResolver resolver;

    [SerializeField] private float swapInterval = 0.2f;

    private string currentCategory = string.Empty;
    private int currentLabelIndex = 0;
    private float timer = 0f;
    private List<string> currentLabels = new List<string>();
    private bool loop = false;

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
        currentLabels.Clear();
        currentLabelIndex = 0;
        timer = 0f;
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(currentCategory) || currentLabels.Count == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= swapInterval)
        {
            timer -= swapInterval;
            currentLabelIndex++;

            if (currentLabelIndex >= currentLabels.Count)
            {
                if (loop)
                {
                    currentLabelIndex = 0;
                }
                else
                {
                    currentLabelIndex = currentLabels.Count - 1;
                    return;
                }
            }

            resolver.SetCategoryAndLabel(currentCategory, currentLabels[currentLabelIndex]);
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
    
    public void Play(string categoryName, bool shouldLoop = true)
    {
        if(currentCategory.Equals(categoryName)) return;

        currentCategory = categoryName;
        currentLabels = GetLabels(categoryName).ToList();
        currentLabelIndex = 0;
        timer = 0f;
        loop = shouldLoop;

        if (currentLabels.Count > 0)
        {
            resolver.SetCategoryAndLabel(currentCategory, currentLabels[0]);
        }
    }
}
