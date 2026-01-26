using System.Collections.Generic;




public class RoomInfo
{
    /// <summary>
    /// 
    /// </summary>
    public Layout layout { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<int> walls { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Size size { get; set; }

    #region Custom Fields
    /// <summary>
    /// Custom name
    /// </summary>
    public string name;
    #endregion
}

public class BaseItem
{
    public int d { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int z { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int x { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int y { get; set; }
}

public class StairsItem : BaseItem
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public int d { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int z { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int x { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int id { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int y { get; set; }
}

public class FurnitureItem : BaseItem
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public int d { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int z { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int x { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int id { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int y { get; set; }
}

public class InnerWallsItem : BaseItem
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public int d { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int z { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int x { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int id { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int y { get; set; }
}

public class FloorItem : BaseItem
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public int z { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int x { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int id { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int y { get; set; }
}

public class DoorsItem : BaseItem
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public int d { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int z { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int x { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int id { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int y { get; set; }
}

public class OuterWallsItem : BaseItem
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public int z { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int x { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int id { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int y { get; set; }
}

public class CeilingItem : BaseItem
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public int z { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int x { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int id { get; set; }
    ///// <summary>
    ///// 
    ///// </summary>
    //public int y { get; set; }
}

public class Layout
{
    /// <summary>
    /// 
    /// </summary>
    public List<StairsItem> stairs { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<FurnitureItem> furniture { get; set; }
    /// <summary>
    /// 
    /// </summary>
    //public List<string> bonus { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<InnerWallsItem> innerWalls { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<FloorItem> floor { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<DoorsItem> doors { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<OuterWallsItem> outerWalls { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<CeilingItem> ceiling { get; set; }
}

public class Size
{
    /// <summary>
    /// 
    /// </summary>
    public int z { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int x { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int y { get; set; }
}

