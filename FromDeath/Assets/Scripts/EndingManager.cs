using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float textScrollSpeed = 25f;
    [SerializeField] private string menuScene = "MainMenu";

    [Header("Referencias")]
    [SerializeField] private Text endingText;
    [SerializeField] private Button returnButton;

    void Start()
    {
        returnButton.onClick.AddListener(ReturnToMenu);
        StartCoroutine(ScrollText());
    }

    IEnumerator ScrollText()
    {
        RectTransform textTransform = endingText.GetComponent<RectTransform>();
        float startPosition = textTransform.anchoredPosition.y;
        
        while(textTransform.anchoredPosition.y < startPosition + 1000)
        {
            textTransform.anchoredPosition += Vector2.up * textScrollSpeed * Time.deltaTime;
            yield return null;
        }
        
        returnButton.gameObject.SetActive(true);
    }

    void ReturnToMenu() => SceneManager.LoadScene(menuScene);
}