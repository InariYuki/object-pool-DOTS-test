using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private void Start() {
        float val = 1;
        for(int i = 0; i < 100000; i++){
            val += Mathf.Exp(Mathf.Sqrt(val));
        }
    }
    public float speed , rotation , exist_time , time;
    private void Update() {
        time += Time.deltaTime;
        if(time >= exist_time){
            //Destroy(gameObject);
            ObjectPool<BulletController>.instance.Recycle(this);
        }
        transform.Translate(new Vector3(speed * Time.deltaTime * Mathf.Cos(rotation + time) , speed * Time.deltaTime * Mathf.Sin(rotation + time) , 0));
    }
    public void Reset(){
        time = 0;
    }
}
