using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false; // responsible to check if random walk room want to be use or square room

    public Vector2Int spawnPosition; // Store spawn 

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithims.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int
            (dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRandomRooms(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }


        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTile(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        // Find a suitable spawn point within the generated dungeon
        spawnPosition = FindValidSpawnPoint(floor);
    }

    private Vector2Int FindValidSpawnPoint(HashSet<Vector2Int> floorPositions)
    {
        // Example logic to find a random valid spawn point from floor positions
        // Ensure the spawn position is within the dungeon and not on a wall
        if (floorPositions.Count > 0)
        {
            // Convert floor positions to a list and pick a random position
            List<Vector2Int> floorList = new List<Vector2Int>(floorPositions);
            return floorList[Random.Range(0, floorList.Count)];
        }
        return Vector2Int.zero; // Default to (0,0) if no valid position
    }

    private HashSet<Vector2Int> CreateRandomRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int> ();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin + offset)
                    &&  position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination, int corridorWidth = 2)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        int maxCorridorLength = 1000; // Limits the corridor length to prevent memory consumption
        int steps = 0;

        while (position.y != destination.y && steps < maxCorridorLength)
        {
            steps++;
            if (destination.y > position.y)
                position += Vector2Int.up;
            else if (destination.y < position.y)
                position += Vector2Int.down;
            corridor.Add(position);
            AddCorridorWidth(corridor, position, corridorWidth, isVertical: true);
        }

        steps = 0;
        while (position.x != destination.x && steps < maxCorridorLength)
        {
            steps++;
            if(destination.x > position.x)
                position += Vector2Int.right;
            else if (destination.x < position.x)
                position += Vector2Int.left;
            corridor.Add(position);
            AddCorridorWidth(corridor, position, corridorWidth, isVertical: false);
        }
        return corridor;
    }

    private void AddCorridorWidth(HashSet<Vector2Int> corridor, Vector2Int position, int width, bool isVertical)
    {
        for (int i = 1; i < width; i++)
        {
            if (isVertical)
            {
                // Extend corridor horizontally for vertical movement
                corridor.Add(position + Vector2Int.right * i);
                corridor.Add(position + Vector2Int.left * i);
            }
            else
            {
                // Extend corridor vertically for horizontal movement
                corridor.Add(position + Vector2Int.up * i);
                corridor.Add(position + Vector2Int.down * i);
            }
        }
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = 0; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
