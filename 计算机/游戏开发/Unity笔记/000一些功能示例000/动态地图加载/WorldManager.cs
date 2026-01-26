using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// //物体存储
// public class RestoredObject{
// 	private GameObject obj;
// 	private Vector2 worldCoords;
// 	private Vector3 accuratePos;
// }

public class WorldManager : MonoBehaviour {

	[SerializeField]
	private GameObject player; //玩家

	[SerializeField]
	private float unitLon; //每块单位经度
	[SerializeField]
	private float unitLat; //每块单位纬度

	[SerializeField]
	private float chunkWidth; //每块宽度(m)

	[SerializeField]
	private float chunkHeight; //每块高度(m)

	[SerializeField]
	private int range; //可视范围

	private Chunk currentChunk; //当前块

	private List<Chunk> activeChunkList; //已存在的块

	//public List<GameObject> restoredGameObjects; //存储物体

	public Chunk CurrentChunk { get { return currentChunk; } }

	// Use this for initialization
	void Start () {
		activeChunkList = new List<Chunk> ();
		Chunk[] chunks = Object.FindObjectsOfType<Chunk> ();
		foreach (Chunk chunk in chunks) {
			activeChunkList.Add (chunk);
		}

		currentChunk = ClosestChunk (player.transform.position);

		//InvokeRepeating("CheckScenery", 1f, 1f);
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.O)) {
			CoordsReturnToZero ();
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			CheckScenery ();
		}
	}

	//------------------------------------------------------------------------------------------------------

	//坐标归零
	void CoordsReturnToZero () {
		//归零坐标
		GameObject[] objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().GetRootGameObjects ();
		Vector3 playerPos = player.transform.position;
		foreach (GameObject obj in objs) {
			obj.transform.Translate (-playerPos, Space.World);
		}
	}

	void CheckScenery () {

		//当前地形块
		currentChunk = ClosestChunk (player.transform.position);

		//删除多余地形块
		for (int i = activeChunkList.Count - 1; i >= 0; i--) {
			float x = Mathf.Abs (activeChunkList[i].transform.position.x - currentChunk.transform.position.x);
			float z = Mathf.Abs (activeChunkList[i].transform.position.z - currentChunk.transform.position.z);
			//距离冗余0.1
			float rangef = (float)range + 0.1f;
			if (x > rangef * chunkWidth || z > rangef * chunkHeight) {
				Destroy (activeChunkList[i].gameObject, 0.1f);
				activeChunkList.Remove (activeChunkList[i]);
			}
		}

		//动态加载地形块
		for (int i = -range; i < range + 1; i++) {
			for (int j = -range; j < range + 1; j++) {

				float expectPosX = currentChunk.transform.position.x + (i * chunkWidth);
				float expectPosY = currentChunk.transform.position.y;
				float expectPosZ = currentChunk.transform.position.z + (j * chunkHeight);

				List<Chunk> expectChunk = activeChunkList.FindAll (c => Mathf.Abs (c.transform.position.x - expectPosX) < 1f)
					.FindAll (c => Mathf.Abs (c.transform.position.z - expectPosZ) < 1f);

				if (expectChunk.Count == 0) {
					StartCoroutine (CoroutineLoadScenery (currentChunk.longtitude + unitLon * i, currentChunk.lattitude + unitLat * j, new Vector3 (expectPosX, expectPosY, expectPosZ)));
				}
			}
		}

		//设置Neighbors
		StartCoroutine(SetSceneryNeighbors());
	}

	IEnumerator CoroutineLoadScenery (float lon, float lat, Vector3 position) {
		GameObject scenery = null;
		float fmtLon = lon;
		float fmtLat = lat;
		var request = Resources.LoadAsync<GameObject> (string.Format ("Scenery/scenery_{0}_{1}", fmtLon, fmtLat));
		yield return request;

		if (!(request.asset != null)) {
			request = Resources.LoadAsync<GameObject> ("Scenery/scenery_empty");
			yield return request;
		}

		scenery = request.asset as GameObject;
		scenery = Instantiate (scenery, position, new Quaternion ());

		Chunk chunk = scenery.GetComponent<Chunk> ();
		chunk.longtitude = lon;
		chunk.lattitude = lat;
		activeChunkList.Add (chunk);

	}

	//设置Neighbors
	IEnumerator SetSceneryNeighbors () {

		yield return new WaitUntil (() => activeChunkList.Count == 9);

		foreach (Chunk chunk in activeChunkList) {
			//当前块经纬
			float lon = chunk.GetComponent<Chunk> ().longtitude;
			float lat = chunk.GetComponent<Chunk> ().lattitude;
			//查找邻边块
			Chunk chunkLeft = activeChunkList.Find (x => Mathf.Abs(x.longtitude - (lon - unitLon)) < 0.001f && Mathf.Abs(x.lattitude - lat) < 0.001f);
			Chunk chunkRight = activeChunkList.Find (x => Mathf.Abs(x.longtitude - (lon + unitLon)) < 0.001f && Mathf.Abs(x.lattitude - lat) < 0.001f);
			Chunk chunkUp = activeChunkList.Find (x => Mathf.Abs(x.longtitude - lon) < 0.001f && Mathf.Abs(x.lattitude - (lat + unitLat)) < 0.001f);
			Chunk chunkDown = activeChunkList.Find (x => Mathf.Abs(x.longtitude - lon) < 0.001f && Mathf.Abs(x.lattitude - (lat - unitLat)) < 0.001f);

			// int neighborNum = 0;
			// if(chunkLeft != null) neighborNum ++;
			// if(chunkRight != null) neighborNum ++;
			// if(chunkUp != null) neighborNum ++;
			// if(chunkDown != null) neighborNum ++;
			// print("边界数：" + neighborNum);

			//查找邻边块中的地形
			Terrain terrainThis = chunk.GetComponentInChildren<Terrain>();
			Terrain terrainLeft = chunkLeft != null ? chunkLeft.GetComponentInChildren<Terrain> () : new Terrain ();
			Terrain terrainUp = chunkUp != null ? chunkUp.GetComponentInChildren<Terrain> () : new Terrain ();
			Terrain terrainRight = chunkRight != null ? chunkRight.GetComponentInChildren<Terrain> () : new Terrain ();
			Terrain terrainDown = chunkDown != null ? chunkDown.GetComponentInChildren<Terrain> () : new Terrain ();

			//地形接缝连接
			// int heightMapWidth = terrainThis.terrainData.heightmapWidth;
			// int heightMapHeight = terrainThis.terrainData.heightmapHeight;
			// if(chunkLeft != null){
			// 	float[,] heights = chunk.GetComponentInChildren<Terrain>().terrainData.GetHeights(0, 0, 1, heightMapHeight);
			// 	float[,] heightsL = chunkLeft.GetComponentInChildren<Terrain>().terrainData.GetHeights(heightMapWidth - 1, 0, 1, heightMapHeight);
			// 	float[,] heightsAverage = new float[heightMapHeight, 1];
			// 	for(int i = 0; i < heightMapHeight; i ++){
			// 		heightsAverage[i, 0] = (heights[i, 0] + heightsL[i, 0]) / 2f;
			// 	}
			// 	terrainLeft.terrainData.SetHeights(heightMapWidth - 1, 0, heightsAverage);
			// 	terrainThis.terrainData.SetHeights(0, 0, heightsAverage);
			// }
			// if(chunkUp != null){
			// 	float[,] heights = chunk.GetComponentInChildren<Terrain>().terrainData.GetHeights(0, 0, heightMapWidth, 1);
			// 	float[,] heightsU = chunkUp.GetComponentInChildren<Terrain>().terrainData.GetHeights(0, heightMapHeight - 1, heightMapWidth, 1);
			// 	float[,] heightsAverage = new float[1, heightMapWidth];
			// 	for(int i = 0; i < heightMapWidth; i ++){
			// 		heightsAverage[0, i] = (heights[0, i] + heightsU[0, i]) / 2f;
			// 	}
			// 	terrainUp.terrainData.SetHeights(0, heightMapHeight - 1, heightsAverage);
			// 	terrainThis.terrainData.SetHeights(0, 0, heightsAverage);
			// }
			
			//设置邻边
			chunk.GetComponentInChildren<Terrain> ().SetNeighbors (terrainLeft,
				terrainUp,
				terrainRight,
				terrainDown
			);
		}

		yield return null;
	}

	private Chunk ClosestChunk (Vector3 pos) {

		Chunk closestChunk = null;
		float minDistance = 9999999f; //使用sqrMagnitude节省性能
		foreach (Chunk chunk in activeChunkList) {
			float distance = (pos - (chunk.transform.position + new Vector3 (chunkWidth * 0.5f, 0f, chunkHeight * 0.5f))).sqrMagnitude;
			if (distance < minDistance) {
				closestChunk = chunk;
				minDistance = distance;
			}
		}
		return closestChunk;
	}
}