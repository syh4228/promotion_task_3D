using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    public InventoryManager Inventory { get; private set; }
    public GameDataManager Data { get; private set; }

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(gameObject);

        Data = gameObject.AddComponent<GameDataManager>();
        Inventory = gameObject.AddComponent<InventoryManager>();

        GameUtil.LoadFullData();
    }

}
