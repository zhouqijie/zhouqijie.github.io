using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{

    public static float CalculateAngle(float l, float h, float v0, bool returnRad, float g = 9.8f)
    {
        float rad = (Mathf.Asin((v0 * v0 * h + g * l * l) / (Mathf.Sqrt(h * h + l * l) * v0 * v0)) - Mathf.Atan(-h / l)) / 2f;
        if (returnRad)
        {
            return rad;
        }
        else
        {
            return (rad / Mathf.PI) * 180f;
        }
    }

    public static float CalculateAngle(float l, float h, float v0, out float t, bool returnRad, float g = 9.8f)
    {
        float rad = (Mathf.Asin((v0 * v0 * h + g * l * l) / (Mathf.Sqrt(h * h + l * l) * v0 * v0)) - Mathf.Atan(-h / l)) / 2f;
        t = l / (Mathf.Cos(rad) * v0);
        if (returnRad)
        {
            return rad;
        }
        else
        {
            return (rad / Mathf.PI) * 180f;
        }
    }



    




    /// <summary>
    /// 计算等价加成
    /// </summary>
    /// <param name="lvl"></param>
    /// <returns></returns>
    public static float CalFac(int lvl)
    {
        int clampedLvl = Mathf.Clamp(lvl, 1, 30);
        float fac = 1f;

        if(clampedLvl > 5)
        {
            fac *= Mathf.Pow(1.15f, 4);
        }
        else
        {
            fac *= Mathf.Pow(1.15f, clampedLvl - 1);
            return fac;
        }


        if(clampedLvl > 10)
        {
            fac *= Mathf.Pow(1.13f, 5);
        }
        else
        {
            fac *= Mathf.Pow(1.13f, clampedLvl - 5);
            return fac;
        }


        if(clampedLvl > 15)
        {
            fac *= Mathf.Pow(1.1f, 5);
        }
        else
        {
            fac *= Mathf.Pow(1.1f, clampedLvl - 10);
            return fac;
        }

        if(clampedLvl > 20)
        {
            fac *= Mathf.Pow(1.08f, 5);
        }
        else
        {
            fac *= Mathf.Pow(1.08f, clampedLvl - 15);
            return fac;
        }


        if(clampedLvl > 25)
        {
            fac *= Mathf.Pow(1.06f, 5);
        }
        else
        {
            fac *= Mathf.Pow(1.06f , clampedLvl - 20);
            return fac;
        }
        
        if(clampedLvl > 25)
        {
            fac *= Mathf.Pow(1.05f, clampedLvl - 25);
            return fac;
        }


        return fac;
    }

    /// <summary>
    /// 计算舰长属性加成
    /// </summary>
    /// <param name="val"></param>
    /// <param name="lvl"></param>
    /// <returns></returns>
    public static float CalFacCrew(float val, float lvl)
    {
        float add = (val / 100f) * (1f + (lvl * 0.01f));

        return 1f + add;
    }


    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="attack"></param>
    /// <param name="effectarmor"></param>
    public static float CalDamage(float attack, float effectarmor)
    {
        float trueDamage = (attack * attack) / (attack + 5f * effectarmor);

        trueDamage *= Random.Range(0.8f, 1.2f);

        return trueDamage;
    }
    public static float CalDamage(float attack, Hull hull, float piercingPercentage)
    {
        float effectArmor = hull.Armor * hull.armorMultipiler * (1f - piercingPercentage);
        float trueDamage = (attack * attack) / (attack + 5f * effectArmor);

        trueDamage *= Random.Range(0.8f, 1.2f);

        return trueDamage;
    }

    /// <summary>
    /// 计算战舰解锁花销
    /// </summary>
    /// <param name="shipinfo"></param>
    /// <returns></returns>
    public static int CalShipUnlockCost(ShipInfo shipinfo)
    {
        return 10;
    }

    /// <summary>
    /// 计算战舰升级花销
    /// </summary>
    /// <param name="shipLvl"></param>
    /// <param name="shipRank"></param>
    /// <returns></returns>
    public static int CalShipUpgradeCost(int shipLvl, int shipRank)
    {
        //return 100 * shipLvl * shipRank;
        return Mathf.RoundToInt(100 * Utils.CalFac(shipLvl) * shipRank);
    }

    /// <summary>
    /// 计算部件升级花销
    /// </summary>
    /// <param name="comLvl"></param>
    /// <param name="comRank"></param>
    /// <returns></returns>
    public static int CalComUpgradeCost(int comLvl, int comRank)
    {
        //return 1 * comLvl * comRank;
        return Mathf.RoundToInt(1 * Utils.CalFac(comLvl) * comRank);
    }

    /// <summary>
    /// 计算拆建收获
    /// </summary>
    /// <param name="comLvl"></param>
    /// <param name="comRank"></param>
    /// <returns></returns>
    public static int CalComDecomGet(int comLvl, int comRank, float getPercentage)
    {
        int sum = 0;
        for(int i = 1; i < comLvl; i++)
        {
            sum += CalComUpgradeCost(comLvl, comRank);
        }

        int result = 1 + Mathf.RoundToInt(sum * getPercentage);//一点基础

        return result;
    }

    /// <summary>
    /// 船员升级花销
    /// </summary>
    /// <param name="crew"></param>
    /// <returns></returns>
    public static int CalCrewUpgradeCost(Crew crew)
    {
        int ranklvl = ToRankLvl(crew.crewInfo.rank);

        return ranklvl * 10 * crew.lvl;
    }





    //---------------------------------Rank char-----------------------------------------------------------------
    public static int ToRankLvl(string rank)
    {
        int lvl = 0;
        switch (rank)
        {
            case "D":
                {
                    lvl = 0;
                }
                break;
            case "C":
                {
                    lvl = 1;
                }
                break;
            case "B":
                {
                    lvl = 2;
                }
                break;
            case "A":
                {
                    lvl = 3;
                }
                break;
            case "S":
                {
                    lvl = 4;
                }
                break;
            default:
                break;
        }
        return lvl;
    }

    //----------------------------------Text---------------------------------

    public static string AbilityIntroduce(string str)
    {
        string result = "";

        switch (str)
        {
            case "维修":
                {
                    result = "修复战舰损耗。损耗约严重，修复效果越好。";
                }
                break;
            case "精确射击":
                {
                    result = "下一轮炮击的精确度大幅提升。";
                }
                break;
            case "终极防御":
                {
                    result = "一定时间内大幅提升防御力。";
                }
                break;
            case "发射鱼雷":
                {
                    result = "发射一轮威力巨大的鱼雷。";
                }
                break;
            case "反舰导弹":
                {
                    result = "锁定目标后可发射一枚反舰导弹。";
                }
                break;
            default:
                break;
        }

        return result;
    }

    public static string AbilityUse(string str)
    {
        string result = "";

        switch (str)
        {
            case "维修":
                {
                    result = "战舰已修复";
                }
                break;
            case "精确射击":
                {
                    result = "精确射击已激活";
                }
                break;
            case "终极防御":
                {
                    result = "终极防御已激活  防御力大幅提升";
                }
                break;
            case "发射鱼雷":
                {
                    result = "鱼雷已发射";
                }
                break;
            case "反舰导弹":
                {
                    result = "反舰巡航导弹已发射";
                }
                break;
            default:
                break;
        }

        return result;
    }

    public static Color RankColor(string rank)
    {
        Color clr;
        switch (rank)
        {
            case "C":
                clr = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                break;
            case "B":
                clr = new Color(0.4f, 0.4f, 1f, 1.0f);
                break;
            case "A":
                clr = new Color(1f, 0.4f, 1f, 1f);
                break;
            case "S":
                clr = new Color(1f, 1f, 0.4f, 1.0f);
                break;
            default:
                clr = new Color(1f, 1f, 1f, 1.0f);
                break;
        }
        return clr;
    }


    //--------------------------------------Ad--------------------------------------------------------------------



    public static void Ad(UnityEngine.Events.UnityAction callBackSuccess, UnityEngine.Events.UnityAction callBackFail)
    {
        //99 - 广告
    }
}





public static class StaticClass
{
    //---------------------------------------------------------------------文字处理映射-------------------------------------------------------------------------------


    public static string ToChinese(this string str)
    {
        string result;
        switch (str)
        {
            case "Material":
            case "Materials":
                result = "升级材料";
                break;
            case "Debris":
                result = "碎片";
                break;
            case "Money":
                result = "金币";
                break;
            default:
                result = "？";
                break;
        }

        return result;
    }


    public static Vector3 OnOceanPlane(this Vector3 pos, float height = 0f)
    {
        return new Vector3(pos.x, height, pos.z);
    }
}