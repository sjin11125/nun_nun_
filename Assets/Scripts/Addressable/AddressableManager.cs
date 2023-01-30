using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    public AssetReference SpawnablePrefab;
    // Start is called before the first frame update
    public void DownloadAssets(object key)
    {
        Addressables.GetDownloadSize(key).Completed += (opSize) =>
        {
            if (opSize.Status == AsyncOperationStatus.Succeeded && opSize.Result > 0)
            {
                Addressables.DownloadDependenciesAsync(key, true).Completed += (opDownload) =>
                {
                    if (opDownload.Status != AsyncOperationStatus.Succeeded)
                    {
                        return;
                    }

                };
            };
        };

      

    }


}
