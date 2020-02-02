using UnityEngine;
using TMPro;

public class HUDIconHandler : MonoBehaviour
{
    private Transform HUDIconParent;
    [SerializeField] private bool dependsOnDistance;
    [SerializeField] private int showTextDistance;
    private Transform playerTransform;
    private TextMeshProUGUI hudText;

    // Start is called before the first frame update
    void Awake()
    {
        HUDIconParent = GameObject.FindGameObjectWithTag("IconParent").transform;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        hudText = Instantiate(new GameObject(), HUDIconParent).AddComponent<TextMeshProUGUI>();
        hudText.faceColor = new Color32(255, 255, 255, 255);
        hudText.alignment = TextAlignmentOptions.Center;
        hudText.rectTransform.sizeDelta = new Vector2(500f, 40f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) > showTextDistance)
            HideText();
        else
            ShowText();

        hudText.rectTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.3f, 0));
    }


    public void UpdateText(string text)
    {
        hudText.text = text;
    }

    private void HideText()
    {
        hudText.gameObject.SetActive(false);
    }

    private void ShowText()
    {
        hudText.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        Destroy(hudText.gameObject);
    }
}