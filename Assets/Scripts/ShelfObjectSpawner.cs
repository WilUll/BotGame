using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfObjectSpawner : MonoBehaviour
{
    public List<Shelf> shelves = new List<Shelf>();
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (Shelf shelf in shelves)
        {
            float shelfOffset = 0;
            for (int i = 0; i < shelf.ShelfObjects.Count; i++)
            {
                GameObject shelfObject = Instantiate(shelf.ShelfObjects[i]);
                shelfObject.transform.localScale /= transform.lossyScale.z;
                shelfObject.transform.parent = shelf.ShelfParent;
                shelfObject.transform.position = shelf.ShelfParent.position + Vector3.forward * shelfOffset;
                shelfOffset += shelf.ShelfObjects[i].GetComponent<Collider>().bounds.max.z;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Shelf
{
    public Transform ShelfParent;
    public List<GameObject> ShelfObjects;
}
