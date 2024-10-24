using System.Threading.Tasks;
using UnityEngine;

public abstract class UGUIDataElement<TDataType>: MonoBehaviour
{
    public abstract void SetData(TDataType data);
}

public abstract class UGUIAsyncDataElement<TDataType> : MonoBehaviour
{
    public abstract Task SetDataAsync(TDataType data);
}