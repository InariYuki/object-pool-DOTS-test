using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

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
            ObjectPool<BulletController>.instance.Recycle(this);
        }
        transform.Translate(new Vector3(speed * Time.deltaTime * Mathf.Cos(rotation + time) , speed * Time.deltaTime * Mathf.Sin(rotation + time) , 0));
    }
    public void Reset(){
        time = 0;
    }
}
public struct BulletData{
    public float _speed , _rotation , _exist_time , _time , _deltaTime;
    public bool _isReadyRecycle;
    public float3 _position;
    public BulletData(BulletController bc , float deltaTime){
        _speed = bc.speed;
        _rotation = bc.rotation;
        _exist_time = bc.exist_time;
        _time = bc.time;
        _deltaTime = deltaTime;
        _isReadyRecycle = false;
        _position = bc.transform.position;
    }
    public void Calculate(){
        _time += _deltaTime;
        if(_time >= _exist_time){
            _isReadyRecycle = true;
        }
        _position += new float3(_speed * _deltaTime * Mathf.Sin(_rotation + _time * 5) , _speed * _deltaTime * Mathf.Cos(_rotation + _time * 10) , 0);
        float val = 1;
        for(int i = 0; i < 100000; i++){
            val += Mathf.Exp(Mathf.Sqrt(val));
        }
    }
}
[BurstCompile]
public struct BulletUpdateJob : IJobParallelFor{
    public NativeArray<BulletData> bulletDataList;
    public void Execute(int index){
        BulletData data = bulletDataList[index];
        data.Calculate();
        bulletDataList[index] = data;
    }
}
