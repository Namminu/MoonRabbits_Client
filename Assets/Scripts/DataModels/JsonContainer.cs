using System;
using System.Collections.Generic;

[Serializable]
public class JsonContainer<T>
{
  public string name;
  public string version;
  public List<T> data;
}
