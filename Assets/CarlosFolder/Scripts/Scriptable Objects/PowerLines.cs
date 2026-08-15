using UnityEngine;

[CreateAssetMenu(fileName = "PowerLines", menuName = "Scriptable Objects/PowerLines")]
public class PowerLines : ScriptableObject
{
    [Header("Power Line")]
    public string lineID;
    public LineRenderer renderer;
    public Transform start;
    public Transform end;   
}
