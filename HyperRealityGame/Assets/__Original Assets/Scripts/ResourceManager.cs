using UnityEngine;
using System.Collections;

public enum Unlockable
{
    MachineGun,
    Laser,
    RocketLauncher,
    StealthDrive,
    Effect2,
    Effect3
}

[System.Serializable]
public class UnlockableInfo
{
    public Unlockable unlockable;
    public int pointsNeeded;
    public bool locked = true;
}

public class ResourceManager : Singleton<ResourceManager>
{
    public UnlockableInfo[] unlockableInformation;
    public int currentPoints;

    void Start()
    {
        
    }

    public void UpdatePoints (int points)
    {
        currentPoints += points;
        NotifyPointUpdate();
    }

    public delegate void PointNotifer();
    public event PointNotifer OnPointUpdate;

    public void NotifyPointUpdate()
    {
        if (OnPointUpdate != null)
        {
            OnPointUpdate();
        }
    }

    public bool TryToUnlock (Unlockable unlockable)
    {
        int index;
        UnlockableInfo unlockableInfo = GetUnlockableInfo(unlockable, out index);
        int pointedNeeded = unlockableInfo.pointsNeeded;
        if (pointedNeeded <= currentPoints)
        {
            currentPoints -= pointedNeeded;
            NotifyPointUpdate();
            Unlock(index);
            return true;
        }
        else
        {
            return false;
        }
    }

    public UnlockableInfo GetUnlockableInfo (Unlockable unlockable, out int index)
    {
        index = -1;
        for (int i = 0; i < unlockableInformation.Length; i++)
        {
            UnlockableInfo unlockableInfo = unlockableInformation[i];
            if (unlockableInfo.unlockable == unlockable)
            {
                index = i;
                return unlockableInfo;
            }
        }
        //shouldn't be reached
        return null;
    }

    void Unlock (int index)
    {
        UnlockableInfo unlockableInfo = unlockableInformation[index];
        unlockableInfo.locked = false;
        unlockableInformation[index] = unlockableInfo;
        NotifyUnlock(unlockableInfo.unlockable);
    }

    public delegate void UnlockNotifier (Unlockable unlockable);
    public event UnlockNotifier OnUnlock;

    public void NotifyUnlock (Unlockable unlockable)
    {
        if (OnUnlock != null)
        {
            OnUnlock(unlockable);
        }
    }

    public bool IsLocked (Unlockable unlockable)
    {
        for (int i = 0; i < unlockableInformation.Length; i++)
        {
            UnlockableInfo unlockableInfo = unlockableInformation[i];
            if (unlockableInfo.unlockable == unlockable)
            {
                return unlockableInfo.locked;
            }
        }
        return false;
    }
}
