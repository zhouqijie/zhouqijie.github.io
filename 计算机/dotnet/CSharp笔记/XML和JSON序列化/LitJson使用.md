# 一、LitJson

## JSON对象访问示例：
```CSharp
string json = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\blocks.json");
Debug.Log("Json Length: " + json.Length);
JsonData jData = JsonMapper.ToObject(json);


JsonData prefabs = jData["prefabs"];

foreach (string key in prefabs.Keys)
{
    Debug.Log("Key:" + key);
    if (prefabs[key].ContainsKey("symmetry"))
    {
        Debug.Log("  symmetry: " + prefabs[key]["symmetry"].ToString());
    }
}
```

## JSON数组访问示例：
```CSharp
string json = @"{
'materials':[
{'materialName': 'mat1'},
{'materialName': 'mat2'}
]

}";

JsonData jData = JsonMapper.ToObject(json);

JsonData jMats = jData["materials"];

foreach (JsonData item in jMats)
{
    Debug.Log("MatName: " + item["materialName"]);
}
```

# 二、JsonReader和JsonWritter  

## Reader示例：  
```CSharp
using LitJson;
using System;

public class DataReader
{
    public static void Main()
    {
        string sample = @"{
            ""name""  : ""Bill"",
            ""age""   : 32,
            ""awake"" : true,
            ""n""     : 1994.0226,
            ""note""  : [ ""life"", ""is"", ""but"", ""a"", ""dream"" ]
          }";

        PrintJson(sample);
    }

    public static void PrintJson(string json)
    {
        JsonReader reader = new JsonReader(json);

        Console.WriteLine ("{0,14} {1,10} {2,16}", "Token", "Value", "Type");
        Console.WriteLine (new String ('-', 42));

        // The Read() method returns false when there's nothing else to read
        while (reader.Read()) {
            string type = reader.Value != null ?
                reader.Value.GetType().ToString() : "";

            Console.WriteLine("{0,14} {1,10} {2,16}",
                              reader.Token, reader.Value, type);
        }
    }
}
```
输出:
```
Token      Value             Type
------------------------------------------
   ObjectStart                            
  PropertyName       name    System.String
        String       Bill    System.String
  PropertyName        age    System.String
           Int         32     System.Int32
  PropertyName      awake    System.String
       Boolean       True   System.Boolean
  PropertyName          n    System.String
        Double  1994.0226    System.Double
  PropertyName       note    System.String
    ArrayStart                            
        String       life    System.String
        String         is    System.String
        String        but    System.String
        String          a    System.String
        String      dream    System.String
      ArrayEnd                            
     ObjectEnd                     
```

## Writter示例：  

```CSharp
using LitJson;
using System;
using System.Text;

public class DataWriter
{
    public static void Main()
    {
        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);

        writer.WriteArrayStart();
        writer.Write(1);
        writer.Write(2);
        writer.Write(3);

        writer.WriteObjectStart();
        writer.WritePropertyName("color");
        writer.Write("blue");
        writer.WriteObjectEnd();

        writer.WriteArrayEnd();

        Console.WriteLine(sb.ToString());
    }
}
```
输出：
```
[1,2,3,{"color":"blue"}]
```
