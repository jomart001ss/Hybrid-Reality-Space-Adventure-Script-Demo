using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UILook : MonoBehaviour
{
    protected Transform newTransform;
    protected Transform playerTransform;
    protected Image image;
    protected Color originalColor;
    public GameObject toEnableOnHover;
    public GameObject toEnableOnHover2;
    protected Rewired.Player player;

    protected void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        newTransform = GetComponent<Transform>();
        player = Controller.instance.player;
        if (toEnableOnHover != null)
        {
            toEnableOnHover.SetActive(false);
        }
        if (toEnableOnHover2 != null)
        {
            toEnableOnHover2.SetActive(false);
        }
    }

    public bool selectByDefault = false;

    protected void OnEnable()
    {
        if (selectByDefault)
        {
            Select(false);
        }
        else
        {
            Deselect();
        }
        playerTransform = VRManager.instance.currentCamera.transform;
    }

    public void Deselect(bool disabling = false)
    {
        StopAllCoroutines();
        if (!disabling)
        {
            StartCoroutine(ChangeColor(image.color, originalColor, 3f));
        }
        if (toEnableOnHover != null)
        {
            toEnableOnHover.SetActive(false);
        }
        if (toEnableOnHover2 != null)
        {
            toEnableOnHover2.SetActive(false);
        }
        highlighted = false;
    }

    protected IEnumerator ChangeColor(Color colorA, Color colorB, float speed)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * speed;
            image.color = Color.Lerp(colorA, colorB, timer);
            yield return null;
        }
    }

    protected bool highlighted = false;
    public bool alwaysHighlight = false;
    protected static UILook lockedSelection;
    public float minimumDistance;

    protected void Update()
    {
        if (alwaysHighlight)
        {
            CheckIfHighlighted();
        }
        else if (highlighted)
        {
            GetInput();
        }
    }

    [HideInInspector] public static UILook currentlyHighlighted;

    protected void CheckIfHighlighted()
    {
        if (highlighted)
        {
            GetInput();
        }
        else
        {
            if (currentlyHighlighted != null)
            {
                currentlyHighlighted.Deselect();
            }
            Select();
        }
    }

    public Color highlightedCol;
    public AudioClip selectedSound;
    private bool justSelected = false;

    public virtual void Select (bool playSound = true)
    {
        justSelected = true;
        StopAllCoroutines();
        currentlyHighlighted = this;
        image.color = highlightedCol;
        highlighted = true;
        if (selectedSound != null && playSound)
        {
            AudioManager.instance.Play(selectedSound, Vector3.zero, VolumeGroup.MenuSound, 0.07f, 1f, 0f, false, 256);
        }
    }

    public UILook leftUI;
    public UILook rightUI;
    public UILook topUI;
    public UILook bottomUI;
    private float leftStickDeadZone = 0.35f;

    protected virtual void GetInput()
    {
        if (justSelected)
        {
            justSelected = false;
            return;
        }
        if (lockedSelection == null)
        {
            Rewired.Player player = Controller.instance.player;
            if (Input.GetKeyDown(KeyCode.A) ||
                player.GetButtonDown("D-Pad Left") ||
                Controller.instance.leftStickLeftFlick ||
                Controller.instance.touchLeftStickLeftFlick)
            {
                if (leftUI != null)
                {
                    Deselect();
                    leftUI.Select();
                }
            }
            else if (Input.GetKeyDown(KeyCode.D) ||
                player.GetButtonDown("D-Pad Right") ||
                Controller.instance.leftStickRightFlick ||
                Controller.instance.touchLeftStickRightFlick)
            {
                if (rightUI != null)
                {
                    Deselect();
                    rightUI.Select();
                }
            }
            else if (Input.GetKeyDown(KeyCode.W) ||
                player.GetButtonDown("D-Pad Up") ||
                Controller.instance.leftStickUpFlick ||
                Controller.instance.touchLeftStickUpFlick)
            {
                if (topUI != null)
                {
                    Deselect();
                    topUI.Select();
                }
            }
            else if (Input.GetKeyDown(KeyCode.S) ||
                player.GetButtonDown("D-Pad Down") ||
                Controller.instance.leftStickDownFlick ||
                Controller.instance.touchLeftStickDownFlick)
            {
                if (bottomUI != null)
                {
                    Deselect();
                    bottomUI.Select();
                }
            }
            else if (Controller.instance.player.GetButtonDown("Select") ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.E) /*||
            OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Touch)*/)
            {
                Activate();
            }
        }
    }

    protected virtual void Activate ()
    {

    }

    protected void OnDisable()
    {
        if (highlighted)
        {
            Deselect(true);
            currentlyHighlighted = null;
            highlighted = false;
        }
    }
}
