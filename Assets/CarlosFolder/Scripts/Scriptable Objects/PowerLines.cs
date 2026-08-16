using UnityEngine;
[CreateAssetMenu(fileName = "PowerLines", menuName = "Scriptable Objects/PowerLines")]
public class PowerLines : ScriptableObject
{
    [Header("Power Line")]
    public string lineID;
    public GameObject source;
    public bool powered;
    public LineRenderer renderer;
    public GameObject lineObject;
    public Transform start;
    public Transform end;
}