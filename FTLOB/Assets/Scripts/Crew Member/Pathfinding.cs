using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [Header("Navigation Grid)")]
    [SerializeField] private Vector2 navigationCellSize;

    [Header("Obstacle Detection")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField, Min(0f)] private float movementCheckRadius = 0.01f;

    [Header("Pathfinding Test")]
    [SerializeField] private bool generatePath;
    [SerializeField] private bool showGrid;

    [SerializeField]
    private Vector3Int testStartPosition = new Vector3Int(0, 1, 0);

    [SerializeField]
    private Vector3Int testEndPosition = new Vector3Int(5, 7, 0);

    private bool pathGenerated;

    // Each original tile is subdivided into smaller navigation cells, which are stored in this dictionary.
    private Dictionary<Vector2Int, Cell> cells;

    [SerializeField]
    private List<Vector2Int> cellsToSearch = new List<Vector2Int>();

    [SerializeField]
    private List<Vector2Int> searchedCells = new List<Vector2Int>();

    [SerializeField]
    private List<Vector2Int> finalPath = new List<Vector2Int>();

    private void Update()
    {
        if (generatePath && !pathGenerated)
        {
            pathGenerated = true;

            if (GenerateGrid())
            {
                Vector2Int start = TileToNavigationCell(testStartPosition);
                Vector2Int end = TileToNavigationCell(testEndPosition);
                FindPath(start, end);
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
        cells = new Dictionary<Vector2Int, Cell>();

        cellsToSearch.Clear();
        searchedCells.Clear();
        finalPath.Clear();

        if (groundTilemap == null)
        {
            Debug.LogError("Ground Tilemap is not assigned.", this);
            return false;
        }

        Physics2D.SyncTransforms();

        // Store the original cell size and bounds of the ground tilemap for Gizmos visualization
        navigationCellSize = new Vector2(groundTilemap.cellSize.x, groundTilemap.cellSize.y);
        BoundsInt groundBounds = groundTilemap.cellBounds;

        foreach (Vector3Int position in groundBounds.allPositionsWithin)
        {
            if (!groundTilemap.HasTile(position))
                continue;

            Vector2Int tilePosition = new Vector2Int(position.x, position.y);
            Vector3 worldCenter = groundTilemap.GetCellCenterWorld(position);

            // Check obstacle colliders
            bool hasObstacle = Physics2D.OverlapCircle(worldCenter, movementCheckRadius, obstacleLayer) != null;
            Cell cell = new Cell(tilePosition, worldCenter);
            cell.isWall = hasObstacle;
            cells.Add(tilePosition, cell);
        }

        Debug.Log($"Generated {cells.Count} navigation cells.");
        return cells.Count > 0;
    }

    // Convert a tile position to its corresponding navigation cell position
    private Vector2Int TileToNavigationCell(Vector3Int tilePosition)
    {
        return new Vector2Int(tilePosition.x, tilePosition.y);
    }

    // Convert a world position into its corresponding navigation cell position
    private Vector2Int WorldToNavigationCell(Vector3 worldPosition)
    {
        Vector3Int tilePosition = groundTilemap.WorldToCell(worldPosition);
        return new Vector2Int(tilePosition.x, tilePosition.y);
    }

    private void FindPath(Vector2Int startPos, Vector2Int endPos)
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
            Vector2Int cellToSearch = cellsToSearch[0];

            foreach (Vector2Int position in cellsToSearch)
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

    private void SearchCellNeighbors(Vector2Int cellPos, Vector2Int endPos)
    {
        Cell currentCell = cells[cellPos];

        // Eight possible movement directions
        Vector2Int[] directions = 
        {
            new Vector2Int( 0,  1), // Up
            new Vector2Int( 0, -1), // Down
            new Vector2Int(-1,  0), // Left
            new Vector2Int( 1,  0), // Right

            new Vector2Int(-1,  1), // Up-left
            new Vector2Int( 1,  1), // Up-right
            new Vector2Int(-1, -1), // Down-left
            new Vector2Int( 1, -1)  // Down-right
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPos = cellPos + direction;

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
                Vector2Int horizontalNeighbor = cellPos + new Vector2Int(direction.x, 0);
                Vector2Int verticalNeighbor = cellPos + new Vector2Int(0, direction.y);
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

    private bool CanMoveBetween(Vector2Int startCell, Vector2Int endCell)
    {
        Vector3 startWorld = cells[startCell].worldPosition;
        Vector3 endWorld = cells[endCell].worldPosition;
        Vector2 direction = (endWorld - startWorld).normalized;
        float distance = Vector2.Distance(startWorld, endWorld);

        RaycastHit2D hit = Physics2D.CircleCast(startWorld, movementCheckRadius, direction, distance, obstacleLayer);
        return hit.collider == null;
    }

    private void ReconstructPath(Vector2Int startPos, Vector2Int endPos)
    {
        finalPath.Clear();

        Vector2Int currentPos = endPos;
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

    private int GetDistance(Vector2Int pos1, Vector2Int pos2)
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
        Vector2Int startCell = WorldToNavigationCell(worldStart);
        Vector2Int endCell = WorldToNavigationCell(worldDestination);
        Debug.Log($"Start Cell: {startCell} - > center {GetCellWorldPosition(startCell)}");
        Debug.Log($"End Cell: {endCell} - > center {GetCellWorldPosition(endCell)}");

        FindPath(startCell, endCell);

        if (finalPath.Count == 0 || finalPath[finalPath.Count - 1] != endCell)
        {
            return false;
        }

        // Convert every path cell to its actual world-space center.
        foreach (Vector2Int cell in finalPath)
        {
            Vector3 worldPosition = GetCellWorldPosition(cell);

            worldPath.Add(worldPosition);

            Debug.Log($"Path cell: {cell} | " + $"World center: {worldPosition}");
        }
        return true;
    }

    private Vector3 GetCellWorldPosition(Vector2Int cellPos)
    {
        return cells[cellPos].worldPosition;
    }

    private void OnDrawGizmos()
    {
        if (!showGrid || cells == null || groundTilemap == null)
            return;

        foreach (KeyValuePair<Vector2Int, Cell> pair in cells)
        {
            Cell cell = pair.Value;

            // Default colors based on obstacle detection.
            Gizmos.color = cell.isWall ? Color.red : Color.green;

            // Highlight cells that belong to the final path.
            if (finalPath != null && finalPath.Contains(cell.position))
            {
                Gizmos.color = Color.magenta;
            }

            Gizmos.DrawCube(cell.worldPosition, new Vector3(navigationCellSize.x * 0.9f, navigationCellSize.y * 0.9f, 0.05f));
        }
    }

    private class Cell
    {
        public Vector2Int position;
        public Vector3 worldPosition;
        public int fCost = int.MaxValue;
        public int gCost = int.MaxValue;
        public int hCost = int.MaxValue;
        public Vector2Int connection;
        public bool isWall;

        public Cell(Vector2Int pos, Vector3 worldPos)
        {
            position = pos;
            worldPosition = worldPos;
            connection = pos;
        }
    }
}