using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Damath.UI
{
    public class ChoiceWindow : Window
    {
        List<Choice> choices = new();

        [SerializeField] GameObject choicePrefab;
        [SerializeField] GameObject dividerPrefab;
        
        public Choice AddChoice(string label, Sprite icon = null)
        {
            var go = Instantiate(choicePrefab, transform);

            Choice c = go.GetComponent<Choice>();
            c.Label = label;
            c.Icon = icon; 
            choices.Add(c);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            return c;
        }

        public void AddDivider()
        {
            Instantiate(dividerPrefab, transform);
        }
        
        public void Clear()
        {
            if (choices.Count > 0)
            {
                foreach (Transform c in transform)
                {
                    Destroy(c.gameObject);
                }
            }
            choices.Clear();
        }
    }
}