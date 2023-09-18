using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Damath
{
    public class UIHandler : MonoBehaviour
    {
        public static UIHandler Main;

        public Canvas Canvas { get; private set; }
        public Tooltip Tooltip;
        public List<Sprite> icons;
        public Dictionary<string, Sprite> Icons = new();
        private bool IsDimmed;
        
        [Header("Prefabs")]
        public Image dim;
        public GameObject windowPrefab;
        public GameObject choiceWindowPrefab;
        public GameObject choicePrefab;
        public GameObject tooltipPrefab;

        void Awake()
        {
            Main = this;

            Canvas = GetComponentInChildren<Canvas>();
        }

        void Start()
        {
            //
        }

        public void PlayTransition()
        {
            //
        }

        public void AddIcon(string name, Sprite sprite)
        {
            Icons.Add(name, sprite);
        }

        public Window CreateChoiceWindow()
        {
            var newChoiceWindow = Instantiate(choiceWindowPrefab);
            newChoiceWindow.transform.SetParent(Canvas.transform);
            newChoiceWindow.SetActive(false);
            return newChoiceWindow.GetComponent<Window>();
        }

        /// <summary>
        /// Creates an empty window.
        /// </summary>
        public Window CreateWindow()
        {
            var newWindow = Instantiate(windowPrefab, new Vector3(0f, 0f, Constants.ZLocationWindow), Quaternion.identity, Canvas.transform);
            newWindow.SetActive(false);
            return newWindow.GetComponent<Window>();
        }
        
        /// <summary>
        /// Creates a window given a window prefab.
        /// </summary>
        public Window CreateWindow(GameObject prefab)
        {
            var newWindow = Instantiate(prefab, new Vector3(0f, 0f, Constants.ZLocationWindow), Quaternion.identity, Canvas.transform);
            return newWindow.GetComponent<Window>();
        }

        public Tooltip CreateTooltip(string text)
        {
            Tooltip = Instantiate(tooltipPrefab, Canvas.transform).GetComponent<Tooltip>();
            Tooltip.SetText(text);
            Tooltip.Show();

            return Tooltip;
        }

        public Tooltip CreateTooltip(string text, Color color)
        {
            Tooltip = Instantiate(tooltipPrefab, Canvas.transform).GetComponent<Tooltip>();
            Tooltip.SetText(text);
            Tooltip.SetColor(color);
            Tooltip.Show();

            return Tooltip;
        }

        public void HideTooltip()
        {
            Tooltip.Hide();
        }

        public void Dim(float time)
        {
            if (IsDimmed)
            {
                LeanTween.value(dim.gameObject, dim.color.a, 0f, time)
                .setEaseOutExpo()
                .setOnUpdate( (i) =>
                {
                    dim.color = new (dim.color.r, dim.color.g, dim.color.b, i);
                });
            } else
            {
                LeanTween.value(dim.gameObject, 0f, 0.25f, time)
                .setEaseOutExpo()
                .setOnUpdate( (i) =>
                {
                    dim.color = new (dim.color.r, dim.color.g, dim.color.b, i);
                });
            }
            IsDimmed = !IsDimmed;
        }
    }
}
