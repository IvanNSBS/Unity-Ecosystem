using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ObjectPooler : MonoBehaviour {

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int poolSize = 1000;
    }
    public Dictionary<string, Queue<GameObject>> poolDict;
    public List<Pool> pools;
    public static ObjectPooler Instance;

    #region Singleton

    private void Awake() {
        Instance = this;    
    } 
    #endregion
    void Start()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();
        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i < pool.poolSize; i++){
                var obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDict.Add(pool.tag, objectPool);
        }
    }

    public bool AddToPool(string tag, GameObject obj)
    {
        if(!poolDict.ContainsKey(tag)){
            Debug.LogWarning("Pool doesnt have this tag");
            return false;
        }

        obj.SetActive(false);
        poolDict[tag].Enqueue(obj);
        return true;
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Genes father, Genes mother)
    {
        if(!poolDict.ContainsKey(tag)){
            Debug.LogWarning("Pool doesnt have this tag");
            return null;
        }
        var obj = poolDict[tag].Dequeue();
        obj.SetActive(true);
        var obj_agent = obj.GetComponent<Agent>();
        var obj_life = obj.GetComponent<LifeComponent>();
        var obj_vision = obj.GetComponent<VisionComponent>();
        obj_agent.ResetAgent(true, father, mother);
        obj_life.ResetLifeStatus();
        obj_vision.ResetVision();
        obj.transform.position = pos;

        if (tag == "rabbit")
            World.instance.actualRabbitAmount++;
        if (tag == "wolf")
            World.instance.actualWolfAmount++;

        World.instance.UpdateUI();
        return obj;
    }
}