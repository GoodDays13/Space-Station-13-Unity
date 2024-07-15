using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector2Int = UnityEngine.Vector2Int;

public class AtmosphericSimulation : MonoBehaviour
{
    public Tilemap floor;
    public Tilemap walls;
    private int[,] roomNumbers; // -1 tile exposed to space, 0 inapplicable (space or wall), > 0 room
    private BoundsInt bounds;
    private uint[] tilesPerRoom;
    public RoomAtmosphere[] roomAtmospheres;

    // Start is called before the first frame update
    void Start () {
        //InitializeArray(floor, ref tilesFloor);
        //InitializeArray(walls, ref tilesWall);
        //atmosphere = new(floor, walls);
        roomAtmospheres = new RoomAtmosphere[255];
        roomAtmospheres[0] = new RoomAtmosphere(0f,float.PositiveInfinity,0f);
        //tilesFloor[]
        //floor = tilemap;
        //walls = wall;
        

        tilesPerRoom = new uint[255];
        //InitializeArray();
        bounds = floor.cellBounds;
        roomNumbers = new int[bounds.size.x, bounds.size.y];

        CalculateAll();
        //TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        //bottom = new(tilemap.cellBounds.x, tilemap.cellBounds.y);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public RoomAtmosphere GetRoomAtmosphere(Vector2Int cell)
    {
        int room = GetRoom(cell);
        return roomAtmospheres[room >= 0 ? room : 0];
    }
    
    private void CalculateAll()
    {
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                if (floor.GetTile(new(x + bounds.x, y + bounds.y)) &&
                    roomNumbers[x,y] == 0)
                    CalculateRoom(new(x + bounds.x, y + bounds.y));
            }
        }

    }

    public void CalculateRoom(Vector2Int cell)
    {
        int room = NextAvailableRoom();
        CalculateRoom(cell, room);
        roomAtmospheres[room] = new(tilesPerRoom[room], 1f, 293.15f);
    }

    // returns true if it was space
    private bool CalculateRoom(Vector2Int cell, int room)
    {
        if (!SetRoom(cell, room))
            return floor.GetTile(new(cell.x,cell.y,0)) == null &&
                   walls.GetTile(new(cell.x,cell.y,0)) == null;
        //if(!tilesPerRoom.Contains(room))
        //    tilesPerRoom.Add(room);
        //tilesPerRoom[room]++;

        foreach (var surround in SurroundingCoords(cell))
        {
            if (CalculateRoom(surround, room))
            {
                SetRoom(cell, -1);
                //tilesPerRoom[room]--;
                return true;
            }
        }
        return false;
    }

    private int NextAvailableRoom()
    {
        int room = 1;
        while (tilesPerRoom[room] != 0)
            room++;
        return room;
    }

    private Vector2Int[] SurroundingCoords(Vector2Int cell)
    {
        Vector2Int[] output = new Vector2Int[6];

        output[1] = new Vector2Int(cell.x + 1, cell.y);
        output[4] = new Vector2Int(cell.x - 1, cell.y);

        if (cell.y % 2 != 0)
            cell.x++;

        output[0] = new Vector2Int(cell.x, cell.y - 1);
        output[2] = new Vector2Int(cell.x, cell.y + 1);
        output[3] = new Vector2Int(cell.x - 1, cell.y - 1);
        output[5] = new Vector2Int(cell.x - 1, cell.y + 1);


        return output;
    }

    /// <summary>
    /// Gets the room that a cell is in.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns>Room ID for a cell. 0 if no room.</returns>
    public int GetRoom(Vector2Int cell)
    {
        cell.x -= bounds.x;
        cell.y -= bounds.y;
        if (cell.x < 0 || cell.y < 0)
            return 0;
        if (cell.x >= bounds.size.x || cell.y >= bounds.size.y)
            return 0;
        return roomNumbers[cell.x, cell.y];
    }

    /// <summary>
    /// Tries to change the room number of a tile.
    /// </summary>
    /// <param name="cell">Cell coords of the tile.</param>
    /// <param name="room">Room number to change it to.</param>
    /// <returns>True if it successfully changed the room number to a different one than it was.</returns>
    private bool SetRoom(Vector2Int cell, int room)
    {
        if (floor.GetTile(new(cell.x, cell.y, 0)) == null ||
            walls.GetTile(new(cell.x, cell.y, 0)) != null)
            return false;

        cell.x -= bounds.x;
        cell.y -= bounds.y;
        if (cell.x < 0 || cell.y < 0)
            return false;
        if (cell.x >= bounds.size.x || cell.y >= bounds.size.y)
            return false;

        var oldRoom = roomNumbers[cell.x, cell.y];
        if (oldRoom == room)
            return false;

        if (oldRoom > 0)
            tilesPerRoom[oldRoom]--;
        if (room > 0)
            tilesPerRoom[room]++;
        //tilesPerRoom.Remove(roomNumbers[cell.x, cell.y]);
        roomNumbers[cell.x, cell.y] = room;
        return true;
    }

    //}

    public struct RoomAtmosphere
    {
        /// <summary>
        /// Amount of moles in room.
        /// </summary>
        public float moles;
        /// <summary>
        /// Volume of room, in Liters.
        /// </summary>
        public float volume;
        /// <summary>
        /// Temperature of room, in Kelvin.
        /// </summary>
        public float temperature;

        private const float R = 0.082057f;
        /// <summary>
        /// Pressure of the room, in atmospheres. Calculated with the ideal gas formula.
        /// </summary>
        public readonly float Pressure => volume > 0 ? moles * R * temperature / volume : 0;

        public RoomAtmosphere(float moles, uint tiles, float temperature)
        {
            this.moles = moles;
            volume = tiles * 2000; // 2000 liters per tile
            this.temperature = temperature;
        }

        public RoomAtmosphere(uint tiles, float pressure, float temperature)
        {
            volume = tiles * 2000; // 2000 liters per tile
            this.temperature = temperature;
            moles = (pressure * volume) / (R * temperature);
        }

        public RoomAtmosphere(float moles, float volume, float temperature)
        {
            this.moles = moles;
            this.volume = volume;
            this.temperature = temperature;
        }

        public readonly override string ToString()
        {
            return $"{moles} moles, {volume} Liters, {temperature} K, {Pressure} atm";
        }
    }
}
