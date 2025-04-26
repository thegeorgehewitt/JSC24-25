using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistent
{
    void LoadData(PersistentData data);

    void SaveData(PersistentData data);

    void GenerateGuid();
}
