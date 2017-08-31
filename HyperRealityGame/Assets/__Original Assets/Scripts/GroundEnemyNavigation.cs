using UnityEngine;
using System.Collections;

/// <summary>
/// Used to move grounded enemies. It'll stick onto nav meshes
/// and gradually move towards the players. Once it gets within it's 
/// attack range it'll stop moving and turn on an attack using 
/// the AttackManager script. Once the attack ends SwitchToRoaming 
/// will be called and the cycle will continue.
/// </summary>
public class GroundEnemyNavigation : EnemyNavigation 
{
}
