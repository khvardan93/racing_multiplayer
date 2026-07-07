using System.Collections.Generic;
using System;
using UnityEngine;

public interface ISceneService
{
    public event Action OnRivalSpawned;
    IReadOnlyList<Transform> SpawnPoints { get; }
    void SetCameraTarget(Transform target);
    void SetRivalCameraTarget(Transform target);
}
