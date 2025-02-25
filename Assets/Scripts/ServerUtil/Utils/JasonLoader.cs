using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JsonFileLoader
{
  private readonly JsonSerializerSettings settings = new()
  {
    Formatting = Formatting.Indented,
    NullValueHandling = NullValueHandling.Ignore,
    MissingMemberHandling = MissingMemberHandling.Ignore
  };

  public static string[] SearchAllJsonFiles(string dirPath)
  {
    if (!Directory.Exists(dirPath))
    {
      throw new DirectoryNotFoundException($"디렉토리를 찾을 수 없습니다: {dirPath}");
    }
    return Directory.GetFiles(dirPath, "*.json");
  }

  public T ReadJsonFile<T>(string filePath)
  {
    if (!File.Exists(filePath))
    {
      throw new FileNotFoundException($"파일을 찾을 수 없습니다: {filePath}");
    }

    try
    {
      string text = File.ReadAllText(filePath);
      T ret = JsonConvert.DeserializeObject<T>(text, settings);
      if (ret == null)
      {
        throw new Exception($"JSON 디시리얼라이즈 실패: {filePath}");
      }
      return ret;
    }
    catch (Exception ex)
    {
      throw new Exception($"JSON 파일 로드 중 오류 발생: {filePath} - {ex.Message}");
    }
  }

  public List<T> ReadAllJsonFiles<T>(string dirPath)
  {
    string[] filePaths = SearchAllJsonFiles(dirPath);
    List<T> dataList = new();

    foreach (var filePath in filePaths)
    {
      dataList.Add(ReadJsonFile<T>(filePath));
    }

    return dataList;
  }
}
