using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public IGame Game { get { return this.GetComponent<IGame>(); } }


    private void Awake()
    {
        switch (Infomanager.gameType)
        {
            case 0:
                {
                    this.gameObject.AddComponent<ClassicGame>();

                    if(Infomanager.GetInstance().userData.totalClassicGames < 2)
                    {
                        this.gameObject.AddComponent<InGameGuider>();
                    }
                }
                break;
            case 1:
                {
                    this.gameObject.AddComponent<BattleGame>();

                    if (Infomanager.GetInstance().userData.totalBattleGames < 2)
                    {
                        this.gameObject.AddComponent<InGameGuider>();
                    }
                }
                break;
            default:
                break;
        }


        
    }
}
