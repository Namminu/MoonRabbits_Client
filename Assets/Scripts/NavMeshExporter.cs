using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class SerializableVector3
{
  public float x, y, z;

  public SerializableVector3(Vector3 vector)
  {
    x = vector.x;
    y = vector.y;
    z = vector.z;
  }
}

[System.Serializable]
public class NavMeshDataExport
{
  public List<SerializableVector3> vertices;
  public List<int> indices;
}

public class NavMeshExporter : MonoBehaviour
{
  [ContextMenu("Export NavMesh to JSON")]
  public void ExportNavMeshToJson()
  {
    NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

    if (navMeshData.vertices.Length == 0 || navMeshData.indices.Length == 0)
    {
      Debug.LogWarning("NavMesh 데이터가 없습니다. NavMesh가 제대로 베이크되었는지 확인하세요.");
      return;
    }

    // Vector3 → SerializableVector3로 변환
    List<SerializableVector3> serializedVertices = new List<SerializableVector3>();
    foreach (var vertex in navMeshData.vertices)
    {
      serializedVertices.Add(new SerializableVector3(vertex));
    }

    // 직렬화할 데이터 구조 생성
    NavMeshDataExport exportData = new NavMeshDataExport
    {
      vertices = serializedVertices,
      indices = new List<int>(navMeshData.indices)
    };

    // JSON 직렬화
    string json = JsonUtility.ToJson(exportData, true);

    // 파일로 저장
    string path = Application.dataPath + "/navmesh.json";
    File.WriteAllText(path, json);

    Debug.Log($"NavMesh 데이터가 JSON으로 저장되었습니다: {path}");
  }
}


// 사용 방법
// Unity에서 빈 GameObject를 생성한다.
// NavMeshExporter.cs 스크립트를 GameObject에 추가한다.
// 인스펙터(Inspector)에서 우클릭 → "Export NavMesh to JSON" 클릭.
// Assets/navmesh.json 경로에 JSON 파일이 생성된다.