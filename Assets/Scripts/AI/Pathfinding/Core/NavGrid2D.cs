using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

using Custom.Utility;

namespace Custom.AI.Pathfinding
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class NavGrid2D : MonoBehaviour
    {
        private class NodeGraph
        {
            public int registered;
            public PathNode[] graph;
            public NavGridAgentBase agentClass;
        }



        public static event Action<NavGrid2D> OnNavGridUpdated;
        public static event Action<NavGrid2D> OnNodeGraphUpdated; 
        public static event Action<NavGridAgentBase> OnNewAgentRegistered;
        public static event Action<NavGridAgentBase> OnAgentUnregistered;



        // Static Baking
        [SerializeField] private LayerMask blockableLayers;
        [SerializeField] private GridGenerationMode gridGenerateMode;

        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Vector2 center;
        [SerializeField] private Vector2 size;
        [SerializeField] private Vector2Int cellCount;

        [SerializeField] private float minAngle = 0.0f;
        [SerializeField] private float maxAngle = 60.0f;

        [SerializeField] private int fixedFramesPerUpdate = 20;

        // Dynamic Obstacles
        [SerializeField] private BoxCollider2D obstacleDetectBounds;



        private bool isNavGridDirty = true;
        private bool isNodeGraphDirty = true;
        private int frameCounter;

        /*
         * Occupied cells are true, otherwise false.
         */
        private readonly Dictionary<Vector2Int, NavCellType> grid = new();

        /*
         * A "node graph" is defined by a collection of PathNodes.
         * A "node graph collection" contains node graphs for each of registered agents.
         * Each NavGrid2D have their own collections of node graphs.
         */
        private readonly Dictionary<int, NodeGraph> nodeGraphs = new();

        private readonly Dictionary<NavGridObstacle2D, BoundsInt> obstacles = new();



        public Vector2 CellSize
        {
            get
            {
                if (gridGenerateMode == GridGenerationMode.Tilemap && tilemap)
                    return tilemap.cellSize;
                else if (gridGenerateMode == GridGenerationMode.FreeBounds)
                    return size / cellCount;
                else
                    return Vector2.zero;
            }
        }

        public Vector2 Size
        {
            get
            {
                if (gridGenerateMode == GridGenerationMode.Tilemap && tilemap)
                    return tilemap.localBounds.size;
                else if (gridGenerateMode == GridGenerationMode.FreeBounds)
                    return size;
                else
                    return Vector2.zero;
            }
            set
            {
                if (size == value) return;

                size = value;

                if (gridGenerateMode == GridGenerationMode.FreeBounds)
                    isNavGridDirty = true;
            }
        }

        public Vector2 Center
        {
            get
            {
                if (gridGenerateMode == GridGenerationMode.Tilemap && tilemap)
                    return tilemap.localBounds.center;
                else if (gridGenerateMode == GridGenerationMode.FreeBounds)
                    return center;
                else
                    return Vector2.zero;
            }
            set
            {
                if (center == value) return;

                center = value;

                if (gridGenerateMode == GridGenerationMode.FreeBounds)
                    isNavGridDirty = true;
            }
        }
        public Vector2Int CellBounds
        {
            get
            {
                if (gridGenerateMode == GridGenerationMode.Tilemap && tilemap)
                    return (Vector2Int)tilemap.size;
                else if (gridGenerateMode == GridGenerationMode.FreeBounds)
                    return cellCount;
                else
                    return Vector2Int.zero;
            }
            set
            {
                if (cellCount == value) return;

                cellCount = value;

                if (gridGenerateMode == GridGenerationMode.FreeBounds)
                    isNavGridDirty = true;
            }
        }

        public Vector2 Min => Center - Size / 2.0f;

        public Vector2 Max => Center + Size / 2.0f;



#if UNITY_EDITOR
        private void Reset()
        {
            obstacleDetectBounds = GetComponent<BoxCollider2D>();
        }
#endif

        private void Awake()
        {
            if (!obstacleDetectBounds) obstacleDetectBounds = GetComponent<BoxCollider2D>();

            obstacleDetectBounds.isTrigger = true;

            var rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        private void Start()
        {
            BakeNavGrid();
            BakeNodeGraph();
        }

        private void FixedUpdate()
        {
            frameCounter++;

            if (frameCounter > fixedFramesPerUpdate)
            {
                if (isNavGridDirty)
                    BakeNavGrid();

                if (isNodeGraphDirty)
                    BakeNodeGraph();

                frameCounter = 0;
            }
        }

        private void OnTriggerEnter2D(Collider2D _collision)
        {
            if (!_collision.TryGetComponent(out NavGridObstacle2D asObstacle)) return;

            if (obstacles.ContainsKey(asObstacle)) return;

            obstacles.Add(asObstacle, new BoundsInt());

            asObstacle.OnUpdated += Carve;
            asObstacle.OnDestroyed += OnDestroyed;
        }

        private void OnTriggerExit2D(Collider2D _collision)
        {
            if (!_collision.TryGetComponent(out NavGridObstacle2D asObstacle)) return;

            obstacles.Remove(asObstacle);

            asObstacle.OnUpdated -= Carve;
            asObstacle.OnDestroyed -= OnDestroyed;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (var node in grid)
            {
                switch (node.Value)
                {
                    case NavCellType.Empty:
                        Gizmos.color = Color.green;
                        break;

                    case NavCellType.Flat:
                        Gizmos.color = Color.red;
                        break;

                    case NavCellType.OneWay:
                        Gizmos.color = Color.yellow;
                        break;

                    case NavCellType.Slope:
                        Gizmos.color = Color.cyan;
                        break;

                    case NavCellType.OneWaySlope:
                        Gizmos.color = Color.magenta;
                        break;

                    default:
                        Gizmos.color = Color.white;
                        break;
                }

                Gizmos.DrawWireCube(CellToWorld(node.Key).Value, CellSize * 0.95f);
            }
        }
#endif



        #region Static Baking
        private void BakeNavGrid()
        {
            if (tilemap) tilemap.CompressBounds();

            // Generate node grid.
            Vector2Int cellPos;

            for (int x = 0; x < CellBounds.x; x++)
            {
                for (int y = 0; y < CellBounds.y; y++)
                {
                    cellPos = new(x, y);

                    grid[cellPos] = GenerateCellTypeAt(CellToWorld(cellPos).Value);
                }
            }

            // Update boundaries.
            obstacleDetectBounds.offset = Center - (Vector2)transform.position;
            obstacleDetectBounds.size = Size;

            isNavGridDirty = false;

            OnNavGridUpdated?.Invoke(this);
        }

        private void BakeNodeGraph()
        {
            // Re-bake registered agents' node graph.
            foreach (var graph in nodeGraphs)
            {
                nodeGraphs[graph.Key].graph = graph.Value.agentClass.ConnectGraphNodes(graph.Value.agentClass.GenerateGraphNodes(this));
            }

            isNodeGraphDirty = false;

            OnNodeGraphUpdated?.Invoke(this);
        }



        private NavCellType GenerateCellTypeAt(Vector2 _worldLocation)
        {
            // Sort cell type.
            // When defining new cell types, manual definition must be sorted here.
            RaycastHit2D[] hitResult = Physics2D.RaycastAll(_worldLocation + 0.475f * CellSize.y * Vector2.up, Vector2.down, CellSize.y * 0.95f, blockableLayers);

            if (hitResult.Length == 0)
            {
                return NavCellType.Empty;
            }
            else
            {
                bool allTrigger = true;
                bool allEffector = true;
                bool allSlope = true;
                float zRotation;

                foreach (var hit in hitResult)
                {
                    if (allTrigger && !hit.collider.isTrigger) allTrigger = false;
                    if (allEffector && !hit.collider.usedByEffector) allEffector = false;

                    zRotation = Mathf.Abs(Vector2.Angle(Vector2.up, hit.normal));
                    zRotation = Mathf.Min(zRotation, 360 - zRotation);
                    if (allSlope && (zRotation <= minAngle || zRotation > maxAngle)) allSlope = false; 
                }

                if (allTrigger)
                    return NavCellType.Empty;
                else if (allEffector)
                    return allSlope ? NavCellType.OneWaySlope : NavCellType.OneWay;
                else if (allSlope)
                    return NavCellType.Slope;
                else 
                    return NavCellType.Flat;
            }
        }
        #endregion

        #region Obstacle Handling
        private void Carve(NavGridObstacle2D _obstacle)
        {
            // Un-carve old bounds.
            if (obstacles.ContainsKey(_obstacle))
                UpdateNavGridInBounds(obstacles[_obstacle]);

            // Carve new bounds.
            BoundsInt? cellBounds = WorldToCell(_obstacle.Bounds);
            if (!cellBounds.HasValue) return;

            obstacles[_obstacle] = cellBounds.Value;
            UpdateNavGridInBounds(cellBounds.Value);
        }

        private void UpdateNavGridInBounds(BoundsInt _boundsInt)
        {
            Vector2Int cellPos;
            for (int x = _boundsInt.xMin; x <= _boundsInt.xMax; x++)
            {
                for (int y = _boundsInt.yMin; y <= _boundsInt.yMax; y++)
                {
                    cellPos = new(x, y);

                    var newCellType = GenerateCellTypeAt(CellToWorld(cellPos).Value);

                    if (newCellType != grid[cellPos])
                    {
                        isNodeGraphDirty = true;
                        grid[cellPos] = newCellType;
                    }
                }
            }
        }



        private void OnDestroyed(NavGridObstacle2D _obstacle)
        {
            obstacles.Remove(_obstacle);

            _obstacle.OnUpdated -= Carve;
            _obstacle.OnDestroyed -= OnDestroyed;
        }
        #endregion

        #region Pathfinding
        /// <summary>
        /// Register a <see cref="NavGridAgentBase"/> to this nav grid. <br/>
        /// Node graph of this agent will automatically be updated after baking. <br/>
        /// <b>NOTE:</b> Agents with the same type and <see cref="NavGridAgentBase.agentData"/> will be considered the same agent.
        /// </summary>
        /// <param name="_agent"> The <see cref="NavGridAgentBase"/> to register. </param>
        /// <returns>
        /// <see langword="true"/> if a new agent is registered. Otherwise, <see langword="false"/>.
        /// </returns>
        public bool RegisterAgent(NavGridAgentBase _agent)
        {
            int key = _agent.GetHashCode();
            if (nodeGraphs.ContainsKey(key))
            {
                nodeGraphs[key].registered++;
            }
            else
            {
                nodeGraphs.Add(key, new NodeGraph()
                {
                    registered = 1,
                    agentClass = _agent,
                    graph = _agent.ConnectGraphNodes(_agent.GenerateGraphNodes(this))
                });

                OnNavGridUpdated?.Invoke(this);
            }

            return true;
        }

        /// <summary>
        /// Unregister a <see cref="NavGridAgentBase"/> from this nav grid. <br/>
        /// </summary>
        /// <param name="_agent"> The <see cref="NavGridAgentBase"/> to unregister. </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="_agent"/> is already registered. Otherwise, <see langword="false"/>.
        /// </returns>
        public bool UnregisterAgent(NavGridAgentBase _agent)
        {
            int key = _agent.GetHashCode();
            if (!nodeGraphs.ContainsKey(key)) return false;

            nodeGraphs[key].registered--;

            if (nodeGraphs[key].registered == 0)
                nodeGraphs.Remove(key);

            return true;
        }

        /// <summary>
        /// Get generated node graph of <paramref name="_agent"/>.
        /// </summary>
        /// <param name="_agent"> The <see cref="NavGridAgentBase"/> to retrieve graph from. </param>
        /// <returns>
        /// If <paramref name="_agent"/> is registered, returns calculated node graph.
        /// Otherwise, returns empty array.
        /// </returns>
        public PathNode[] GetAgentNodeGraph(NavGridAgentBase _agent)
        {
            int key = _agent.GetHashCode();
            if (!nodeGraphs.ContainsKey(key)) return new PathNode[0];

            return nodeGraphs[key].graph;
        }
        #endregion

        #region Grid Utilities
        /// <summary>
        /// Check if grid contains cell at the given location.
        /// </summary>
        /// <param name="_cellLocation"> Location to check for in cell space. </param>
        /// <returns>
        /// <see langword="true"/> if the given location is within bounds, Otherwise, <see langword="false"/>.
        /// </returns>
        public bool Contains(Vector2Int _cellLocation)
        {
            return (_cellLocation.x >= 0) && (_cellLocation.x < CellBounds.x)
                && (_cellLocation.y >= 0) && (_cellLocation.y < CellBounds.y);
        }

        /// <inheritdoc cref="Contains(Vector2Int)"/>
        /// <param name="_worldLocation"> Location to check for in world space. </param>
        public bool Contains(Vector2 _worldLocation)
        {
            return (_worldLocation.x > Min.x) && (_worldLocation.x < Max.x)
                && (_worldLocation.y > Min.y) && (_worldLocation.y < Max.y);
        }



        /// <summary>
        /// Return the generated <see cref="NavCellType"/> at the given location.
        /// </summary>
        /// <param name="_cellLocation">    The location in cell space. </param>
        /// <returns>
        /// The cell type at the given position if found. Otherwise, return <see cref="NavCellType.None"/>.
        /// </returns>
        public NavCellType GetCellTypeAt(Vector2Int _cellLocation)
        {
            if (!Contains(_cellLocation)) return NavCellType.None;

            return grid[_cellLocation];
        }

        /// <inheritdoc cref="Contains(Vector2Int)"/>
        /// <param name="_cellLocation">    The location in world space. </param>
        public NavCellType GetCellTypeAt(Vector2 _worldLocation)
        {
            if (!Contains(_worldLocation)) return NavCellType.None;

            return grid[WorldToCell(_worldLocation)];
        }



        /// <summary>
        /// Check if cell at the given location is occupied or not.
        /// </summary>
        /// <param name="_cellLocation"> Location to check for in cell space. </param>
        /// <returns>
        /// <see langword="true"/> if cell is occupied, Otherwise, <see langword="false"/>. <br/>
        /// <b>NOTE:</b> If cell location is out of bounds, returns <see langword="false"/>.
        /// </returns>
        public bool Occupied(Vector2Int _cellLocation)
        {
            if (!Contains(_cellLocation)) return false;

            return grid[_cellLocation] != NavCellType.Empty;
        }

        /// <param name="_worldLocation"> Location to check for in world space. </param>
        /// <inheritdoc cref="Occupied(Vector2)"/>
        public bool Occupied(Vector2 _worldLocation)
        {
            if (!Contains(_worldLocation)) return false;

            return grid[WorldToCell(_worldLocation)] != NavCellType.Empty;
        }


        /// <summary>
        /// Check if a bounding box around at the given location is occupied or not.
        /// </summary>
        /// <param name="_extents"> The extents in both directions to check for. </param>
        /// <inheritdoc cref="Occupied(Vector2Int)"/>
        public bool Occupied(Vector2 _worldLocation, Vector2 _extents)
        {
            Vector2Int min = WorldToCell(_worldLocation - _extents);
            Vector2Int max = WorldToCell(_worldLocation + _extents);

            for (int y = min.y; y <= max.y; y++)
                for (int x = min.x; x <= max.x; x++)
                    if (Occupied(new(x, y))) return true;

            return false;
        }

        /// <inheritdoc cref="Occupied(Vector2Int)"/>
        /// <param name="_worldLocations"> A list of cell locations to check for. </param>
        /// <returns>
        /// <see langword="true"/> if any cell is occupied, Otherwise, <see langword="false"/>. <br/>
        /// </returns>
        public bool OccupiedCells(Vector2Int[] _cellLocations)
        {
            foreach (var location in _cellLocations)
            {
                if (!Contains(location)) continue;

                if (grid[location] != NavCellType.Empty) return true;
            }

            return false;
        }



        /// <summary>
        /// Check if cells between <paramref name="_start"/> cell and <paramref name="_end"/> are walkable.
        /// </summary>
        /// <param name="_start">   The starting point of the line which the algorithm checks along. </param>
        /// <param name="_end">     The end point of the line which the algorithm checks along. </param>
        /// <returns>
        /// <see langword="true"/> if any cell are occupied, Otherwise, <see langword="false"/>.
        /// </returns>
        public bool OccupiedFromTo(Vector2Int _start, Vector2Int _end)
        {
            foreach (var cell in LineToCells(_start, _end))
            {
                if (Occupied(cell)) return true;
            }

            return false;
        }

        /// <summary>
        /// Get the first cell occupied along a given line.
        /// </summary>
        /// <param name="_start">           The starting point of the line which the algorithm checks along. </param>
        /// <param name="_end">             The end point of the line which the algorithm checks along. </param>
        /// <param name="_result">          <b>OUT:</b> The cell position of the first cell occupied. </param>
        /// <param name="_includeStart">    If true, results that equals to <paramref name="_start"/> will be false. 
        ///                                 Otherwise, included as normal. </param>
        /// <returns>
        /// <see langword="true"/> if a cell was found. Otherwise, <see langword="false"/>.
        /// </returns>
        public bool GetFirstOccupied(Vector2Int _start, Vector2Int _end, out Vector2Int _result, bool _includeStart = false)
        {
            _result = _start;

            foreach (var cell in LineToCells(_start, _end))
            {
                if (!Occupied(cell)) continue;

                _result = cell;
                return _includeStart || _result != _start;
            }

            return false;
        }



        /// <summary>
        /// Converts a world-space <see cref="Bounds"/> to a tilemap-based <see cref="BoundsInt"/>, if valid.
        /// </summary>
        /// <param name="_worldBounds"> The world-space bounds to convert. </param>
        /// <returns>
        /// A <see cref="BoundsInt"/> representing the tilemap space covered by the world bounds,  
        /// or <see langword="null"/> if the conversion is invalid.  
        /// </returns>
        public BoundsInt? WorldToCell(Bounds _worldBounds)
        {
            Vector2Int min = WorldToCell(_worldBounds.min);
            Vector2Int max = WorldToCell(_worldBounds.max);

            if (min == new Vector2(-1, -1) || max == new Vector2(-1, -1)) return null;

            Vector2Int diff = max - min;

            return new BoundsInt(
                min.x, min.y, 0,
                diff.x, diff.y, 0);
        }

        /// <summary>
        /// Get the cell location at <paramref name="_worldLocation"/>.
        /// </summary>
        /// <param name="_worldLocation"> Location in world space. </param>
        /// <returns>
        /// If a cell is found, returns the cell location. If not, returns (-1; -1).
        /// </returns>
        public Vector2Int WorldToCell(Vector2 _worldLocation)
        {
            if (Contains(_worldLocation))
                return Vector2Int.FloorToInt((_worldLocation - Min) / CellSize);
            else
                return new Vector2Int(-1, -1);
        }

        /// <summary>
        /// Get the world location at <paramref name="_cellLocation"/>.
        /// </summary>
        /// <param name="_cellLocation"> Location in cell space. </param>
        /// <returns>
        /// If the given location is within bounds, returns the center of the cell in world space. <br/>
        /// Otherwise, returns <see langword="null"/>.
        /// </returns>
        public Vector2? CellToWorld(Vector2Int _cellLocation)
        {
            if (!Contains(_cellLocation))
                return null;
            else
                return _cellLocation * CellSize + CellSize / 2.0f - Size / 2.0f + Center;
        }



        /// <summary>
        /// Flooring distance from world space unit to cell unit.
        /// </summary>
        /// <param name="_distance">    Distance in world space unit. </param>
        /// <param name="_swizzle">     If true, use CellSize.y. Otherwise, use CellSize.x </param>
        /// <returns>
        /// Rounded cell unit.
        /// </returns>
        public int FloorToCell(float _distance, bool _swizzle = false)
        {
            return Mathf.FloorToInt(_distance / (_swizzle ? CellSize.y : CellSize.x));
        }

        /// <summary>
        /// Ceiling distance from world space unit to cell unit.
        /// </summary>
        /// <inheritdoc cref="FloorToCell(float, bool)"/>
        public int CeilToCell(float _distance, bool _swizzle = false)
        {
            return Mathf.CeilToInt(_distance / (_swizzle ? CellSize.y : CellSize.x));
        }



        /// <summary>
        /// Convert world space line to cells in grid.
        /// </summary>
        /// <param name="_start">   The starting point of the line which the algorithm checks along. <br/>
        ///                         Out of bounds value will be clamped to bounds. </param>
        /// <param name="_end">     The end point of the line which the algorithm checks along. <br/>
        ///                         Out of bounds value will be clamped to bounds. </param>
        /// <returns>
        /// An array of cells location within the grid.
        /// </returns>
        public Vector2Int[] LineToCells(Vector2 _start, Vector2 _end)
        {
            List<Vector2Int> cells = new();

            _start -= Min;
            _end -= Min;

            int x0 = Mathf.FloorToInt(_start.x / CellSize.x);
            int y0 = Mathf.FloorToInt(_start.y / CellSize.x);
            int x1 = Mathf.FloorToInt(_end.x / CellSize.y);
            int y1 = Mathf.FloorToInt(_end.y / CellSize.y);

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = (x1 > x0) ? 1 : -1;
            int sy = (y1 > y0) ? 1 : -1;
            float deltaX = (dx == 0) ? float.MaxValue : (CellSize.x / Mathf.Abs(_end.x - _start.x));
            float deltaY = (dy == 0) ? float.MaxValue : (CellSize.y / Mathf.Abs(_end.y - _start.y));
            float tMaxX = (x1 > x0) ? ((x0 + 1) * CellSize.x - _start.x) * deltaX : (_start.x - x0 * CellSize.x) * deltaX;
            float tMaxY = (y1 > y0) ? ((y0 + 1) * CellSize.y - _start.y) * deltaY : (_start.y - y0 * CellSize.y) * deltaY;
            int steps = dx + dy;

            for (int i = 0; i <= steps; i++)
            {
                cells.Add(new Vector2Int(x0, y0));

                if (tMaxX < tMaxY)
                {
                    x0 += sx;
                    tMaxX += deltaX;
                }
                else
                {
                    y0 += sy;
                    tMaxY += deltaY;
                }
            }

            return cells.ToArray();
        }

        /// <summary>
        /// Convert world space line from cell locations to cells in grid.
        /// </summary>
        /// <inheritdoc cref="LineToCells(Vector2, Vector2)"/>
        public Vector2Int[] LineToCells(Vector2Int _start, Vector2Int _end)
        {
            _start.Clamp(Vector2Int.zero, CellBounds - Vector2Int.one);
            _end.Clamp(Vector2Int.zero, CellBounds - Vector2Int.one);

            return LineToCells(CellToWorld(_start).Value, CellToWorld(_end).Value);
        }



        /// <summary>
        /// Convert world space cubic bezier curve to cells in grid.
        /// </summary>
        /// <param name="_p0">      Start point of bezier curve. </param>
        /// <param name="_p1">      Start tangent of bezier curve. </param>
        /// <param name="_p2">      End point of bezier curve. </param>
        /// <param name="_p3">      End tangent of bezier curve. </param>
        /// <param name="_step">    Normalized distance to move along the curve each iteration. <br/>
        ///                         <b>NOTE:</b> Small step size will drastically decreases performance, use with caution. </param>
        /// <returns>
        /// An array of cells location within the grid.
        /// </returns>
        public Vector2Int[] CubicBezierToCells(Vector2 _p0, Vector2 _p1, Vector2 _p2, Vector2 _p3, float _step = 0.1f)
        {
            HashSet<Vector2Int> visitedCells = new();

            Vector2 prevPoint = _p0;
            visitedCells.Add(WorldToCell(prevPoint));

            // Sample points along the bezier curve.
            for (float t = _step; t <= 1.0; t += _step)
            {
                Vector2 currentPoint = BezierUtil.CubicBezier(_p0, _p1, _p2, _p3, t);
                visitedCells.UnionWith(LineToCells(prevPoint, currentPoint));
                prevPoint = currentPoint;
            }

            return visitedCells.ToArray();
        }

        /// <summary>
        /// Convert world space quadratic bezier curve to cells in grid.
        /// </summary>
        /// <param name="_p0">      The starting control point. </param>
        /// <param name="_p1">      The middle control point. </param>
        /// <param name="_p2">      The ending control point. </param>
        /// <param name="_step">    Normalized distance to move along the curve each iteration. <br/>
        ///                         <b>NOTE:</b> Small step size will drastically decreases performance, use with caution. </param>
        /// <returns>
        /// An array of cells location within the grid.
        /// </returns>
        public Vector2Int[] QuadraticBezierToCells(Vector2 _p0, Vector2 _p1, Vector2 _p2, float _step = 0.1f)
        {
            HashSet<Vector2Int> visitedCells = new();

            Vector2 prevPoint = _p0;
            visitedCells.Add(WorldToCell(prevPoint));

            // Sample points along the bezier curve.
            for (float t = _step; t <= 1.0; t += _step)
            {
                Vector2 currentPoint = BezierUtil.QuadraticBezier(_p0, _p1, _p2, t);
                visitedCells.UnionWith(LineToCells(prevPoint, currentPoint));
                prevPoint = currentPoint;
            }

            return visitedCells.ToArray();
        }
        #endregion
    }



    /// <summary>
    /// How should the nav grid bake its grid.
    /// </summary>
    public enum GridGenerationMode
    {
        /// <summary>
        /// Using tile map properties to define cell grid.
        /// </summary>
        Tilemap,

        /// <summary>
        /// Using free bound values to define cell grid.
        /// </summary>
        FreeBounds,
    }



    /// <summary>
    /// Defined types for different cells.
    /// </summary>
    public enum NavCellType
    {
        /// <summary>
        /// This should never be used. <br/>
        /// Return value of nav grid functions when a cell is not found.
        /// </summary>
        None,

        /// <summary>
        /// A cell that is not overlapped with any collider in blockable layers of the nav grid.
        /// </summary>
        Empty,

        /// <summary>
        /// A cell that is overlapped with a collider in blockable layers of the nav grid.
        /// </summary>
        Flat,

        /// <summary>
        /// A cell that is overlapped with a rotated collider in blockable layers of the nav grid.
        /// </summary>
        Slope,

        /// <summary>
        /// A cell that is overlapped with a collider using a platform effector in blockable layers of the nav grid.
        /// </summary>
        OneWay,

        /// <summary>
        /// A cell that is overlapped with a rotated collider using a platform effector in blockable layers of the nav grid.
        /// </summary>
        OneWaySlope,
    }
}
