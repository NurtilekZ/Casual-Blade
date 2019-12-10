using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaypointsHandler : MonoBehaviour
{
    private CinemachinePath path;

    private void Start()
    {
        path = GetComponent<CinemachinePath>();
    }

    public void PlaceWaypoints()
    {
        int side;
        for (int i = 1; i < path.m_Waypoints.Length; i++)
        {
            side = Random.Range(0, 2);
            if (side == 0)
            {
                path.m_Waypoints[i].position =
                    path.m_Waypoints[i - 1].position + new Vector3(0, ScreenSize.GetScreenToWorldHeight, 0);
            }
            else
            {
                path.m_Waypoints[i].position =
                    path.m_Waypoints[i - 1].position + new Vector3(ScreenSize.GetScreenToWorldWidth, 0, 0);
            }
        }
    }
}
