using System;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.WorldPositioning;
using Niantic.Lightship.AR.XRSubsystems;
using System.IO;
using Random = UnityEngine.Random;
using UnityEngine.Events;

[System.Serializable]
public class SerializedObjectData
{
    public double Latitude;
    public double Longitude;
    public int PrefabIndex;
}

[System.Serializable]
public class SerializedObjectList
{
    public List<SerializedObjectData> Objects = new List<SerializedObjectData>();
}

public class AddWPSObjects : MonoBehaviour
{
    [SerializeField] ARWorldPositioningObjectHelper PositioningObjectHelper;
    [SerializeField] private ARWorldPositioningManager PositioningManager;
    [SerializeField] private List<GameObject> prefabs;

    private ARWorldPositioningCameraHelper PositioningCameraHelper;

    private string saveFilePath;

    private SerializedObjectList spawnedObjects;
    public List<GameObject> InstantiatedObjects = new();

    private bool _isInitialized = false;

    private WorldPositioningStatus _wpsStatus = WorldPositioningStatus.SubsystemNotRunning;

    public UnityAction OnWPSInitialized;

    void Start()
    {
        PositioningCameraHelper = FindObjectOfType<ARWorldPositioningCameraHelper>();
        PositioningManager.OnStatusChanged += OnStatusChanged;

        saveFilePath = Path.Combine(Application.persistentDataPath, "db.json");
        LoadSpawnedObjects();
    }

    private void OnStatusChanged(WorldPositioningStatus newVal)
    {
        _wpsStatus = newVal;

        if (!_isInitialized && newVal == WorldPositioningStatus.Available)
        {
            RespawnSavedObjects();
            _isInitialized = true;
            OnWPSInitialized.Invoke();
        }
    }

    public void SpawnObjectAtPosition()
    {
        double latitude = PositioningCameraHelper.Latitude;
        double longitude = PositioningCameraHelper.Longitude;

        if (prefabs.Count == 0)
        {
            return;
        }

        int prefabIndex = Random.Range(0, prefabs.Count);

        SpawnObject(latitude, longitude, prefabIndex);

        SaveSpawnedObject(latitude, longitude, prefabIndex);
    }

    private void SpawnObject(double latitude, double longitude, int prefabIndex)
    {
        double altitude = 0.0;

        GameObject newAnchor = Instantiate(
            prefabs[prefabIndex],
            Vector3.zero,
            Quaternion.identity
        );
        InstantiatedObjects.Add(newAnchor);

        PositioningObjectHelper.AddOrUpdateObject(newAnchor, latitude, longitude, altitude, Quaternion.identity);
    }

    private void SaveSpawnedObject(double latitude, double longitude, int prefabIndex)
    {
        SerializedObjectData newData = new SerializedObjectData
        {
            Latitude = latitude,
            Longitude = longitude,
            PrefabIndex = prefabIndex,
        };

        spawnedObjects.Objects.Add(newData);
        SaveSpawnedObjectsToFile();
    }

    private void SaveSpawnedObjectsToFile()
    {
        string json = JsonUtility.ToJson(spawnedObjects);
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadSpawnedObjects()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            spawnedObjects = JsonUtility.FromJson<SerializedObjectList>(json);
        }
        else
        {
            spawnedObjects = new SerializedObjectList();
        }
    }

    private void RespawnSavedObjects()
    {
        foreach (var data in spawnedObjects.Objects)
        {
            if (data.PrefabIndex >= 0 && data.PrefabIndex < prefabs.Count)
            {
                SpawnObject(data.Latitude, data.Longitude, data.PrefabIndex);
            }
        }
    }

    public void EraseAllSerializedObjects()
    {
        spawnedObjects.Objects.Clear();
        SaveSpawnedObjectsToFile();
    }

    public void EraseAllInstantiatedObjects()
    {
        PositioningObjectHelper.RemoveAllObjects();
        foreach (var obj in InstantiatedObjects)
        {
            Destroy(obj);
        }
        InstantiatedObjects.Clear();
    }
}
