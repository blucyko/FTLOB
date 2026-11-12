using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [Header("Obstacle Detection")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float obstacleDetectionRadius = 0.2f;

    [Header("Pathfinding Test")]
    [SerializeField] private bool generatePath;
    [SerializeField] private bool showGrid;

    [SerializeField]
    private Vector3Int testStartPosition = new Vector3Int(0, 1, 0);

    [SerializeField]
    private Vector3Int testEndPosition = new Vector3Int(5, 7, 0);

    private bool pathGenerated;

    private Dictionary<Vector3Int, Cell> cells;

    [SerializeField]
    private List<Vector3Int> cellsToSearch = new List<Vector3Int>();

    [SerializeField]
    private List<Vector3Int> searchedCells = new List<Vector3Int>();

    [SerializeField]
    private List<Vector3Int> finalPath = new List<Vector3Int>();

    private void Update()
    {
        if (generatePath && !pathGenerated)
        {
            pathGenerated = true;

            if (GenerateGrid())
            {
                FindPath(
                   new Vector3Int(testStartPosition.x, testStartPosition.y, 0), new Vector3Int(testEndPosition.x, testEndPosition.y, 0));
            }
        }
        else if (!generatePath)
        {
            pathGenerated = false;
        }
    }

    // Generate grid using painted ground tiles
    private bool GenerateGrid()
    {
        cells = new Dictionary<Vector3Int, Cell>();

        cellsToSearch.Clear();
        searchedCells.Clear();
        finalPath.Clear();

        if (groundTilemap == null)
        {
            Debug.LogError("Ground Tilemap is not assigned.", this);
            return false;
        }

        Physics2D.SyncTransforms();
        BoundsInt groundBounds = groundTilemap.cellBounds;
        foreach (Vector3Int position in groundBounds.allPositionsWithin)
        {
            if (!groundTilemap.HasTile(position))
                continue;

            Cell cell = new Cell(position);

            Vector3 worldCenter = groundTilemap.GetCellCenterWorld(position);

            // Mark cell as wall if it has a wall tile.
            bool isWallTile = wallTilemap != null && wallTilemap.HasTile(position);

            // Check if there's an obstacle in the cell.
            bool hasObstacle = Physics2D.OverlapCircle(worldCenter, obstacleDetectionRadius, obstacleLayer) != null;

            cell.isWall = isWallTile || hasObstacle;
            cells.Add(position, cell);
        }

        Debug.Log($"Generated {cells.Count} walkable cells from ground tilemap.",this);
        return cells.Count > 0;
    }

    private void FindPath(Vector3Int startPos, Vector3Int endPos)
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
            Vector3Int cellToSearch = cellsToSearch[0];

            foreach (Vector3Int position in cellsToSearch)
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

    private void SearchCellNeighbors(Vector3Int cellPos, Vector3Int endPos)
    {
        Cell currentCell = cells[cellPos];

        // Eight possible movement directions
        Vector3Int[] directions = 
        {
            new Vector3Int( 0,  1, 0), // Up
            new Vector3Int( 0, -1, 0), // Down
            new Vector3Int(-1,  0, 0), // Left
            new Vector3Int( 1,  0, 0), // Right

            new Vector3Int(-1,  1, 0), // Up-left
            new Vector3Int( 1,  1, 0), // Up-right
            new Vector3Int(-1, -1, 0), // Down-left
            new Vector3Int( 1, -1, 0)  // Down-right
        };

        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPos = cellPos + direction;

            if (!cells.TryGetValue(neighborPos, out Cell neighborNode))
                continue;

            if (neighborNode.isWall || searchedCells.Contains(neighborPos))
            {
                continue;
            }
            bool isDiagonal = direction.x != 0 && direction.y != 0;
            if (isDiagonal)
            {
                // Prevent diagonal movement through corners of walls by checking adjacent cells.
                Vector3Int horizontalNeighbor = cellPos + new Vector3Int(direction.x, 0, 0);
                Vector3Int verticalNeighbor = cellPos + new Vector3Int(0, direction.y, 0);
                if (!cells.TryGetValue(horizontalNeighbor, out Cell horizontalCell) || horizontalCell.isWall || 
                    !cells.TryGetValue(verticalNeighbor, out Cell verticalCell) || verticalCell.isWall)
                {
                    continue;
                }
            }

            // Check that movement to the neighbor is not blocked by an obstacle in the world.
            if (!CanMoveBetween(cellPos, neighborPos))
            {
                continue;
            }

            int movementCost = isDiagonal ? 14 : 10; // Diagonal movement costs more

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

    private bool CanMoveBetween(Vector3Int startCell, Vector3Int endCell)
    {
        Vector3 startWorld = groundTilemap.GetCellCenterWorld(startCell);
        Vector3 endWorld = groundTilemap.GetCellCenterWorld(endCell);
        Vector2 direction = (endWorld - startWorld).normalized;
        float distance = Vector2.Distance(startWorld, endWorld);

        RaycastHit2D hit = Physics2D.CircleCast(startWorld, obstacleDetectionRadius, direction, distance, obstacleLayer);
        return hit.collider == null;
    }

    private void ReconstructPath(Vector3Int startPos, Vector3Int endPos)
    {
        finalPath.Clear();

        Vector3Int currentPos = endPos;
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

    private int GetDistance(Vector3Int pos1, Vector3Int pos2)
    {
        int dx = Mathf.Abs(pos1.x - pos2.x);
        int dy = Mathf.Abs(pos1.y - pos2.y);

        int minDistance = Mathf.Min(dx, dy);
        int maxDistance = Mathf.Max(dx, dy);

        // Diagonal movement costs 14, straight movement costs 10.
        return (minDistance * 14 + (maxDistance - minDistance) * 10);
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
        Debug.Log($"Start Cell: {startCell} - > center {groundTilemap.GetCellCenterWorld(startCell)}");
        Debug.Log($"End Cell: {endCell} - > center {groundTilemap.GetCellCenterWorld(endCell)}");

        FindPath(startCell, endCell);

        if (finalPath.Count == 0 ||
            finalPath[finalPath.Count - 1] != endCell)
        {
            return false;
        }

        // Convert every path cell to its actual world-space center.
        foreach (Vector3Int cell in finalPath)
        {
            Vector3 worldPosition = groundTilemap.GetCellCenterWorld(cell);

            worldPath.Add(worldPosition);

            Debug.Log($"Path cell: {cell} | " + $"World center: {worldPosition}");
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (!showGrid || cells == null || groundTilemap == null)
            return;

        foreach (KeyValuePair<Vector3Int, Cell> pair in cells)
        {
            Vector3Int cellPosition = pair.Key;
            Vector3 worldPosition = groundTilemap.GetCellCenterWorld(cellPosition);

            if (pair.Value.isWall)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            if (finalPath != null && finalPath.Contains(cellPosition))
                Gizmos.color = Color.magenta;

            Vector3 size = groundTilemap.cellSize;

            Gizmos.DrawCube(worldPosition, new Vector3(size.x, size.y, 0.1f));
        }
    }

    private class Cell
    {
        public Vector3Int position;
        public int fCost = int.MaxValue;
        public int gCost = int.MaxValue;
        public int hCost = int.MaxValue;
        public Vector3Int connection;
        public bool isWall;

        public Cell(Vector3Int pos)
        {
            position = pos;
            connection = pos;
        }
    }
}