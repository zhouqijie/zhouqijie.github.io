using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FictionBoard
{

	private Piece[, ] arr;
	public FictionBoard ()
	{
		arr = new Piece[9, 10];
	}
	public Piece this [int ix, int iy]
	{
		get { return arr[ix - 1, iy - 1]; }
		set { arr[ix - 1, iy - 1] = value; }
	}
	public static FictionBoard LoadFromScene ()
	{
		FictionBoard boardFromScene = new FictionBoard ();
		foreach (Chunk chunk in GameObject.FindObjectsOfType<Chunk> ())
		{
			if (chunk.transform.childCount > 0)
			{
				boardFromScene[chunk.Position.x, chunk.Position.y] = chunk.GetComponentInChildren<Piece> ();
			}
			else
			{
				boardFromScene[chunk.Position.x, chunk.Position.y] = null;
			}
		}
		return boardFromScene;
	}

	public float EvaluateValue ()
	{
		float value = 0f;
		for (int x = 1; x <= 9; x++)
		{
			for (int y = 1; y <= 10; y++)
			{
				if (this [x, y] != null)
				{
					if (!this [x, y].IsPlayer)
					{
						value += this [x, y].value;
					}
					else
					{
						value -= this [x, y].value;
					}
				}
			}
		}
		return value;
	}


	public FictionBoard Clone(){
		FictionBoard newBoard = new FictionBoard();
		for(int x = 1; x <= 9; x ++){
			for(int y = 1; y <= 10; y++){
				newBoard[x, y] = this[x, y];
			}
		}

		return newBoard;
	}
}