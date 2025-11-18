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

    // Variável para controlar a visibilidade
    private bool _isVisible = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Método para mostrar/ocultar a linha
    public void SetPathVisibility(bool visible)
    {
        _isVisible = visible;

        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = visible;
        }
    }

    // Método para alternar a visibilidade
    public void TogglePathVisibility()
    {
        _isVisible = !_isVisible;

        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = _isVisible;
        }
    }

    // Método para obter o estado atual da visibilidade
    public bool IsPathVisible()
    {
        return _isVisible;
    }

    // Resto do seu código permanece igual...
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
            corners.Insert(0, pointA.position);
            corners.Add(pointB.position);
            List<Vector3> cornersOrtho = Orthogonalize(corners);

            _lineRenderer.positionCount = cornersOrtho.Count;

            for (int i = 0; i < cornersOrtho.Count; i++)
                _lineRenderer.SetPosition(i, cornersOrtho[i] + Vector3.up);

            print("<color=green>PATH FOUND");
        }
        else print("<color=red>PATH FAILED");
    }

    public void ResetPath()
    {
        if (_lineRenderer.positionCount <= 0) return;

        _lineRenderer.positionCount = 0;
    }

    Vector3 SampleToNavmesh(Vector3 sourcePos)
    {
        if (NavMesh.SamplePosition(sourcePos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            return hit.position;

        return sourcePos; // fallback
    }

    List<Vector3> Orthogonalize(List<Vector3> corners, float threshold = 0f)
    {
        // Seu código existente...
        List<Vector3> result = new List<Vector3>();
        if (corners.Count == 0)
            return result;

        Vector3 first = corners[0];
        first.y = 1f;
        result.Add(first);

        int i = 0;

        while (i < corners.Count - 1)
        {
            Vector3 start = result[result.Count - 1];
            Vector3 next = corners[i + 1];
            next.y = 1f;

            float dx = Mathf.Abs(next.x - start.x);
            float dz = Mathf.Abs(next.z - start.z);

            bool horizontal = dx > dz;

            int farthest = i + 1;

            for (int j = i + 2; j < corners.Count; j++)
            {
                Vector3 prev = corners[farthest];
                Vector3 curr = corners[j];
                curr.y = 1f;

                float dx2 = Mathf.Abs(curr.x - prev.x);
                float dz2 = Mathf.Abs(curr.z - prev.z);

                bool sameDir;

                if (horizontal)
                    sameDir = dx2 >= (dz2 + threshold);
                else
                    sameDir = dz2 >= (dx2 + threshold);

                if (!sameDir)
                    break;

                farthest = j;
            }

            Vector3 end = corners[farthest];
            end.y = 1f;

            if (horizontal)
            {
                float avgZ = (start.z + end.z) * 0.5f;
                start.z = avgZ;
                end.z = avgZ;
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