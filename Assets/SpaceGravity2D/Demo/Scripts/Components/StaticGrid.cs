using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for creating static unchangable rectangular grid in scene.
    /// </summary>
    public class StaticGrid : MonoBehaviour
    {
        public float GridCellSize = 1f;
        public int GridAxisLinesCount = 100;
        public float GridLineWidht = 0.1f;
        public Material GridLineMaterial;
        [Header("Assign player reference to make grid follow.")]
        public Transform FollowTarget;

        private void Start()
        {
            CreateGrid();
        }

        private void Update()
        {
            if (FollowTarget != null)
            {
                var x = (int)FollowTarget.position.x / (int)GridCellSize;
                var z = (int)FollowTarget.position.z / (int)GridCellSize;
                transform.position = new Vector3(x * GridCellSize, transform.position.y, z * GridCellSize);
            }
        }

        private void CreateGrid()
        {
            for (int i = 0; i < GridAxisLinesCount + 1; i++)
            {
                var go = new GameObject("lineX_" + i);
                go.transform.SetParent(transform);
                var line = go.AddComponent<LineRenderer>();
                line.positionCount = 2;
                line.startWidth = GridLineWidht;
                line.endWidth = GridLineWidht;
                line.material = GridLineMaterial;
                line.useWorldSpace = false;
                line.SetPosition(0, new Vector3((-GridAxisLinesCount / 2f) * GridCellSize, 0f, (-GridAxisLinesCount / 2f) * GridCellSize + GridCellSize * i));
                line.SetPosition(1, new Vector3((GridAxisLinesCount / 2f) * GridCellSize, 0f, (-GridAxisLinesCount / 2f) * GridCellSize + GridCellSize * i));
            }
            for (int i = 0; i < GridAxisLinesCount + 1; i++)
            {
                var go = new GameObject("lineY_" + i);
                go.transform.SetParent(transform);
                var line = go.AddComponent<LineRenderer>();
                line.positionCount = 2;
                line.startWidth = GridLineWidht;
                line.endWidth = GridLineWidht;
                line.material = GridLineMaterial;
                line.useWorldSpace = false;
                line.SetPosition(0, new Vector3((-GridAxisLinesCount / 2f) * GridCellSize + GridCellSize * i, 0f, (-GridAxisLinesCount / 2f) * GridCellSize));
                line.SetPosition(1, new Vector3((-GridAxisLinesCount / 2f) * GridCellSize + GridCellSize * i, 0f, (GridAxisLinesCount / 2f) * GridCellSize));
            }

        }
    }
}