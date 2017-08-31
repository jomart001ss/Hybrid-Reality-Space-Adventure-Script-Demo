using UnityEngine;
using System.Collections;

public class ColorManager : Singleton<ColorManager>
{
	public delegate void ColorChangeNotifier (Color? color, float transitionTime, bool randomize = false);
	public event ColorChangeNotifier OnColorChange;

	public void ChangeColor (Color? color, float transitionTime, bool randomize = false)
    {
		if (OnColorChange != null)
        {
            OnColorChange(color, transitionTime, randomize);
        }		
	}

	public Color ammoIncreaseCol, healthIncreaseCol, shootCol;
	public Color safeTargetCol, dangerTargetCol, enemyTargetCol;
	public Color starterLoadedCol, starterNotLoadedCol;
	public Color chargeLoadedCol, chargeNotLoadedCol;
	public Color burstLoadedCol, burstNotLoadedCol;
	public Color hitCol;
    public Color unlockableMenuAvailableBG;
    public Color unlockableMenuUnavailableBG;
    public Color unloclableMenuUnlockedBG;
    public Color passiveMenuLockedBG;
    public Color passiveMenuDeactivatedBG;
    public Color passiveMenuActivatedBG;
}
