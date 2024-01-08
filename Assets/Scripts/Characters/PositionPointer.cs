using UnityEngine;

// data class
public class PositionPointer : MonoBehaviour
{
    [SerializeField]
    private int _Position;

    public int Position => _Position;
}
