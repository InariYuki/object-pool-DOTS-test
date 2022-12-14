using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    Queue<T> object_queue;
    GameObject prefab;
    static ObjectPool<T> _instance = null;
    public static ObjectPool<T> instance{
        get{
            if(_instance == null){
                _instance = new ObjectPool<T>();
            }
            return _instance;
        }
    }
    public int queue_count{
        get{
            return object_queue.Count;
        }
    }
    public void InitPool(GameObject pre){
        prefab = pre;
        object_queue = new Queue<T>();
    }
    public T Spawn(Vector3 pos , Quaternion quat){
        if(prefab == null){
            Debug.LogError(typeof(T) + " prefab not set!");
            return default(T);
        }
        if(queue_count <= 0){
            GameObject g = GameObject.Instantiate(prefab , pos , quat);
            T t = g.GetComponent<T>();
            if(t == null){
                Debug.LogError(typeof(T) + " not found in prefab");
                return default(T);
            }
            object_queue.Enqueue(t);
        }
        T obj = object_queue.Dequeue();
        obj.gameObject.transform.position = pos;
        obj.gameObject.transform.rotation = quat;
        obj.gameObject.SetActive(true);
        return obj;
    }
    public void Recycle(T obj){
        object_queue.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
