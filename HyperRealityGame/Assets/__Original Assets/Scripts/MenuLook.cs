using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuLook : UILook
{
    protected Text buttonText;
     
    new protected void Awake ()
    {
        base.Awake();
        if (toEnableOnHover != null)
        {
            buttonText = toEnableOnHover.transform.GetChild(0).GetComponent<Text>();
        }   
    }
    new protected void OnEnable ()
    {
        base.OnEnable();
    }

    void UpdateButtonUI (ControlScheme controlScheme)
    {
        ControlScheme currentControlScheme = DataManager.instance.GetControlSchemeSettings();
        string newLetter = "A";
        if (controlScheme == ControlScheme.Gamepad)
        {
            newLetter = "A";
        }
        else if (controlScheme == ControlScheme.MotionController)
        {
            newLetter = "A";
        }
        else if (controlScheme == ControlScheme.MouseKeyboard)
        {
            newLetter = "E";
        }
        buttonText.text = newLetter;
    }
    public override void Select (bool playSound = true)
	{
        base.Select(playSound);
        if (!(SceneManager.GetActiveScene().buildIndex == 0 &&
            VRManager.VRMode == VRMode.Desktop))
        {
            if (toEnableOnHover != null)
            {
                toEnableOnHover.SetActive(true);
            }
        }
        if (toEnableOnHover2 != null)
        {
            toEnableOnHover2.SetActive(true);
        }
    }

    public MenuAction menuAction;
    public MenuSwitcher menuSwitcher;
	public AudioClip activatedSound;
	public bool dontFadeOut;
	public Color selectedCol;

	protected override void Activate () 
	{
        if (menuAction != null)
        {
            menuAction.ActivateActions();
        }
        if (menuSwitcher != null)
        {
            menuSwitcher.ActivateActions();
        }
        if (activatedSound != null)
        {
            AudioManager.instance.Play(activatedSound, Vector3.zero, VolumeGroup.MenuSound, 0.05f, 1f, 0f, false);
        }
        StopAllCoroutines ();
		if (!dontFadeOut)
		{
			StartCoroutine (ChangeColor (selectedCol, highlightedCol, 2f));
		}
	}

    new void OnDisable ()
    {
        base.OnDisable();
    }
}
