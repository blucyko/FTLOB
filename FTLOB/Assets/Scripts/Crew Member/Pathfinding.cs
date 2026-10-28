using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [Header("Ship Room Coordinates")]
    [SerializeField] private Vector3Int mainRoomOrigin;
    [SerializeField] private Vector3Int cockpitRoomOrigin;

    [Header("Pathfinding Test")]
    [SerializeField] private bool generatePath;
    [SerializeField] private bool showGrid;

    [SerializeField]
    private Vector2Int testStartPosition = new Vector2Int(0, 1);

    [SerializeField]
    private Vector2Int testEndPosition = new Vector2Int(5, 7);

    private bool pathGenerated;

    private Dictionary<Vector2, Cell> cells;

    [SerializeField]
    private List<Vector2> cellsToSearch = new List<Vector2>();

    [SerializeField]
    private List<Vector2> searchedCells = new List<Vector2>();

    [SerializeField]
    private List<Vector2> finalPath = new List<Vector2>();

    private void Update()
    {
        if (generatePath && !pathGenerated)
        {
            pathGenerated = true;

            if (GenerateGrid())
            {
                FindPath(new Vector2(testStartPosition.x, testStartPosition.y), new Vector2(testEndPosition.x, testEndPosition.y));
            }
        }
        else if (!generatePath)
        {
            pathGenerated = false;
        }
    }

    private bool GenerateGrid()
    {
        cells = new Dictionary<Vector2, Cell>();

        cellsToSearch.Clear();
        searchedCells.Clear();
        finalPath.Clear();

        if (groundTilemap == null)
        {
            Debug.LogError("Ground Tilemap is not assigned.", this);
            return false;
        }

        // Main ship room: 8 x 6 cells.
        AddRoomToGrid(mainRoomOrigin, 8, 6);

        // Cockpit: 4 x 4 cells.
        AddRoomToGrid(cockpitRoomOrigin, 5, 4);

        Debug.Log($"Generated {cells.Count} grid cells from the two ship rooms.",this);
        return cells.Count > 0;
    }

    private void AddRoomToGrid(Vector3Int origin, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(origin.x + x, origin.y + y, origin.z);
                // Only include cells that contain ground tiles.
                if (!groundTilemap.HasTile(tilePosition))
                    continue;

                Vector2 position = new Vector2(tilePosition.x, tilePosition.y);

                // Prevent duplicate cells if room rectangles overlap.
                if (cells.ContainsKey(position))
                    continue;

                Cell cell = new Cell(position);

                cell.isWall = wallTilemap != null && wallTilemap.HasTile(tilePosition);
                cells.Add(position, cell);
            }
        }
    }

    private void FindPath(Vector2 startPos, Vector2 endPos)
    {
        searchedCells.Clear();
        cellsToSearch.Clear();
        finalPath.Clear();

        if (cells == null)
        {
            Debug.LogWarning("Pathfinding: GenerateGrid must run first.", this);
            return;
        }

        if (!cells.TryGetValue(startPos, out Cell startCell))
        {
            Debug.LogWarning($"Pathfinding: Start {startPos} has no ground tile.", this);
            return;
        }

        if (!cells.TryGetValue(endPos, out Cell endCell))
        {
            Debug.LogWarning($"Pathfinding: Destination {endPos} has no ground tile.", this);
            return;
        }

        if (startCell.isWall || endCell.isWall)
        {
            Debug.LogWarning("Pathfinding: Start or destination is blocked by a wall.", this);
            return;
        }

        // Reset costs and connections before every search.
        foreach (Cell cell in cells.Values)
        {
            cell.fCost = int.MaxValue;
            cell.gCost = int.MaxValue;
            cell.hCost = int.MaxValue;
            cell.connection = cell.position;
        }

        if (startPos == endPos)
        {
            finalPath.Add(startPos);
            Debug.Log("Pathfinding: Already at the destination.");
            return;
        }

        startCell.gCost = 0;
        startCell.hCost = GetDistance(startPos, endPos);
        startCell.fCost = startCell.hCost;

        cellsToSearch.Add(startPos);

        while (cellsToSearch.Count > 0)
        {
            // Find the open cell with the lowest fCost.
            Vector2 cellToSearch = cellsToSearch[0];

            foreach (Vector2 position in cellsToSearch)
            {
                Cell candidate = cells[position];
                Cell currentBest = cells[cellToSearch];

                if (candidate.fCost < currentBest.fCost || (candidate.fCost == currentBest.fCost && candidate.hCost < currentBest.hCost))
                {
                    cellToSearch = position;
                }
            }

            cellsToSearch.Remove(cellToSearch);
            searchedCells.Add(cellToSearch);

            if (cellToSearch == endPos)
            {
                ReconstructPath(startPos, endPos);

                Debug.Log($"Pathfinding: Path found! " + $"Path length: {finalPath.Count} cells.");
                return;
            }

            SearchCellNeighbors(cellToSearch, endPos);
        }

        Debug.LogWarning($"Pathfinding: No path exists from {startPos} to {endPos}.");
    }

    private void SearchCellNeighbors(Vector2 cellPos, Vector2 endPos)
    {
        Cell currentCell = cells[cellPos];

        // Only four directions: up, down, left, right.
        // This prevents diagonal travel across tile corners.
        Vector2[] directions = {Vector2.up, Vector2.down, Vector2.left, Vector2.right};

        foreach (Vector2 direction in directions)
        {
            Vector2 neighborPos = cellPos + direction;

            if (!cells.TryGetValue(neighborPos, out Cell neighborNode))
                continue;

            if (neighborNode.isWall || searchedCells.Contains(neighborPos))
            {
                continue;
            }

            int gCostToNeighbor = currentCell.gCost + GetDistance(cellPos, neighborPos);

            if (gCostToNeighbor >= neighborNode.gCost)
                continue;

            neighborNode.connection = cellPos;
            neighborNode.gCost = gCostToNeighbor;
            neighborNode.hCost = GetDistance(neighborPos, endPos);
            neighborNode.fCost = neighborNode.gCost + neighborNode.hCost;

            if (!cellsToSearch.Contains(neighborPos))
                cellsToSearch.Add(neighborPos);
        }
    }

    private void ReconstructPath(Vector2 startPos, Vector2 endPos)
    {
        finalPath.Clear();

        Vector2 currentPos = endPos;
        int safetyCounter = 0;

        while (currentPos != startPos)
        {
            finalPath.Add(currentPos);

            Cell currentCell = cells[currentPos];

            if (currentCell.connection == currentPos)
            {
                Debug.LogError("Pathfinding: Broken connection while reconstructing path.");
                finalPath.Clear();
                return;
            }

            currentPos = currentCell.connection;

            safetyCounter++;

            if (safetyCounter > cells.Count)
            {
                Debug.LogError("Pathfinding: Path reconstruction exceeded grid size.");
                finalPath.Clear();
                return;
            }
        }

        finalPath.Add(startPos);
        finalPath.Reverse();
    }

    private int GetDistance(Vector2 pos1, Vector2 pos2)
    {
        // Manhattan distance: only horizontal and vertical movement.
        int dx = Mathf.Abs(Mathf.RoundToInt(pos1.x) - Mathf.RoundToInt(pos2.x));
        int dy = Mathf.Abs(Mathf.RoundToInt(pos1.y) - Mathf.RoundToInt(pos2.y));
        return (dx + dy) * 10;
    }

    public bool TryFindPath(Vector3 worldStart, Vector3 worldDestination, out List<Vector3> worldPath)
    {
        worldPath = new List<Vector3>();

        if (groundTilemap == null)
        {
            Debug.LogError("Pathfinding: Ground Tilemap is not assigned.");
            return false;
        }

        if (!GenerateGrid())
            return false;

        // Convert the crew and destination world positions into tile cells.
        Vector3Int startCell = groundTilemap.WorldToCell(worldStart);
        Vector3Int endCell = groundTilemap.WorldToCell(worldDestination);
        Debug.Log($"Cell {startCell} - > center {groundTilemap.GetCellCenterWorld(startCell)}");
        Debug.Log($"Cell {endCell} - > center {groundTilemap.GetCellCenterWorld(endCell)}");

        Vector2 startPos = new Vector2(startCell.x, startCell.y);
        Vector2 endPos = new Vector2(endCell.x, endCell.y);

        FindPath(startPos, endPos);

        if (finalPath.Count == 0 ||
            finalPath[finalPath.Count - 1] != endPos)
        {
            return false;
        }

        // Convert every path cell to its actual world-space center.
        foreach (Vector2 cell in finalPath)
        {
            Vector3Int cellPosition = new Vector3Int(Mathf.RoundToInt(cell.x), Mathf.RoundToInt(cell.y), startCell.z);
            Vector3 worldPosition = groundTilemap.GetCellCenterWorld(cellPosition);

            worldPath.Add(worldPosition);

            Debug.Log($"Path cell: {cellPosition} | " + $"World center: {worldPosition}");
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (!showGrid || cells == null || groundTilemap == null)
            return;

        foreach (KeyValuePair<Vector2, Cell> pair in cells)
        {
            Vector2 position = pair.Key;
            Vector3Int cellPosition = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), groundTilemap.WorldToCell(transform.position).z);
            Vector3 worldPosition = groundTilemap.GetCellCenterWorld(cellPosition);

            if (pair.Value.isWall)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            if (finalPath != null && finalPath.Contains(position))
                Gizmos.color = Color.magenta;

            Vector3 size = groundTilemap.cellSize;

            Gizmos.DrawCube(worldPosition, new Vector3(size.x, size.y, 0.1f));
        }
    }

    private class Cell
    {
        public Vector2 position;
        public int fCost = int.MaxValue;
        public int gCost = int.MaxValue;
        public int hCost = int.MaxValue;
        public Vector2 connection;
        public bool isWall;

        public Cell(Vector2 pos)
        {
            position = pos;
            connection = pos;
        }
    }
}