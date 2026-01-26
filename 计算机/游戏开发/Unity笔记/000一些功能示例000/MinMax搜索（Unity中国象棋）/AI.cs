using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{

	private Judge judge;

	private bool isCalculating;

	// Use this for initialization
	void Awake ()
	{
		isCalculating = false;
		judge = GameObject.Find ("Judge").GetComponent<Judge> ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (judge.IsAiTurn && !isCalculating)
		{
			isCalculating = true;
			StartCoroutine (Calulate ());
		}
	}

	IEnumerator Calulate ()
	{
		yield return new WaitForSeconds (0.1f);
		//Start

		//产生根节点
		FictionBoard currentBoard = FictionBoard.LoadFromScene ();
		FictionBoardNode root = new FictionBoardNode (currentBoard);

		//生成子节点
		root.GenerateChildBoards (false);
		foreach (FictionBoardNode childD1 in root.childs)
		{
			childD1.GenerateChildBoards (true);
			if (childD1.childs != null)
			{
				foreach (FictionBoardNode childD2 in childD1.childs)
				{
					childD2.GenerateChildBoards (false);
				}
			}

		}

		//树节点评价和计算
		float maxValue = -9999f; //maxValue( 当前节点待定最大值)
		FictionBoardNode targetNode = null;
		List<FictionBoardNode> targetNodesBackup = new List<FictionBoardNode> ();
		if (root.childs != null)
		{
			foreach (FictionBoardNode nodeD1 in root.childs)
			{

				float valueD1 = 9999f; //minValue（ 第一层级当前遍历节点的待定最小值 ）

				if (nodeD1.childs != null) //???第一级有子节点
				{
					foreach (FictionBoardNode nodeD2 in nodeD1.childs)
					{

						float valueD2 = -9999f; //maxValue（ 第二层级当前遍历节点的待定最大值 ）

						if (nodeD2.childs != null) //???第二级有子节点
						{
							foreach (FictionBoardNode nodeD3 in nodeD2.childs)
							{
								float valueD3 = -9999f; //minValue（ 第三层级当前遍历节点的待定最小值 ）

								valueD3 = nodeD3.value;//------------------------------------------------------------------------------

								if (valueD3 > valueD2)
								{
									valueD2 = valueD3;
								}
							}
						}
						else //???第二级没有子节点
						{
							valueD2 = nodeD2.value;
						}

						if (valueD2 < valueD1)
						{
							valueD1 = valueD2;
						}
					}
				}
				else //???第一级没有子节点
				{
					valueD1 = nodeD1.value; //???
				}

				if (valueD1 > maxValue)
				{
					maxValue = valueD1;
					targetNodesBackup.Clear ();
					targetNodesBackup.Add (nodeD1);
				}
				else if (valueD1 == maxValue)
				{
					targetNodesBackup.Add (nodeD1);
				}
			}
		}
		int index = UnityEngine.Random.Range (0, targetNodesBackup.Count - 1);
		targetNode = targetNodesBackup[index];
		print("最优选择数量" + targetNodesBackup.Count);
		yield return null;

		float[] changes = targetNode.GetChanges ();

		//行动
		Chunk[] chunks = GameObject.FindObjectsOfType<Chunk> ();
		Chunk targetChunk = null;
		foreach (Chunk chunk in chunks)
		{
			if (chunk.Position.x == changes[2] && chunk.Position.y == changes[3])
			{
				targetChunk = chunk;
			}
		}
		foreach (Chunk chunk in chunks)
		{
			if (chunk.Position.x == changes[0] && chunk.Position.y == changes[1])
			{
				chunk.GetComponentInChildren<Piece> ().MoveTo (targetChunk);
			}
		}

		yield return new WaitForEndOfFrame (); //等待生命周期末的Destroy
		//End
		judge.AiFinish ();
		isCalculating = false;
	}

	public static bool ValidateFiction (FictionBoard board, int x, int y, int newx, int newy)
	{

		bool isValidated = false;

		//ZeroMovement
		if (newx == x && newy == y)
		{
			return false;
		}

		//Occupied?  //Attacking?
		bool isEating = false;
		if (board[newx, newy] != null)
		{
			if (board[newx, newy].IsPlayer == board[x, y].IsPlayer)
			{
				return false;
			}
			else if (board[newx, newy].IsPlayer != board[x, y].IsPlayer)
			{
				isEating = true;
			}
		}
		else
		{
			isEating = false;
		}

		//MoveNum
		int moveX = newx - x;
		int moveY = newy - y;
		//Judge By PieceType (Switch Case)
		// print(board[x, y]);print("x: " + x + " y: " + y + " newx: " + newx + " newy:" + newy);
		switch (board[x, y].PieceType)
		{
			//---------------------------------------------------------------------------
			case PieceType.JIANG:
				{
					bool isRightPos = Math.Abs (moveX) + Math.Abs (moveY) == 1;
					bool isInGridX = newx <= 6 && newx >= 4;
					bool isInGridY = board[x, y].IsPlayer ? newy <= 3 : newy >= 8;
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
					bool isInGridX = newx <= 6 && newx >= 4;
					bool isInGridY = board[x, y].IsPlayer ? newy <= 3 : newy >= 8;
					if (isInGridX && isInGridY && isRightPos)
					{
						isValidated = true;
					}
					break;
				}

				//---------------------------------------------------------------------------
			case PieceType.XIANG:
				{
					bool isToSelfSide = board[x, y].IsPlayer ? newy <= 5 : newy >= 6;
					bool isRightPos = Math.Abs (moveX) == 2 && Math.Abs (moveY) == 2;

					if (isRightPos && isToSelfSide)
					{
						bool isNotBlocked = board[(newx + x) / 2, (newy + y) / 2] == null;
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
						bool isNotBlock = board[x + Math.Sign (moveX), y] == null;
						if (isNotBlock)
						{
							isValidated = true;
						}
					}
					if (isRightPos12)
					{
						bool isNotBlocked = board[x, y + Math.Sign (moveY)] == null;
						if (isNotBlocked)
						{
							isValidated = true;
						}
					}
					break;
				}
			case PieceType.JU:
				{
					bool isInLineXn = x == newx;
					bool isInLineYn = y == newy;
					if (isInLineXn)
					{
						int biggerY = Math.Max (newy, y);
						int smallerY = Math.Min (newy, y);
						bool isNotBlocked = true;
						for (int i = smallerY + 1; i < biggerY; i++)
						{
							if (board[x, i] != null) isNotBlocked = false;
						}
						if (isNotBlocked)
						{
							isValidated = true;
						}
					}
					if (isInLineYn)
					{
						int biggerX = Math.Max (newx, x);
						int smallerX = Math.Min (newx, x);
						bool isNotBlocked = true;
						for (int i = smallerX + 1; i < biggerX; i++)
						{
							if (board[i, y] != null) isNotBlocked = false;
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
					bool isInLineXn = x == newx;
					bool isInLineYn = y == newy;
					if (isInLineXn)
					{
						int biggerY = Math.Max (newy, y);
						int smallerY = Math.Min (newy, y);
						int numBlocks = 0;
						for (int i = smallerY + 1; i < biggerY; i++)
						{
							if (board[x, i] != null) numBlocks++;
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
						int biggerX = Math.Max (newx, x);
						int smallerX = Math.Min (newx, x);
						int numBlocks = 0;
						for (int i = smallerX + 1; i < biggerX; i++)
						{
							if (board[i, y] != null) numBlocks++;
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
					bool isInSelfSide = board[x, y].IsPlayer ? y <= 5 : y >= 6;

					if (isInSelfSide)
					{
						bool isRightPos = (moveX == 0 && moveY == (board[x, y].IsPlayer ? 1 : -1));
						if (isRightPos)
						{
							isValidated = true;
						}
					}
					else
					{
						bool isRightPos = Math.Abs (moveX) + Math.Abs (moveY) == 1 && (board[x, y].IsPlayer ? moveY >= 0 : moveY <= 0);
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

}