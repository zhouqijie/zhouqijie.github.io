using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FictionBoardNode
{
	public FictionBoardNode parent;
	public List<FictionBoardNode> childs;

	public FictionBoard board;
	public float value;

	//构造函数
	public FictionBoardNode (FictionBoard newboard)
	{
		this.board = newboard;
		this.parent = null;
		this.childs = null;
	}

	//生成子节点
	public void GenerateChildBoards (bool isPlayer)
	{	
		if(this.parent == null) this.value = this.board.EvaluateValue();

		if (this.value < 500f && this.value > -500f)
		{
			List<FictionBoardNode> childBoardNodes = new List<FictionBoardNode> ();
			for (int x = 1; x <= 9; x++)
			{
				for (int y = 1; y <= 10; y++)
				{
					if (this.board[x, y] != null)
					{
						if (this.board[x, y].IsPlayer == isPlayer)
						{
							for (int newx = 1; newx <= 9; newx++)
							{
								for (int newy = 1; newy <= 10; newy++)
								{
									if (AI.ValidateFiction (this.board, x, y, newx, newy))
									{
										FictionBoardNode childBoardNode = new FictionBoardNode (this.board.Clone ());

										childBoardNode.board[newx, newy] = this.board[x, y];
										childBoardNode.board[x, y] = null;
										childBoardNode.parent = this;
										childBoardNode.value = childBoardNode.board.EvaluateValue ();

										childBoardNodes.Add (childBoardNode);
									}
								}
							}
						}
					}
				}
			}
			this.childs = childBoardNodes;
		}
		
	}

	//获取变化return: {x, y, newx, newy}
	public float[] GetChanges ()
	{
		float[] changes = new float[4];
		for (int x = 1; x <= 9; x++)
		{
			for (int y = 1; y <= 10; y++)
			{

				if (this.board[x, y] != this.parent.board[x, y])
				{
					if (this.board[x, y] == null)
					{
						changes[0] = x;
						changes[1] = y;
					}
					else
					{
						changes[2] = x;
						changes[3] = y;
					}
				}

			}
		}
		return changes;
	}
}