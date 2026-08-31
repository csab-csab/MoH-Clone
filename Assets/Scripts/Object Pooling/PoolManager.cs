using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance{get; private set;}

    [Header("Blood Splatter")]
    [SerializeField]private GameObject bloodSplatterObject;
    [SerializeField]private int bloodSplatterMaxSize = 500;
    private int _currentSize;
    private List<GameObject> _bloodSplatters = new List<GameObject>();
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
    }

    private void Start()
    {
        PoolBloodSplatter();
    }

    private void PoolBloodSplatter()
    {
        for (_currentSize = 0; _currentSize < bloodSplatterMaxSize; _currentSize++)
        {
            var splat = Instantiate(bloodSplatterObject, this.gameObject.transform);
            splat.SetActive(false);
            _bloodSplatters.Add(splat);
        }
    }

    //Requires position to ensure particle effect can play and look correct on enable
    public GameObject PlayBloodSplatter(Vector3 position, Quaternion rotation)
    {
        foreach (var splat in _bloodSplatters)
        {
            if (!splat.activeInHierarchy)
            {
                splat.transform.position = position;
                splat.transform.rotation = rotation;
                splat.SetActive(true);
                return splat;
            }
        }

        //if no inactive splatter is found, create and return a new one.
        var newSplat = Instantiate(bloodSplatterObject, this.gameObject.transform);
        Destroy(newSplat, 30);
        return newSplat;
    }
  
}

