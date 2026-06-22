using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public static class GameUtil
{
    private static long _lastId = 0;

    public static void LoadFullData()
    {
        GameDataManager.Instance.LoadItemData("Item");
    }

    public static async UniTask<Sprite> LoadAndSetSpriteImage(Image targetImage, string spritePath)
    {
        Sprite sprite = await ResourceManager.Instance.LoadSprite(spritePath);
        if (sprite != null)
        {
            targetImage.sprite = sprite; 
        }
        return sprite;
    }

    public static long GenerateUniqueId()
    {
        long newId = DateTime.UtcNow.Ticks;

        while (true)
        {
            long lastId = Volatile.Read(ref _lastId);
            long idToAssign = (newId <= lastId) ? lastId + 1 : newId;

            if (Interlocked.CompareExchange(ref _lastId, idToAssign, lastId) == lastId)
            {
                return idToAssign;
            }
        }
    }
}
