using UnityEngine;
using HighlightingSystem;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Class that control the attacks for each enemy type. The script is placed
/// on each enemy as well as all attacks as seperate scripts. 
/// 
/// These attack scripts must inherit from the Attack class and must be disabled by default.
/// Once an attack is randomly chosen from here it'll be enabled to start it.
/// Each attack script must also call InitAttackVariables in Awake to initialize 
/// various useful variables. Once the attack is finished groundEnemyNavigation.SwitchToRoaming (this)
/// or flyingEnemyNavigation.SwitchToRoaming (this) needs to be called to
/// switch back into roaming mode and end the attack by disabling it. After 
/// this another attack will be randomly chosen continuing the whole cycle.
/// For examples of these attacks go to Enemies/Attacks.
/// </summary>
public class AttackManager : MonoBehaviour 
{
}
