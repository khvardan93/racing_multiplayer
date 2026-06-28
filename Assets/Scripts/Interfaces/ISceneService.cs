using System.Collections.Generic;
using UnityEngine;

public interface ISceneService
{
    IReadOnlyList<Transform> SpawnPoints { get; }
    void SetCameraTarget(Transform target);
}
