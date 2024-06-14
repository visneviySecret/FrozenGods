using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    int itemSpace = 10;
    int itemCountInMap = 5;
    public float laneOffset = 2f;
    int mapSize;
    enum TrackPos { LeftSide = -2, Left = -1, Center = 0, Right = 1, RightSide = 2 };
    public GameObject EnemyPrefab;

    public List<GameObject> maps = new List<GameObject>();
    public List<GameObject> activeMaps = new List<GameObject>();

    static public MapGenerator instance;

    private void Awake() 
    {
        instance = this;
        mapSize = itemCountInMap * itemSpace;
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        foreach (GameObject map in maps) {
            map.SetActive(false);
        }
    }
    void Start()
    {

    }

    void Update()
    {
        if (RoadGenerator.Instance.speed == 0) return;
        foreach (GameObject map in activeMaps) {
            map.transform.position -= new Vector3(0, 0, RoadGenerator.Instance.speed * Time.deltaTime);
        }
        if (activeMaps[0].transform.position.z < -mapSize) {
            RemoveFirstActiveMap();
            AddActiveMap();
        }
    }

    void RemoveFirstActiveMap() 
    {
        activeMaps[0].SetActive(false);
        maps.Add(activeMaps[0]);
        activeMaps.RemoveAt(0);
    }

    public void ResetMaps()
    {
        while (activeMaps.Count > 0) {
            RemoveFirstActiveMap();
        }
        AddActiveMap();
        AddActiveMap();
    }

    void AddActiveMap() 
    {
        int r = Random.Range(0, maps.Count);
        GameObject go = maps[r];
        go.SetActive(true);
        foreach(Transform child in go.transform) {
            child.gameObject.SetActive(true);
        }
        go.transform.position = activeMaps.Count > 0 ?
                                activeMaps[activeMaps.Count - 1].transform.position + Vector3.forward * mapSize :
                                new Vector3(0, 0, 10);
        maps.RemoveAt(r);
        activeMaps.Add(go);
    }

    GameObject MakeMap1()
    {
        GameObject result = new GameObject("Map1");
        result.transform.SetParent(transform);
        for (int i = 0; i < itemCountInMap; i++) {
            GameObject obstacle = null;
            TrackPos trackPos = TrackPos.Center;
            if (i == 2) { trackPos = TrackPos.Left; obstacle = EnemyPrefab;}
            if (i == 3) { trackPos = TrackPos.LeftSide; obstacle = EnemyPrefab;}
            if (i == 4) { trackPos = TrackPos.Center; obstacle = EnemyPrefab;}
            if (i == 5) { trackPos = TrackPos.RightSide; obstacle = EnemyPrefab;}

            Vector3 obstaclePos = new Vector3((int)trackPos * laneOffset, 0, i * itemSpace);
            if (obstacle != null) {
                GameObject go = Instantiate(obstacle, obstaclePos, Quaternion.identity);
                go.transform.SetParent(result.transform);
            }
        }
        return result;
    }
}
