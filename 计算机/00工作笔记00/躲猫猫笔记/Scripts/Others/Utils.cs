using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils
{
    //Ability
    public static Ability GetAbility(CharactorInstance chara, int team)
    {
        if (chara.info.abilities.Count < 1) return null;

        AbilityInfo abinfo = chara.info.abilities[0];

        Ability ab;
        switch (abinfo.id)
        {
            case 0:
                {
                    ab = new AbilityRun(abinfo);
                }
                break;
            case 1:
                {
                    ab = new AbilityHeal(abinfo);
                }
                break;
            case 2:
                {
                    ab = new AbilityMask(abinfo);
                }
                break;
            case 3:
                {
                    ab = new AbilityRespawn(abinfo);
                }
                break;
            case 4:
                {
                    ab = new AbilityGuard(abinfo);
                }
                break;
            default:
                ab = null;
                break;
        }

        return ab;
    }
}

public class UtilsGame
{
    //Charactor
    public static Vector3 GetCellCenterPos(Vector3 pos)
    {
        return new Vector3(Mathf.FloorToInt(pos.x) + 0.5f, Mathf.FloorToInt(pos.y + 0.1f), Mathf.FloorToInt(pos.z) + 0.5f);
    }

    public static Vector3 GetFirePos(Vector3 charactorPos)
    {
        return new Vector3(charactorPos.x, charactorPos.y + 1f, charactorPos.z);
    }
    


    /// <summary>
    /// 扫描某个圆圈范围内的地面点。
    /// </summary>
    /// <param name="needinSafeArea">是否要在安全区</param>
    /// <param name="radius">搜索半径</param>
    /// <param name="center">center的y坐标无关紧要</param>
    /// <param name="limitMinMax">y坐标的范围限制</param>
    /// <returns></returns>
    public static Vector3 ScanRandomPos(IGame game, bool needinSafeArea, int radius, Vector3 center, Vector2 limitMinMax)
    {
        Vector3 posResult = new Vector3();

        float minMaxLength = limitMinMax.y - limitMinMax.x;

        for (int i = 0; i < 50; i++)
        {
            Vector3 rdmPos = UtilsGame.GetCellCenterPos(center + new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius)));
            if (needinSafeArea && !game.IsSafeArea(rdmPos))
            {
                continue;
            }

            RaycastHit hit;
            RaycastHit hitReverse;
            Vector3 raystartPos = new Vector3(rdmPos.x, limitMinMax.y + 0.1f, rdmPos.z);
            if (Physics.Raycast(new Ray(raystartPos, -Vector3.up), out hit, minMaxLength + 0.2f))
            {
                //reverse raycast
                if(Physics.Raycast(new Ray(hit.point - new Vector3(0f, 0.1f, 0f), Vector3.up), out hitReverse, minMaxLength + 0.2f))
                {
                    if(hitReverse.point.y < raystartPos.y)
                    {
                        continue;
                    }
                }

                //if in y range
                if (hit.point.y < limitMinMax.y    &&    hit.point.y > limitMinMax.x)
                {
                    posResult = hit.point;
                    break;
                }
            }
        }

        return posResult;
    }


    //Talents
    public static void CalPropertyAdd(List<TalentNodeInfo> talents, ref float attackMul, ref float hpMul, ref float speedMul)
    {
        foreach (TalentNodeInfo talent in talents.Where(t => t.group == "add"))
        {

            switch (talent.id)
            {
                case 0:
                case 5:
                case 12:
                    speedMul *= ((100f + (float)talent.value) / 100f);
                    break;
                case 2:
                case 7:
                    attackMul *= ((100f + (float)talent.value) / 100f);
                    break;
                case 14:
                case 21:
                    hpMul *= ((100f + (float)talent.value) / 100f);
                    break;
                default:
                    break;
            }
        }
    }
}
