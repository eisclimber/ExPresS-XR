using UnityEngine;

public interface IPoolObject
{
    public void RetrieveFromPool(); 
    public void ReturnToPool(); 
}