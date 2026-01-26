# 保存RenderTexture  

```C#
一、
if (Input.GetKey(KeyCode.R))
{
   Debug.Log("Start");
   RenderTexture active = RenderTexture.active;
   RenderTexture.active = m_ReflectionTexture;
   Texture2D png = new Texture2D(m_ReflectionTexturewidth, m_ReflectionTexture.height, TextureFormatARGB32, false);
   png.ReadPixels(new Rect(0, 0, m_ReflectionTexturewidth, m_ReflectionTexture.height), 0, 0);//从屏读取像素保存到纹理数据中。
   png.Apply();
   RenderTexture.active = active;
   byte[] bytes = png.EncodeToPNG();
   string path = string.Format(System.EnvironmentGetFolderPath(Environment.SpecialFolder.Desktop)+ "/rt_{0}_{1}_{2}.png", DateTime.Now.Hour,DateTime.Now.Minute, DateTime.Now.Second);
   System.IO.File.WriteAllBytes(path, bytes);
   Debug.Log("Success");
   png = null;
}


//二、
private IEnumerator CaptureByCamera(Camera mCamera, Vector2 size, string mFileName)
{
    yield return new WaitForEndOfFrame();
    RenderTexture mRender = new RenderTexture((int)size.x, (int)size.y, 16);
    mCamera.targetTexture = mRender;
    mCamera.Render();
    RenderTexture.active = mRender;
    Texture2D mTexture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGB24, false);
    mTexture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
    mTexture.Apply();
    mCamera.targetTexture = null;
    RenderTexture.active = null;
    GameObject.Destroy(mRender);
    this.textureCap = mTexture;
    //byte[] bytes = mTexture.EncodeToPNG();
    //System.IO.File.WriteAllBytes(mFileName, bytes);
}

// //API：Texture2D.ReadPixel()
// 1.读取屏幕像素信息并存储为纹理数据.
// 2.这将从当前处于激活状态的 RenderTexture 或视图（由/source/指定）复制一个由destX和destY指定的矩形像素区域。这两个坐标使用像素空间坐标 (0，0)是屏幕左下角。

// //API：Texture2D.Apply()
// 实际应用任何先前的 SetPixel 和 SetPixels 更改。

```