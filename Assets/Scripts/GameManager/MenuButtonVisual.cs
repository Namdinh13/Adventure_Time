using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonVisual : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text buttonText;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            background.color = new Color(1, 1, 1, 0);
            buttonText.color = Color.white;
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        background.color = Color.white;
        buttonText.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = new Color(1, 1, 1, 0);
        buttonText.color = Color.white;
    }
}