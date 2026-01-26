using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;

public class UIMap : MonoBehaviour
{
    private GameObject prefab;

    private List<GameObject> points;

    private GameObject grid;

    private Game game;

    private GameObject player;

    private CameraSphere camSphere;
    



    void Start()
    {
        prefab = this.transform.Find("point").gameObject;

        grid = this.transform.Find("ImageGrid").gameObject;

        game = FindObjectOfType<Game>();
        player = FindObjectOfType<PlayerController>().gameObject;
        camSphere = FindObjectOfType<CameraSphere>();


        points = new List<GameObject>();
        for(int i = 0; i < 20; i++)
        {
            GameObject o = Instantiate(prefab, this.transform);
            points.Add(o);
        }

        //invoke 
        InvokeRepeating("UpdateMap", 0.2f, 4f);
        InvokeRepeating("UpdateGrid", 0.2f, 1f);
    }

    private void Update()
    {
        //viuw angle
        float angle = Vector3.SignedAngle(camSphere.transform.forward.OnOceanPlane(), Vector3.forward, -Vector3.up);
        this.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, angle);
    }


    void UpdateGrid()
    {
        //grid
        Vector2 localoffsetNorm = new Vector2(player.transform.position.x, player.transform.position.z) / 10000f;
        grid.GetComponent<RectTransform>().anchoredPosition = -localoffsetNorm * 750f;
    }


    void UpdateMap()
    {
        List<IShipController> ships = new List<IShipController>();
        //ships.Add(FindObjectOfType<PlayerController>());//不显示玩家点
        ships.AddRange(FindObjectsOfType<AIController>());


        //points
        for (int i = 0; i < points.Count; i++)
        {
            if (i < ships.Count)
            {
                points[i].SetActive(true);
                Vector3 pos = new Vector3(ships[i].GameObj.transform.position.x, 0f, ships[i].GameObj.transform.position.z);
                Vector2 localoffsetNormalized = new Vector2((pos - player.transform.position).x / game.viewDistance, (pos - player.transform.position).z / game.viewDistance);

                points[i].GetComponent<RectTransform>().anchoredPosition = localoffsetNormalized * (this.GetComponent<RectTransform>().sizeDelta.x / 2f);

                if (ships[i].Enabled)
                {
                    if (ships[i].Team == 0)
                    {
                        if(ships[i] is PlayerController)
                        {
                            points[i].GetComponent<Image>().color = Color.white;
                        }
                        else
                        {
                            points[i].GetComponent<Image>().color = Color.green;
                        }
                    }
                    else
                    {
                        points[i].GetComponent<Image>().color = Color.red;
                    }
                }
                else
                {
                    points[i].GetComponent<Image>().color = Color.gray;
                }

                points[i].GetComponent<Image>().DOFade(0f, 3.5f);

            }
            else
            {
                points[i].SetActive(false);
            }
        }
    }

}
