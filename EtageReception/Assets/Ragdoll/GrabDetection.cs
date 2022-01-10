using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabDetection : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<GameObject> _grabRangeObjects = null;
    #endregion Fields


    #region Properties
    public List<GameObject> GrabRangeObject
    {
        get
        {
            return _grabRangeObjects;
        }
    }
    #endregion Properties

    void Start()
    {
        _grabRangeObjects = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item" || other.tag == "Key" || other.tag == "Food")
        {
            _grabRangeObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item" || other.tag == "Key" || other.tag == "Food")
        {
            _grabRangeObjects.Remove(other.gameObject);
        }
    }
}
