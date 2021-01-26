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
        int direction;
        for (int i = 1; i < path.m_Waypoints.Length; i++)
        {
            direction = Random.Range(0, 2);
            if (direction == 0)
            {
                path.m_Waypoints[i].position =
                    path.m_Waypoints[i - 1].position + new Vector3(0, Screen.height / 200, 0);
            }
            else
            {
                var side = Random.Range(1, 3);
                path.m_Waypoints[i].position =
                    path.m_Waypoints[i - 1].position + new Vector3(Mathf.Pow(-1, side) * (Screen.width / 200), 0, 0);
            }
        }
    }
}
