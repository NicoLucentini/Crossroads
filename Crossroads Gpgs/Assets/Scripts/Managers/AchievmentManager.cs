using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievmentManager : MonoBehaviour
{

    

    public List<AchievmentUnit> achievments;
    public List<AchievmentSprites> aSprites;
    public AchievmentUnit current;
    

    private void Awake()
    {
        foreach (var a in achievments)
        {
            foreach (var au in a.achs)
            {
                foreach (var s in aSprites)
                {
                    if (au.type == s.type)
                        au.sprite = s.sprite;
                }
            }
        }
    }

    //el 1 es el primero
    public AchievmentUnit GetCurrent(int index)
    {
        if (index - 1 >= 0 && index - 1 < achievments.Count)
            return achievments[index - 1];
        return null;
    }

    public bool CheckAchievment(int index, GameManager mgr)
    {
        AchievmentUnit current = achievments[index];

        bool completed = true;

        Debug.Log(current.achs.Count);
        foreach (var a in current.achs)
        {
            switch (a.type)
            {
                case AchievmentType.POINTS:
                    if (mgr.score < a.value)
                    {
                        a.ui.SetText("points " + (a.value - mgr.score).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.TIME:
                    if (mgr.spawner.timer < a.value)
                    {
                        a.ui.SetText((a.value - mgr.spawner.timer).ToString("n1"));
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.AMBULANCE:
                    if (mgr.ambulanceCount < a.value)
                    {
                        a.ui.SetText((a.value - mgr.ambulanceCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.POLICE:
                    if (mgr.policeCount < a.value)
                    {
                        a.ui.SetText((a.value - mgr.policeCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.FIREFIGHTER:
                    if (mgr.firefighterCount < a.value)
                    {
                        a.ui.SetText((a.value - mgr.firefighterCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.TAXI:
                    a.ui.SetText((a.value - mgr.taxiCount).ToString());
                    if (mgr.taxiCount >= a.value)
                    {
                        a.ui.SetText((a.value - mgr.taxiCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.OLDMAN:
                    if (mgr.oldmanCount < a.value)
                    {
                        a.ui.SetText((a.value - mgr.oldmanCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.NORMALCAR:
                    if (mgr.normalCarsCount < a.value)
                    {
                        a.ui.SetText((a.value - mgr.normalCarsCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.TOTALCARS:
                    if (mgr.carPassed < a.value)
                    {
                        a.ui.SetText((a.value - mgr.carPassed).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.FIX:
                    if (mgr.carFixed < a.value)
                    {
                        a.ui.SetText((a.value - mgr.carFixed).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
            }
        }

        return completed;
    }
}
[System.Serializable]
public class AchievmentSprites
{
    public AchievmentType type;
    public Sprite sprite;
}


[System.Serializable]
public class Achievment
{
    public AchievmentType type;
    public int value;
    [ReadOnly]public Sprite sprite;
    [ReadOnly]public UISituation ui;
}
[System.Serializable]
public class AchievmentUnit
{
    public int xpReward;
    public int coinsReward;
    public List<Achievment> achs;
}
public enum AchievmentType
{
    NORMALCAR,
    TOTALCARS,
    POINTS,
    TIME,
    AMBULANCE,
    POLICE,
    FIREFIGHTER,
    TAXI,
    OLDMAN,
    FIX,
}
