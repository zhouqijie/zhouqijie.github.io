# 一、Unity内置方式（PropertyBlock）

- 在脚本中使用`MeshRenderer.SetPropertyBlock(MaterialPropertyBlock props)`方法。

- 在shader代码中使用宏`UNITY_INSTANCING_BUFFER_START(Props)/UNITY_DEFINE_INSTANCED_PROP(float4, _Color)/UNITY_INSTANCING_BUFFER_END(Props)`。

- 在a2v中定义宏`UNITY_VERTEX_INPUT_INSTANCE_ID`，并在顶点着色器中使用`UNITY_SETUP_INSTANCE_ID(v);`。

- 使用`UNITY_ACCESS_INSTANCED_PROP(Props, _Color);`获取Instance相关属性。


（注：如果要在片段着色器中使用Instance相关属性，需要在v2f中也定义UNITY_VERTEX_INPUT_INSTANCE_ID，并且在顶点着色器中使用宏UNITY_TRANSFER_INSTANCE_ID(v, o)传递数据。）
（缺陷：需要很多GameObject。）

>docs:https://docs.unity3d.com/Manual/GPUInstancing.html

CS代码：
```CSharp
	MaterialPropertyBlock props = new MaterialPropertyBlock();
	MeshRenderer renderer;

	for (int i = 0; i < 10; i++)
	{
		float r = Random.Range(0.0f, 1.0f);
		float g = Random.Range(0.0f, 1.0f);
		float b = Random.Range(0.0f, 1.0f);
		props.SetColor("_Color", new Color(r, g, b));

		renderer = obj.GetComponent<MeshRenderer>();
		renderer.SetPropertyBlock(props);

	}
```


# 二、使用Graphics.DrawMeshInstance接口  

## 关于变换矩阵传入：

- 定义`#pragma multi_compile_instancing`。
- 在a2v结构体中定义UNITY_VERTEX_INPUT_INSTANCE_ID。
- 在DrawMeshInstance函数参数中传入变换矩阵的数组matrices，在顶点着色器中使用宏`UNITY_SETUP_INSTANCE_ID(v);`来自动变换坐标。

## 关于instanceID:SV_InstanceID参数：

- 着色器参数中定义uint instanceID : SV_InstanceID参数。
- 不会自动变换坐标，需要使用positionBuffer。

（注：Unity表面着色器的顶点着色器不能定义instanceID:SV_instanceID。改为在a2v中定义宏UNITY_VERTEX_INPUT_INSTANCE_ID即可）



CS代码：
```CSharp
	Matrix4x4[] matrices = new Matrix4x4[400];
	for (int i = 0; i < 400; i++)
	{
		var position = new Vector3(player.position.x - 100f, 0, player.position.z - 100f) + new Vector3((i / 20) * 10f, 0, (i % 20) * 10f);
	var rotation = Quaternion.Euler(0, 0, 0);
	var scale = Vector3.one; ;
	matrices[i] = Matrix4x4.TRS(position, rotation, scale);
	}

	Graphics.DrawMeshInstanced(mesh, 0, mat, matrices);
```



# 三、使用Graphics.DrawMeshInstancedIndirect接口和ComputeBuffer  


## 示例：
```CSharp
private int instanceCount = 10000;
private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
private ComputeBuffer positionBuffer;
private ComputeBuffer argsBuffer;

void Start()
{
	argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
	UpdateBuffers(FindObjectOfType<Rigidbody>().transform.position);
}
void Update()
{
	Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, new Bounds(Vector3.zero, new Vector3(1000.0f, 1000.0f, 1000.0f)), argsBuffer);
}
void UpdateBuffers(Vector3 playerPos)
{
	if (instanceCount < 1) instanceCount = 1;

	if (positionBuffer != null) positionBuffer.Release();

	positionBuffer = new ComputeBuffer(instanceCount, 16);


	Vector4[] positions = new Vector4[instanceCount];

	for (int i = 0; i < instanceCount; i++)
	{
		positions[i] = new Vector4(playerPos.x - 500, 0, playerPos.z - 500, 1f) + new Vector4((i / 100) * 10f, 0f, (i % 100) * 10f, 1f);
	}

	positionBuffer.SetData(positions);

	mat.SetBuffer("positionBuffer", positionBuffer);

	// indirect args
	uint numIndices = (mesh != null) ? (uint)mesh.GetIndexCount(0) : 0;
	args[0] = numIndices;
	args[1] = (uint)instanceCount;
	argsBuffer.SetData(args);
}

void OnDisable()
{
	if (positionBuffer != null) positionBuffer.Release();
	positionBuffer = null;
	if (argsBuffer != null) argsBuffer.Release();
	argsBuffer = null;
}
```
Shader:
```
//...
StructuredBuffer<float4> positionBuffer;
//...

v2f vert(appdata v, uint instanceID : SV_InstanceID)
{
	//...
	float3 worldPosition = positionBuffer[instanceID].xyz + v.vertex;    //no rotation/scale
}
//...
```




