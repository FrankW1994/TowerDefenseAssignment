using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager main;

    public Transform startPos;
    public Transform[] path;

    private void Awake(){
        main = this;
    }
}
