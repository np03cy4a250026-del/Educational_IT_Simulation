using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseRaycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 50f;
    public LayerMask deviceLayer;

    [Header("Cursor")]
    public Texture2D handCursorTexture;

    private Camera cam;
    private GameObject lastHovered;
    private Texture2D[] currentImages;
    private int currentIndex = 0;
    private GameObject currentPanel;
    private Image currentDeviceImage;
    private TextMeshProUGUI currentCounterText;
    private Button currentPrevButton;
    private Button currentNextButton;

    void Start()
    {
        cam = Camera.main;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Hide ALL panels at start
        foreach (LaptopData device in 
            FindObjectsOfType<LaptopData>())
        {
            if (device.wallInfoPanel != null)
                device.wallInfoPanel.SetActive(false);
        }
    }

    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
            return;
        }

        if (currentPanel != null && currentPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(1) ||
                Input.GetKeyDown(KeyCode.E))
                ClosePanel();
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, deviceLayer))
        {
            if (hit.collider.CompareTag("CyberDevice"))
            {
                HandleHover(hit.collider.gameObject);
                if (Input.GetMouseButtonDown(0))
                    HandleClick(hit.collider.gameObject);
            }
            else
            {
                ClearHover();
            }
        }
        else
        {
            ClearHover();
        }
    }

    void HandleHover(GameObject device)
    {
        if (lastHovered != device)
        {
            lastHovered = device;
            Cursor.SetCursor(handCursorTexture,
                Vector2.zero, CursorMode.Auto);
        }
    }

    void ClearHover()
    {
        if (lastHovered != null)
        {
            lastHovered = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    void HandleClick(GameObject device)
    {
        LaptopData data = device.GetComponent<LaptopData>();

        if (data == null)
        {
            Debug.LogWarning("No LaptopData on: " + device.name);
            return;
        }

        if (data.deviceImages == null ||
            data.deviceImages.Length == 0)
        {
            Debug.LogWarning("No images on: " + device.name);
            return;
        }

        // Close previous panel if open
        ClosePanel();

        // Get THIS device's own panel
        currentPanel          = data.wallInfoPanel;
        currentDeviceImage    = data.deviceImage;
        currentCounterText    = data.counterText;
        currentPrevButton     = data.prevButton;
        currentNextButton     = data.nextButton;

        currentImages = data.deviceImages;
        currentIndex  = 0;

        currentPanel.SetActive(true);

        // Hook buttons
        currentPrevButton.onClick.RemoveAllListeners();
        currentNextButton.onClick.RemoveAllListeners();
        currentPrevButton.onClick.AddListener(ShowPrevImage);
        currentNextButton.onClick.AddListener(ShowNextImage);

        DisplayCurrentImage();
        StartCoroutine(AnimatePanel());
    }

    void DisplayCurrentImage()
    {
        if (currentImages == null ||
            currentImages.Length == 0) return;

        Texture2D tex = currentImages[currentIndex];
        if (tex == null) return;

        Sprite newSprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );
        currentDeviceImage.sprite = newSprite;

        currentCounterText.text =
            $"Image {currentIndex + 1} of {currentImages.Length}";

        currentPrevButton.gameObject.SetActive(currentIndex > 0);
        currentNextButton.gameObject.SetActive(
            currentIndex < currentImages.Length - 1);
    }

    public void ShowNextImage()
    {
        if (currentImages == null) return;
        if (currentIndex < currentImages.Length - 1)
        {
            currentIndex++;
            DisplayCurrentImage();
        }
    }

    public void ShowPrevImage()
    {
        if (currentImages == null) return;
        if (currentIndex > 0)
        {
            currentIndex--;
            DisplayCurrentImage();
        }
    }

    void ClosePanel()
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        currentPanel       = null;
        currentImages      = null;
        currentIndex       = 0;
        currentDeviceImage = null;
        currentCounterText = null;
        currentPrevButton  = null;
        currentNextButton  = null;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ClearHover();
    }

    System.Collections.IEnumerator AnimatePanel()
    {
        Vector3 finalScale = new Vector3(0.01f, 0.01f, 0.01f);
        currentPanel.transform.localScale = Vector3.zero;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 8f;
            currentPanel.transform.localScale =
                Vector3.Lerp(Vector3.zero, finalScale, t);
            yield return null;
        }
        currentPanel.transform.localScale = finalScale;
    }
}