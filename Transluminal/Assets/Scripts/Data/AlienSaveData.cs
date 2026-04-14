using UnityEngine;

public class AlienSaveData
{
    public GameObject alienObject { get; private set; }
    public string currentFloor { get; private set; }
    public float remainingLifeTime{ get; private set; }

    public AlienSaveData(GameObject alienObject,  string currentFloor,  float remainingLifeTime)
    {
        this.alienObject = alienObject;
        this.currentFloor = currentFloor;
        this.remainingLifeTime = remainingLifeTime;
    }
}
