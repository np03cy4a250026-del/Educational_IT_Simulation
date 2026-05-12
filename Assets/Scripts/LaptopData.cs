using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LaptopData : MonoBehaviour
{
    [Header("Images")]
    public Texture2D[] deviceImages;

    [Header("This Device's Wall Panel")]
    public GameObject wallInfoPanel;
    public Image deviceImage;
    public TextMeshProUGUI counterText;
    public Button prevButton;
    public Button nextButton;
}