using UnityEngine;
using System.Collections;

public class DestructiveSingletonManager : DestructiveSingleton<DestructiveSingletonManager>
{
    new void Awake ()
    {
        base.Awake();
    }
}
