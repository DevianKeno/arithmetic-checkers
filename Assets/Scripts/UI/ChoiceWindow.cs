using System.Collections.Generic;
using UnityEngine;

namespace Damath.UI
{
    public class ChoiceWindow : Window
    {
        List<Choice> choices = new();

        [SerializeField] GameObject choicePrefab;
        
        public Choice AddChoice(string label, Sprite icon = null)
        {
            var go = Instantiate(choicePrefab, transform);

            Choice c = go.GetComponent<Choice>();
            c.Label = label;
            c.Icon = icon; 
            c.AddCallback(Hide);
            choices.Add(c);
            return c;
        }
        
        public void ClearChoices()
        {
            if (choices.Count > 0)
            {
                foreach (Choice c in choices)
                {
                    choices.Remove(c);
                    Destroy(c.gameObject);
                }
            }
        }
    }
}