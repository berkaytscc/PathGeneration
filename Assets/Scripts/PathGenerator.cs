using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolItem
{
    public GameObject prefab; // game object to be pooled.
    public int amount; // how many of them to be pooled.
    //public bool expandable; // this will allow the pool to create another platform if we run out of it.
}

[ExecuteInEditMode]
public class PathGenerator : MonoBehaviour
{
    private int colliderAmount;
    public LayerMask layer;
    //[SerializeField] private GameObject player;
    [SerializeField] private GameObject startingPlatform;
    [SerializeField] private GameObject platformToPlace;
    [SerializeField] private String straightPlatformName;
    [SerializeField] private String leftPlatformName;
    [SerializeField] private String rightPlatformName;
    public int mapSize = 20;
    [Range(1, 5)][SerializeField] private int LeftAndRightDensity;
    [HideInInspector] public int numberOfTurns;
    [HideInInspector] public int numberOfPlatforms;
    
    public List<PoolItem> items;
    public List<GameObject> tempItems;
    public List<GameObject> pooledItems;
    [SerializeField] private List<GameObject> expandableItems;
    public bool Regenerate;
    // public bool Generate;
    // public bool Clear;
    public static PathGenerator instance;
    
    [HideInInspector] public ShufflePath shufflePath;

    private int defaultAmount;
    private int leftAmount; // platform that are left from the reduction of the corners.
    private GameObject dummyTraveller;
    private int rightTurnAmount = 0;
    private int leftTurnAmount = 0;

    private int pozX;
    private int negX;
    //private GameObject playerGO;
    private GameObject worldParent;
    public static int MapSize { get; set; }

    private void SetAmounts(){
        // Set the default amounts of the platforms.
        foreach (var item in items)
        {
            item.amount = mapSize / items.Count;
        }
        // Add the leftAmount to the straight platforms
        defaultAmount = mapSize / items.Count;
        items[1].amount = Mathf.RoundToInt(((mapSize / items.Count) * LeftAndRightDensity) / 10);
        leftAmount = defaultAmount - items[1].amount;
        items[0].amount += leftAmount;

        items[2].amount = Mathf.RoundToInt(((mapSize / items.Count) * LeftAndRightDensity) / 10);
        leftAmount = defaultAmount - items[1].amount;
        items[0].amount += leftAmount;
        
        numberOfTurns = items[1].amount + items[2].amount;
    }
    
    public void GeneratePlatforms() {
            instance = this;
            shufflePath = GetComponent<ShufflePath>();
            Regenerate = false;
            // Generate = false;
            GameObject worldHolder = GameObject.FindGameObjectWithTag("PlatformHolder");
            //GameObject playerHolder = GameObject.FindGameObjectWithTag("Player");
            GameObject dummyHolder = GameObject.FindGameObjectWithTag("Dummy");
            if(worldHolder != null) DestroyImmediate(worldHolder);
            //if(playerHolder != null) DestroyImmediate(playerHolder);
            if(dummyHolder != null) DestroyImmediate(dummyHolder);
            
            worldParent = new GameObject("World");
            worldParent.tag = "PlatformHolder";
            MapSize = mapSize;
            if(dummyTraveller == null) {
                dummyTraveller = new GameObject("dummy");
                dummyTraveller.tag = "Dummy";
            }
            dummyTraveller.transform.position = Vector3.zero;
            dummyTraveller.transform.rotation = Quaternion.identity;
            
            dummyTraveller.transform.Translate(Vector3.forward * 10);
            // reorder the list (0 = straightPlatform, 1 = left, 2 = right)
            tempItems = new List<GameObject>() {null,null,null};
            foreach (var item in items)
            {
                if(item.prefab.name == straightPlatformName)
                {
                    tempItems.Insert(0,item.prefab);
                }
                else if (item.prefab.name == leftPlatformName)
                {
                    tempItems.Insert(1, item.prefab);
                }
                else if (item.prefab.name == rightPlatformName)
                {
                    tempItems.Insert(2,item.prefab);
                }
            }
    
            pooledItems = new List<GameObject>();
            expandableItems = new List<GameObject>();
            SetAmounts();
            tempItems.RemoveAll(x => !x); // remove the null objects
            // reorder the original list
            for (int i = 0; i < tempItems.Count; i++)
            {
                items[i].prefab = tempItems[i];
            }
        
            foreach (PoolItem item in items)
            {
            // Here down below with the 'Instantiate' method, we are not interested in
            // position and because it's the other part of our game doing that for us.
                for (int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(worldParent.transform);
                    pooledItems.Add(obj);
                }
            }
            
            for (int i = 0; i < MapSize; i++)
            {
                // place the first platform
                if(i == 0) {
                    GameObject firstPlatform = Instantiate(startingPlatform, Vector3.zero,
                        Quaternion.identity, worldParent.transform);
                    firstPlatform.name = "p First Platform";
                    //playerGO = Instantiate(player, Vector3.zero, Quaternion.identity);
                    //playerGO.transform.position += new Vector3(0,1.75f,0);
                    
                    continue;
                }
                // place the last platform
                if (i == PathGenerator.MapSize - 1 && !Regenerate){
                    GameObject lastPlatform = Instantiate(platformToPlace);
                    lastPlatform.name = "p Last Platform";
                    lastPlatform.transform.forward = dummyTraveller.transform.forward;
                    lastPlatform.transform.position = dummyTraveller.transform.position;
                    lastPlatform.transform.SetParent(worldParent.transform);
                    continue;
                }
                shufflePath.ShuffleThePath();
                
                for (int j = 0; j < pooledItems.Count; j++)
                {
                    if (!pooledItems[j].activeInHierarchy)
                    {
                        pooledItems[j].transform.forward = dummyTraveller.transform.forward;
                        pooledItems[j].SetActive(true);
                        pooledItems[j].transform.position = dummyTraveller.transform.position;
                        pooledItems[j].transform.rotation = dummyTraveller.transform.rotation;
                        if (pooledItems[j].name[0] == 'R')
                        {
                            dummyTraveller.transform.Rotate(0, 90, 0);
                        }
                        else if(pooledItems[j].name[0] == 'L')
                        {
                            dummyTraveller.transform.Rotate(0, -90, 0);
                        }
                        dummyTraveller.transform.Translate(Vector3.forward * 15);
                    }
                }
            }

            for (int i = 0; i < worldParent.transform.childCount; i++)
            {
                if(worldParent.transform.GetChild(i).gameObject.activeInHierarchy == false) {
                    DestroyImmediate(worldParent.transform.GetChild(i).gameObject);
                    
                }
            }

            if(Regenerate){
                Debug.Log(Regenerate + "regenerate calıstı");
        }
    }
   
    public void ClearPlatforms() 
    {
        pooledItems.Clear();
        expandableItems.Clear();
        // reset the ShufflePath script
        shufflePath.ResetShuffle();
        numberOfTurns = 0;
        GameObject worldHolder = GameObject.FindGameObjectWithTag("PlatformHolder");
        //GameObject playerHolder = GameObject.FindGameObjectWithTag("Player");
        GameObject dummyHolder = GameObject.FindGameObjectWithTag("Dummy");
        if(worldHolder != null) DestroyImmediate(worldHolder);
        //if(playerHolder != null) DestroyImmediate(playerHolder);
        if(dummyHolder != null) DestroyImmediate(dummyHolder);

        //TODO: Clear all the child objects of the World parent object
        Regenerate = false;
        // Clear = false;
    }
}

