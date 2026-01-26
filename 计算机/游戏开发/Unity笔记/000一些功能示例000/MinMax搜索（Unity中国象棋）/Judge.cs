using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{

	[HideInInspector]
	public GameObject currentChessBoard;
	private bool isPlayerTurn;
	private bool isAiTurn;
	public bool IsPlayerTurn { get { return isPlayerTurn; } }
	public bool IsAiTurn { get { return isAiTurn; } }
	// Use this for initialization
	void Start ()
	{
		currentChessBoard = GameObject.FindGameObjectWithTag ("ChessBoard");
		isPlayerTurn = true;
		isAiTurn = false;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void PlayerFinish ()
	{
		isPlayerTurn = false;
		isAiTurn = false;
		StartCoroutine(JudgeWinLose(true));
	}
	public void AiFinish ()
	{
		isPlayerTurn = false;
		isAiTurn = false;
		StartCoroutine(JudgeWinLose(false));
	}

	IEnumerator JudgeWinLose (bool playerFinish)
	{ //需要等待Destroy

		yield return new WaitForEndOfFrame ();

		if (GameObject.Find ("JIANG1") != null && GameObject.Find ("JIANG2") != null)
		{
			if(playerFinish){
				isAiTurn = true;
				Displayer.Display("AI回合");
			}
			else{
				isPlayerTurn = true;
				Displayer.Display("玩家回合");
			}
		}
		else
		{
			if (GameObject.Find ("JIANG1") == null)
			{
				Displayer.Display ("你输了");
			}
			if (GameObject.Find ("JIANG2") == null)
			{
				Displayer.Display ("你赢了");
			}
		}
	}

	public Chunk GetChunk (int x, int y)
	{

		Vector3 localPosition = new Vector3 ((float) y, 0, (float) x);
		Collider[] colliders = Physics.OverlapSphere (currentChessBoard.transform.localToWorldMatrix * localPosition, 0.1f);
		if (colliders.Length > 0)
		{
			return colliders[0].GetComponent<Chunk> ();
		}
		else
		{
			return null;
		}
	}

	public bool Validate (Piece piece, Chunk targetChunk)
	{
		bool isValidated = false;

		//ZeroMovement
		if (targetChunk.GetComponentInChildren<Piece> () == piece)
		{
			return false;
		}

		//Occupied?  //Attacking?
		bool isEating = false;
		if (targetChunk.transform.childCount > 0)
		{
			if (targetChunk.transform.GetChild (0).GetComponent<Piece> ().IsPlayer == piece.IsPlayer)
			{
				return false;
			}
			else if (targetChunk.transform.GetChild (0).GetComponent<Piece> ().IsPlayer != piece.IsPlayer)
			{
				isEating = true;
			}
		}
		else
		{
			isEating = false;
		}

		//PieceChunk
		Chunk pieceChunk = piece.transform.parent.GetComponent<Chunk> ();
		//MoveNum
		int moveX = targetChunk.Position.x - pieceChunk.Position.x;
		int moveY = targetChunk.Position.y - pieceChunk.Position.y;
		//Judge By PieceType
		switch (piece.PieceType)
		{
			//---------------------------------------------------------------------------
			case PieceType.JIANG:
				{
					bool isRightPos = Math.Abs (moveX) + Math.Abs (moveY) == 1;
					bool isInGridX = targetChunk.Position.x <= 6 && targetChunk.Position.x >= 4;
					bool isInGridY = piece.IsPlayer ? targetChunk.Position.y <= 3 : targetChunk.Position.y >= 8;
					if (isInGridX && isInGridY && isRightPos)
					{
						isValidated = true;
					}
					break;
				}

				//---------------------------------------------------------------------------
			case PieceType.SHI:
				{
					bool isRightPos = Math.Abs (moveX) == 1 && Math.Abs (moveY) == 1;
					bool isInGridX = targetChunk.Position.x <= 6 && targetChunk.Position.x >= 4;
					bool isInGridY = piece.IsPlayer ? targetChunk.Position.y <= 3 : targetChunk.Position.y >= 8;
					if (isInGridX && isInGridY && isRightPos)
					{
						isValidated = true;
					}
					break;
				}

				//---------------------------------------------------------------------------
			case PieceType.XIANG:
				{
					bool isToSelfSide = piece.IsPlayer ? targetChunk.Position.y <= 5 : targetChunk.Position.y >= 6;
					bool isRightPos = Math.Abs (moveX) == 2 && Math.Abs (moveY) == 2;

					if (isRightPos && isToSelfSide)
					{
						Chunk centerChunk = GetChunk ((targetChunk.Position.x + pieceChunk.Position.x) / 2, (targetChunk.Position.y + pieceChunk.Position.y) / 2);
						bool isNotBlocked = centerChunk.transform.childCount == 0;
						if (isNotBlocked)
						{
							isValidated = true;
						}
					}
					break;
				}
				//---------------------------------------------------------------------------
			case PieceType.MA:
				{
					bool isRightPos12 = Math.Abs (moveX) == 1 && Math.Abs (moveY) == 2;
					bool isRightPos21 = Math.Abs (moveX) == 2 && Math.Abs (moveY) == 1;
					if (isRightPos21)
					{
						Chunk blockChunk = GetChunk (pieceChunk.Position.x + Math.Sign (moveX), pieceChunk.Position.y);
						bool isNotBlock = blockChunk.transform.childCount == 0;
						if (isNotBlock)
						{
							isValidated = true;
						}
					}
					if (isRightPos12)
					{
						Chunk blockChunk = GetChunk (pieceChunk.Position.x, pieceChunk.Position.y + Math.Sign (moveY));
						bool isNotBlocked = blockChunk.transform.childCount == 0;
						if (isNotBlocked)
						{
							isValidated = true;
						}
					}
					break;
				}
			case PieceType.JU:
				{
					bool isInLineXn = pieceChunk.Position.x == targetChunk.Position.x;
					bool isInLineYn = pieceChunk.Position.y == targetChunk.Position.y;
					if (isInLineXn)
					{
						int biggerY = Math.Max (targetChunk.Position.y, pieceChunk.Position.y);
						int smallerY = Math.Min (targetChunk.Position.y, pieceChunk.Position.y);
						bool isNotBlocked = true;
						for (int y = smallerY + 1; y < biggerY; y++)
						{
							Chunk chunk = GetChunk (pieceChunk.Position.x, y);
							if (chunk.transform.childCount > 0) isNotBlocked = false;
						}
						if (isNotBlocked)
						{
							isValidated = true;
						}
					}
					if (isInLineYn)
					{
						int biggerX = Math.Max (targetChunk.Position.x, pieceChunk.Position.x);
						int smallerX = Math.Min (targetChunk.Position.x, pieceChunk.Position.x);
						bool isNotBlocked = true;
						for (int x = smallerX + 1; x < biggerX; x++)
						{
							Chunk chunk = GetChunk (x, pieceChunk.Position.y);
							if (chunk.transform.childCount > 0) isNotBlocked = false;
						}
						if (isNotBlocked)
						{
							isValidated = true;
						}
					}
					break;
				}
			case PieceType.PAO:
				{
					bool isInLineXn = pieceChunk.Position.x == targetChunk.Position.x;
					bool isInLineYn = pieceChunk.Position.y == targetChunk.Position.y;
					if (isInLineXn)
					{
						int biggerY = Math.Max (targetChunk.Position.y, pieceChunk.Position.y);
						int smallerY = Math.Min (targetChunk.Position.y, pieceChunk.Position.y);
						int numBlocks = 0;
						for (int y = smallerY + 1; y < biggerY; y++)
						{
							Chunk chunk = GetChunk (pieceChunk.Position.x, y);
							if (chunk.transform.childCount > 0) numBlocks++;
						}

						if (!isEating && numBlocks == 0)
						{
							isValidated = true; //Normal Move
						}
						else if (isEating && numBlocks == 1)
						{
							isValidated = true; //Attacking Move
						}
					}
					if (isInLineYn)
					{
						int biggerX = Math.Max (targetChunk.Position.x, pieceChunk.Position.x);
						int smallerX = Math.Min (targetChunk.Position.x, pieceChunk.Position.x);
						int numBlocks = 0;
						for (int x = smallerX + 1; x < biggerX; x++)
						{
							Chunk chunk = GetChunk (x, pieceChunk.Position.y);
							if (chunk.transform.childCount > 0) numBlocks++;
						}

						if (!isEating && numBlocks == 0)
						{
							isValidated = true; //Normal Move
						}
						else if (isEating && numBlocks == 1)
						{
							isValidated = true; //Attacking Move
						}
					}
					break;
				}
			case PieceType.BING:
				{
					bool isInSelfSide = piece.IsPlayer ? pieceChunk.Position.y <= 5 : pieceChunk.Position.y >= 6;

					if (isInSelfSide)
					{
						bool isRightPos = (moveX == 0 && moveY == (piece.IsPlayer ? 1 : -1));
						if (isRightPos)
						{
							isValidated = true;
						}
					}
					else
					{
						bool isRightPos = Math.Abs (moveX) + Math.Abs (moveY) == 1 && (piece.IsPlayer ? moveY >= 0 : moveY <= 0);
						if (isRightPos)
						{
							isValidated = true;
						}
					}
					break;
				}
				//---------------------------------------------------------------------------
			default:
				break;
		}

		return isValidated;
	}






	public void Restart(){
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
	}
	void OnGUI(){
		//GUI.Label(new Rect(new Vector2(Screen.width - 100, Screen.height - 400), new Vector2(100, 100)), "MinMax搜索深度：3：");

		//GUI.Label(new Rect(new Vector2(Screen.width - 100, Screen.height - 100), new Vector2(100, 100)), "时间：" + Time.time.ToString("f2"));
	}
}