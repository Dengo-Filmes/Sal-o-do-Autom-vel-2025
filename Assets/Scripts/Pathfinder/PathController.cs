using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PathController : MonoBehaviour
{
    public static PathController Instance;

    LineRenderer _lineRenderer;

    public Transform pointA;
    public Transform pointB;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPositions(Transform totem, Transform destination)
    {
        pointA = totem;
        pointB = destination;
    }

    public void CalculatePath()
    {
        NavMeshPath path = new();

        Vector3 pointAOnNavMesh = SampleToNavmesh(pointA.position);
        Vector3 pointBOnNavMesh = SampleToNavmesh(pointB.position);

        if (NavMesh.CalculatePath(pointAOnNavMesh, pointBOnNavMesh, NavMesh.AllAreas, path))
        {
            List<Vector3> corners = path.corners.ToList();
            //corners.Add(pointB.position);
            List<Vector3> cornersOrtho = Orthogonalize(corners);

            _lineRenderer.positionCount = cornersOrtho.Count;

            for (int i = 0; i < cornersOrtho.Count; i++)
                _lineRenderer.SetPosition(i, cornersOrtho[i] + Vector3.up);

            print("<color=green>PATH FOUND");
        }
        else print("<color=red>PATH FAILED");
    }

    Vector3 SampleToNavmesh(Vector3 sourcePos)
    {
        if (NavMesh.SamplePosition(sourcePos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            return hit.position;

        return sourcePos; // fallback
    }

    List<Vector3> Orthogonalize(List<Vector3> corners)
    {
        List<Vector3> result = new List<Vector3>();
        if (corners.Count == 0)
            return result;

        result.Add(corners[0]);

        int i = 0;

        while (i < corners.Count - 1)
        {
            Vector3 start = result[result.Count - 1];   // importante: usar o valor já normalizado
            Vector3 next = corners[i + 1];

            float dx = Mathf.Abs(next.x - start.x);
            float dz = Mathf.Abs(next.z - start.z);

            bool horizontal = dx > dz;

            int farthest = i + 1;

            // Encontra o último ponto contínuo na mesma direção
            for (int j = i + 2; j < corners.Count; j++)
            {
                Vector3 prev = corners[farthest];
                Vector3 curr = corners[j];

                float dx2 = Mathf.Abs(curr.x - prev.x);
                float dz2 = Mathf.Abs(curr.z - prev.z);

                bool sameDir = horizontal ? (dx2 >= dz2) : (dz2 >= dx2);
                if (!sameDir)
                    break;

                farthest = j;
            }

            // Ponto final bruto
            Vector3 end = corners[farthest];

            // ----------------------------------------------------
            // NORMALIZAÇÃO: média aplicada nos DOIS pontos
            // ----------------------------------------------------

            if (horizontal)
            {
                float avgZ = (start.z + end.z) * 0.5f;
                start.z = avgZ;
                end.z = avgZ;

                // atualizar o último ponto já colocado
                result[result.Count - 1] = start;
            }
            else
            {
                float avgX = (start.x + end.x) * 0.5f;
                start.x = avgX;
                end.x = avgX;

                result[result.Count - 1] = start;
            }

            result.Add(end);
            i = farthest;
        }

        return result;
    }
}
