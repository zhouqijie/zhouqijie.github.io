using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Badge
{
    //Rank Convert
    public static int UserLevelGetRank(int level)
    {
        return level / 4;
    }
    public static int UserLevelGetStarCount(int level)
    {
        return (level % 4) + 1;
    }



    public static int levelPrevious;

    public static int levelCurrent;


    public static void Set(Transform transBadge, int level)
    {
        int rankInt = UserLevelGetRank(level);
        int starsCount = UserLevelGetStarCount(level);


        transBadge.Find("ImageRank").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Ranks/rank" + rankInt.ToString());
        transBadge.Find("ImageText").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Ranks/rankt" + rankInt.ToString());
        for (int i = 0; i < transBadge.Find("Stars").childCount; i++)
        {
            if(i < starsCount)
            {
                transBadge.Find("Stars").GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                transBadge.Find("Stars").GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public static void Anim(Transform transBadge)
    {
        Set(transBadge, Badge.levelPrevious);
        if (levelCurrent == levelPrevious) return;
        
        int rankIntPrevious = UserLevelGetRank(Badge.levelPrevious);
        int starsCountPrevious = UserLevelGetStarCount(Badge.levelPrevious);

        int rankIntCurrent = UserLevelGetRank(Badge.levelCurrent);
        int starsCountCurrent = UserLevelGetStarCount(Badge.levelCurrent);
        

        //大勋章图标//-------ANIM-----
        transBadge.Find("ImageText").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Ranks/rankt" + rankIntCurrent.ToString());
        transBadge.Find("ImageRank").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Ranks/rank" + rankIntCurrent.ToString());


        if (rankIntCurrent > rankIntPrevious)
        {
            //大段位提示动画
            transBadge.Find("ImageRank").localScale = new Vector3(2f, 2f, 2f);
            transBadge.Find("ImageRank").DOScale(Vector3.one, 0.5f);
            //大段位提升声音
        }
        else
        {
            //大段位掉声音（无）
        }


        //星星图标//-------ANIM-----
        if (starsCountCurrent > starsCountPrevious)
        {
            for(int i = 0; i < transBadge.Find("Stars").childCount; i++)
            {
                if(i >= starsCountPrevious && i < starsCountCurrent)
                {
                    Transform starInner = transBadge.Find("Stars").GetChild(i).GetChild(0);
                    starInner.gameObject.SetActive(true);
                    starInner.localScale = new Vector3(2, 2, 2);
                    starInner.DOScale(Vector3.one, 0.5f);
                }
            }
        }
        else
        {
            for (int i = 0; i < transBadge.Find("Stars").childCount; i++)
            {
                if (i >= starsCountCurrent && i < starsCountPrevious)
                {
                    Transform starInner = transBadge.Find("Stars").GetChild(i).GetChild(0);
                    starInner.gameObject.SetActive(false);
                }
            }
        }

        
    }
}


public class Vip
{
    public static int GetLevel(int exp)
    {
        return exp / 100;
    }
}