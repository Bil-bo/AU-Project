using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPointer : MonoBehaviour
{
    [SerializeField]
    private int _Position;

    public int Position => _Position;
}
