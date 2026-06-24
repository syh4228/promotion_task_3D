using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; set; }


    private void Awake()
    {
        Inst = this;
    }

}
