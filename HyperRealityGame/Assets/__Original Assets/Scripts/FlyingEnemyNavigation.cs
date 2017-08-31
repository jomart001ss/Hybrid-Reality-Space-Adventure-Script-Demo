using UnityEngine;
using System.Collections;

/// <summary>
/// Used to move flying enemies. It'll hover a certain distance
/// from the ground and gradually move towards the player maintaining
/// the height. Once it gets within it's attack range it'll stop 
/// moving and turn on an attack using the AttackManager script.
/// Once the attack ends SwitchToRoaming will be called and
/// the cycle will continue.
/// </summary>
public class FlyingEnemyNavigation : EnemyNavigation 
{
}
