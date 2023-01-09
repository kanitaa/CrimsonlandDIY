using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveManager
{
    void LoadData(GameData data);
    void SaveData(GameData data);
    void ResetLevelData(GameData data);
    void ResetAllData(GameData data);

}