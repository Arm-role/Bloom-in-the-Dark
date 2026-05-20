using System.Threading.Tasks;
using UnityEngine;

public interface IAdressablePoolService<T>
{
    Task<GameObject> AsyncGet(T prefab);

    void Return(T prefab, GameObject instance);

    // คืน instance ที่ active อยู่ทั้งหมดเข้า pool — เรียกตอนเข้า scene ใหม่
    // pool root เป็น DontDestroyOnLoad ถ้าไม่คืน object ที่ไม่ถูก Despawn จะค้างข้าม scene
    void ReturnAll();
}
