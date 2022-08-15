using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] GameObject bullet;
    ObjectPool<BulletController> bullet_pool;
    List<BulletController> bulletUpdateList = new List<BulletController>();
    private void Awake() {
        bullet_pool = ObjectPool<BulletController>.instance;
        bullet_pool.InitPool(bullet);
        int warm_up_count = 2000;
        for(int i = 0; i < warm_up_count; i++){
            BulletController t = bullet_pool.Spawn(Vector3.zero , Quaternion.identity);
            t.time = 10;
            bulletUpdateList.Add(t);
        }
    }
    private void Update() {
        print(bullet_pool.queue_count);
        if(Input.GetKey(KeyCode.Z)){
            float d = 4f;
            float d_radian = 360/d * Mathf.PI / 180;
            for(int i = 0; i < d; i++){
                BulletController bullet_instanced = bullet_pool.Spawn(transform.position , Quaternion.identity);
                bullet_instanced.rotation = d_radian * i;
                bullet_instanced.Reset();
                bulletUpdateList.Add(bullet_instanced);
            }
        }
        if(bulletUpdateList.Count != 0){
            NativeArray<BulletData> nativeBulletDataList = new NativeArray<BulletData>(bulletUpdateList.Count , Allocator.TempJob);
            for(int i = 0; i < bulletUpdateList.Count; i++){
                nativeBulletDataList[i] = new BulletData(bulletUpdateList[i] , Time.deltaTime);
            }
            BulletUpdateJob bulletUpdateJob = new BulletUpdateJob();
            bulletUpdateJob.bulletDataList = nativeBulletDataList;
            JobHandle handle = bulletUpdateJob.Schedule(bulletUpdateList.Count , 100);
            handle.Complete();
            for(int i = bulletUpdateList.Count - 1; i >= 0; i--){
                bulletUpdateList[i].transform.position = nativeBulletDataList[i]._position;
                bulletUpdateList[i].time = nativeBulletDataList[i]._time;
                if(nativeBulletDataList[i]._isReadyRecycle){
                    bullet_pool.Recycle(bulletUpdateList[i]);
                    bulletUpdateList.RemoveAt(i);
                }
            }
            nativeBulletDataList.Dispose();
        }
    }
    private void FixedUpdate() {
        Movement();
    }
    void Movement(){
        float x_movement = Input.GetAxisRaw("Horizontal");
        float y_movement = Input.GetAxisRaw("Vertical");
        transform.Translate(new Vector3(x_movement , y_movement).normalized * Time.fixedDeltaTime * speed);
    }
}
