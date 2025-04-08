using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherit to create a single, global accessible instance of a class, available at all times
public class Singleton_Blank<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if(Instance == null)
        {
            Instance = (T)FindObjectOfType(typeof(T));
        }

        else
        {
            Debug.Log("Duplicate Singleton Object has been found, currently destroy it, " + this.gameObject.name);
            
            Destroy(gameObject);
        }
    }
}
