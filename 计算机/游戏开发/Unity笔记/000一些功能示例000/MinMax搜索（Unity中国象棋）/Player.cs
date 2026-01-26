using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Judge judge;

    [HideInInspector]
    public Piece currentPiece;
    void Awake () {
        judge = GameObject.Find ("Judge").GetComponent<Judge> ();

        currentPiece = null;
    }
    void Start () {

    }

    void Update () {
        if (judge.IsPlayerTurn) {
            if (Input.GetKeyDown (KeyCode.Mouse0)) {
                RaycastHit hitInfo;
                bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
                if (isHit) {

                    GameObject hitTarget = hitInfo.collider.gameObject;

                    if (currentPiece != null) {
                        //POS
                        Pos clickPos = hitTarget.GetComponent<Chunk> ().Position;
                        Pos piecePos = currentPiece.GetComponentInParent<Chunk> ().Position;

                        //Judge
                        if (judge.Validate(currentPiece, hitTarget.GetComponent<Chunk>())) {
                            
                            currentPiece.MoveTo(hitTarget.GetComponent<Chunk>());
                            CancelChoose ();
                            FinishTurn ();
                            
                        } else {
                            CancelChoose ();
                        }
                    } else {
                        if (hitTarget.transform.childCount > 0) {
                            if(hitTarget.transform.GetChild(0).GetComponent<Piece>().IsPlayer){
                                Choose (hitTarget.transform.GetChild (0).GetComponent<Piece> ());
                            }
                        }
                    }
                }
            }
        }
    }

    void Choose (Piece piece) {
        currentPiece = piece;
        currentPiece.gameObject.GetComponent<MeshRenderer> ().materials[0].EnableKeyword ("_EMISSION");
    }

    void CancelChoose () {
        currentPiece.gameObject.GetComponent<MeshRenderer> ().materials[0].DisableKeyword ("_EMISSION");
        currentPiece = null;
    }

    void FinishTurn () {
        judge.PlayerFinish ();
    }

}