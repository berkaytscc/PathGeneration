using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[ExecuteInEditMode]
public class ShufflePath : MonoBehaviour
{
    
    public static ShufflePath instance;

    private int intensity;
    private int maxLimit;
    private int minLimit = 4;
    private List<GameObject> tempPathList;
    private List<GameObject> paths;
    private List<Char> pathsNames;
    private int k = 0;
    private bool isExceeded;
    private bool isShuffled = false;
    private GameObject tempObject;
    private int index;

    private void Start()
    {
        instance = this;
    }

    //check if we exceed the number of consecutive straight paths
    public bool CheckIfTheLimitExeeded(List<GameObject> list)
    {
        if(paths.Count >= maxLimit)
        {
            isExceeded = true;
            for (int i = 1; i < maxLimit; i++)
            {
                if (paths.Count - i >= 0)
                {
                    if (paths[^i].name[0] != 'S')
                    {
                        return false;
                    }   
                }
            }
            return isExceeded;
        }
        return false;
    }

    public void ShuffleThePath()
    {
        
        if (!isShuffled)
        {
            tempPathList = PathGenerator.instance.pooledItems;
            intensity = tempPathList.Count / PathGenerator.instance.numberOfTurns;
            minLimit = intensity / 2;
            maxLimit = intensity;

            //placing first straight lines as starting
            for (int i = 0; i < minLimit; i++)
            {
                paths.Add(tempPathList[0]);
                pathsNames.Add(tempPathList[0].name[0]);
                tempPathList.RemoveAt(0);
            }
            
            //then taking a random path and adding it to the new path list
            while (tempPathList.Count > 0)
            {
                Random r = new Random();
                
                index = r.Next(0, tempPathList.Count);
                
                //since the name of the straight path starts with S, we're checking if the name of the random path starts with S
                if (tempPathList[index].name[0] == 'S')
                {
                    //checking if we exceed the number of consecutive straight paths
                    isExceeded = CheckIfTheLimitExeeded(paths);
                    //place the random path if we didn't exceed the number of consecutive straight paths. ELse, just pass it.
                    if (!isExceeded)
                    {
                        paths.Add(tempPathList[index]);
                        
                        pathsNames.Add(tempPathList[index].name[0]);
                        
                        tempPathList.RemoveAt(index);
                    }
                }
                //if not straight path, add a turn, then place straight paths as much as min limit, then go on.
                else if (tempPathList[index].name[0] != 'S')
                {
                    paths.Add(tempPathList[index]);
                    tempPathList.RemoveAt(0);
                    for (int j = 0; j < minLimit; j++)
                    {
                        if (tempPathList.Count > 0)
                        {
                            paths.Add(tempPathList[0]);
                            
                            pathsNames.Add(tempPathList[0].name[0]);
                            
                            tempPathList.RemoveAt(0);
                        }
                    }
                }
            }
            PathGenerator.instance.pooledItems = paths;
            isShuffled = true;
        }
    }
    public void ResetShuffle()
    {
        isShuffled = false;
        paths.Clear();
        pathsNames.Clear();
        tempPathList.Clear();
    }
}
