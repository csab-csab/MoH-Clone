using System;
using UnityEngine;
using UnityEngine.Serialization;

//Offsets material to make it look like water is flowing
[RequireComponent(typeof(Material), typeof(MeshRenderer))]
public class AnimateWaterTexture : MonoBehaviour
{
    private Material _material;
    
    [SerializeField] float offSet;
    
  
    private float _currentOffset;

    [SerializeField] private float speed;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
    }
    
    private void Update()
    {
        if (_currentOffset >= offSet)
        {
            _currentOffset = 0;
        }

        _currentOffset = Mathf.MoveTowards(_currentOffset, 
            offSet, Time.deltaTime * speed);
        
        _material.mainTextureOffset = new Vector2(_material.mainTextureOffset.x, _currentOffset);
    }
    
    
}