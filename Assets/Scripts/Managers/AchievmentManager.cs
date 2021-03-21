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

    public bool CheckAchievment(int index, GameStats stats)
    {
        AchievmentUnit current = achievments[index];

        bool completed = true;

        Debug.Log(current.achs.Count);
        foreach (var a in current.achs)
        {
            switch (a.type)
            {
                case AchievmentType.POINTS:
                    if (stats.score < a.value)
                    {
                        a.ui.SetText("points " + (a.value - stats.score).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.TIME:
                    if (stats.timer < a.value)
                    {
                        a.ui.SetText((a.value - stats.timer).ToString("n1"));
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.AMBULANCE:
                    if (stats.ambulanceCount < a.value)
                    {
                        a.ui.SetText((a.value - stats.ambulanceCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.POLICE:
                    if (stats.policeCount < a.value)
                    {
                        a.ui.SetText((a.value - stats.policeCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.FIREFIGHTER:
                    if (stats.firefighterCount < a.value)
                    {
                        a.ui.SetText((a.value - stats.firefighterCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.TAXI:
                    a.ui.SetText((a.value - stats.taxiCount).ToString());
                    if (stats.taxiCount >= a.value)
                    {
                        a.ui.SetText((a.value - stats.taxiCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.OLDMAN:
                    if (stats.oldmanCount < a.value)
                    {
                        a.ui.SetText((a.value - stats.oldmanCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.NORMALCAR:
                    if (stats.normalCarsCount < a.value)
                    {
                        a.ui.SetText((a.value - stats.normalCarsCount).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.TOTALCARS:
                    if (stats.carPassed < a.value)
                    {
                        a.ui.SetText((a.value - stats.carPassed).ToString());
                        completed &= false;
                        continue;
                    }
                    else
                    {
                        a.ui.SetText("OK");
                    }
                    break;
                case AchievmentType.FIX:
                    if (stats.carFixedCount < a.value)
                    {
                        a.ui.SetText((a.value - stats.carFixedCount).ToString());
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
