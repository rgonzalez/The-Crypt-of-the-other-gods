using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomDistance
{
    public float distance = 0f;
    public Room room;
}

public class LevelBuilder : MonoBehaviour
{

    public int keysToFinish = 1; //the number of keys to finish
	public Room startRoomPrefab, endRoomPrefab;
	public List<Room> roomPrefabs = new List<Room> ();
	public Vector2 iterationRange = new Vector2 (3, 10);
	public MovePlayer playerPrefab;    

    //CONFIGURATION FOR START GAME
    public InventoryManager inventoryManager;
    public Canvas gameCanvas;
    public UIManager uiManager;
    public ShopMenuScript shopCameraManager;
    public WeaponSpawnManager weaponManager;
	List<Doorway> availableDoorways = new List<Doorway> ();
    public string nextLevel = ""; //the name of the next Level

    public static LevelBuilder instance;


    StartRoom startRoom;
	EndRoom endRoom;
	List<Room> placedRooms = new List<Room> ();

	LayerMask roomLayerMask;

    MovePlayer player;

    int pickedKeys = 0;


	void Start ()
	{
        LevelBuilder.instance = this; //each level smash the instance
		roomLayerMask = LayerMask.GetMask ("Room");
		StartCoroutine ("GenerateLevel");
	}

	IEnumerator GenerateLevel ()
	{
		WaitForSeconds startup = new WaitForSeconds (1);
        // WaitForFixedUpdate interval = new WaitForFixedUpdate ();
        WaitForEndOfFrame interval = new WaitForEndOfFrame();

        yield return startup;

		// Place start room
		PlaceStartRoom ();
		yield return interval;

		// Random iterations
		int iterations = Random.Range ((int)iterationRange.x, (int)iterationRange.y);

		for (int i = 0; i < iterations; i++) {
			// Place random room from list
			bool placed = PlaceRoom ();
            yield return interval;
           
		}

		// Place end room
		PlaceEndRoom ();
		yield return interval;

		// Level generation finished
		Debug.Log ("Level generation finished");

        // Place player
        endRoom.nextLevel = this.nextLevel;

        //generate all NavMesh
        foreach(Room room in placedRooms)
        {
            // room.BuildNavMesh(); // the floors has surfaceNavMesh, but now the floors has NavMeshSourceTag
            //is more efficient for the routes
            room.CleanDoorWays();
            room.gameObject.SetActive(false);
        }
        //reactivate the startRoom
        startRoom.gameObject.SetActive(true);
        PlaceKey();
        StartCoroutine(StartGame());
        //regeneration for testing
        //yield return new WaitForSeconds (3);
        //ResetLevelGenerator ();
    }

	void PlaceStartRoom ()
	{
		// Instantiate room
		startRoom = Instantiate (startRoomPrefab) as StartRoom;
		startRoom.transform.parent = this.transform;

		// Get doorways from current room and add them randomly to the list of available doorways
		AddDoorwaysToList (startRoom, ref availableDoorways);

		// Position room
		startRoom.transform.position = Vector3.zero;
		startRoom.transform.rotation = Quaternion.identity;
        ClearWalls(startRoom);
	}

	void AddDoorwaysToList (Room room, ref List<Doorway> list)
	{
		foreach (Doorway doorway in room.doorways) {
			int r = Random.Range (0, list.Count);
			list.Insert (r, doorway);
		}
	}

    /// <summary>
    /// place a room if can be placed
    /// </summary>
    /// <returns>TRUE if placed, FALSE if there is not available rooms to place</returns>
	bool PlaceRoom ()
	{
        List<int> usedRooms = new List<int>(); // list of used rooms to dont pick again, keep the list of index used this iteration
        List<int> availableIndexes = new List<int>(); // is a list of index (in the roomPrefabs) to USE
        bool roomPlaced = false;
        bool finishedPlacing = false;
        for (int i = 0; i < roomPrefabs.Count; i++)
        {
            availableIndexes.Add(i);
        }
        while (availableIndexes.Count > 0)
        {
           
            int actualIndex = Random.Range(0, availableIndexes.Count);
            int actualRoomPrefabIndex = availableIndexes[actualIndex]; // initially the array can be 0,1,2,3,4,5,6... but when the rooms are being extracted, will be [0,4,6,7] for example
                                                                        // so if in an array [0,4,6,7] we get random 2, is 4 in the array (the 2 was wrong placed before..)

            Room currentRoom = Instantiate(roomPrefabs[actualRoomPrefabIndex]) as Room;
            currentRoom.transform.parent = this.transform;

            // Create doorway lists to loop over
            List<Doorway> allAvailableDoorways = new List<Doorway>(availableDoorways);
            List<Doorway> currentRoomDoorways = new List<Doorway>();
            AddDoorwaysToList(currentRoom, ref currentRoomDoorways);

            // Get doorways from current room and add them randomly to the list of available doorways
            // AddDoorwaysToList(currentRoom, ref availableDoorways);
            Doorway doorWayconnected = null;
            // Try all available doorways
            foreach (Doorway availableDoorway in allAvailableDoorways)
            {
                // Try all available doorways in current room
                foreach (Doorway currentDoorway in currentRoomDoorways)
                {
                    // Position room
                    PositionRoomAtDoorway(ref currentRoom, currentDoorway, availableDoorway);

                    // Check room overlaps
                    if (CheckRoomOverlap(currentRoom))
                    {
                        //restart if overlap
                        continue;
                    }

                    roomPlaced = true;

                    // Add room to list

                    ClearWalls(currentRoom);
                    placedRooms.Add(currentRoom);
                    doorWayconnected = currentDoorway;
                    //connect the list of rooms between them
                    Room availableRoom = availableDoorway.transform.GetComponentInParent<Room>();
                    if (availableRoom != null)
                    {
                        currentRoom.connectedRooms.Add(availableRoom);
                        availableRoom.connectedRooms.Add(currentRoom);
                    }
                    availableDoorway.gameObject.SetActive(false);
                    availableDoorways.Remove(availableDoorway);
                    // Exit loop if room has been placed
                    break;
                }

                // Exit loop if room has been placed
                if (roomPlaced)
                {
                    // Get doorways from current room and add them randomly to the list of available doorways
                    AddDoorwaysToList(currentRoom, ref availableDoorways);
                    // Remove occupied doorways
                    doorWayconnected.gameObject.SetActive(false);
                    availableDoorways.Remove(doorWayconnected);
                    return true;                       
                }
            }


            // Room couldn't be placed. Restart generator and try again
            if (!roomPlaced)
            {
                Destroy(currentRoom.gameObject);
                usedRooms.Add(actualRoomPrefabIndex);
                availableIndexes.RemoveAt(actualIndex);
                //ResetLevelGenerator ();
            }
        }
        if (!roomPlaced)
        {
            ResetLevelGenerator();
        }
        return false;
    }

    //clear the walls so the user can see the walls
    void ClearWalls(Room room)
    {
        room.ClearWalls();
    }


    void PositionRoomAtDoorway (ref Room room, Doorway roomDoorway, Doorway targetDoorway)
	{
		// Reset room position and rotation
		room.transform.position = Vector3.zero;
		room.transform.rotation = Quaternion.identity;

		// Rotate room to match previous doorway orientation
		Vector3 targetDoorwayEuler = targetDoorway.transform.eulerAngles;
		Vector3 roomDoorwayEuler = roomDoorway.transform.eulerAngles;
		float deltaAngle = Mathf.DeltaAngle (roomDoorwayEuler.y, targetDoorwayEuler.y);
		Quaternion currentRoomTargetRotation = Quaternion.AngleAxis (deltaAngle, Vector3.up);
		room.transform.rotation = currentRoomTargetRotation * Quaternion.Euler (0, 180f, 0);

		// Position room
		Vector3 roomPositionOffset = roomDoorway.transform.position - room.transform.position;
		room.transform.position = targetDoorway.transform.position - roomPositionOffset;
	}

    /// <summary>
    /// Check if a room is overlaping with other rooms, extract the bounds of the room (with all Gameobjects), and use overlap
    /// </summary>
    /// <param name="room"></param>
    /// <returns>TRUE if overlap, FALSE if is a OK room</returns>
	bool CheckRoomOverlap (Room room)
	{
		Bounds bounds = room.RoomBounds;
		bounds.Expand (-0.1f);

		Collider[] colliders = Physics.OverlapBox (bounds.center, bounds.size / 2, room.transform.rotation, roomLayerMask);
		if (colliders.Length > 0) {
			// Ignore collisions with current room
			foreach (Collider c in colliders) {
				if (c.transform.parent.gameObject.Equals (room.gameObject)) {
					continue;
				} else {
					Debug.LogError ("Overlap detected");
					return true;
				}
			}
		}

		return false;
	}

	void PlaceEndRoom ()
	{
		// Instantiate room
		endRoom = Instantiate (endRoomPrefab) as EndRoom;
		endRoom.transform.parent = this.transform;

		// Create doorway lists to loop over
		List<Doorway> allAvailableDoorways = new List<Doorway> (availableDoorways);
		Doorway doorway = endRoom.doorways [0];

		bool roomPlaced = false;

		// Try all available doorways
		foreach (Doorway availableDoorway in allAvailableDoorways) {
			// Position room
			Room room = (Room)endRoom;
			PositionRoomAtDoorway (ref room, doorway, availableDoorway);

			// Check room overlaps
			if (CheckRoomOverlap (endRoom)) {
                // if the checkroomOverlap returns true, exit the loop and restart the system
				continue;
			}

			roomPlaced = true;

            ClearWalls(room);
            // Remove occupied doorways
            doorway.gameObject.SetActive (false);
			availableDoorways.Remove (doorway);

			availableDoorway.gameObject.SetActive (false);
			availableDoorways.Remove (availableDoorway);

			// Exit loop if room has been placed
			break;
		}

		// Room couldn't be placed. Restart generator and try again
		if (!roomPlaced) {
			ResetLevelGenerator ();
		}
	}

	void ResetLevelGenerator ()
	{
		Debug.LogError ("Reset level generator");

		StopCoroutine ("GenerateLevel");

		// Delete all rooms
		if (startRoom) {
			Destroy (startRoom.gameObject);
		}

		if (endRoom) {
			Destroy (endRoom.gameObject);
		}

		foreach (Room room in placedRooms) {
			Destroy (room.gameObject);
		}

		// Clear lists
		placedRooms.Clear ();
		availableDoorways.Clear ();

		// Reset coroutine
		StartCoroutine ("GenerateLevel");
	}

    //to finish the level, a key must be found, so place the key in the most far (from start) room
    void PlaceKey()
    {
        // we calculate the distances into a new array to just calculate once (not in a sorter system..)
        List<RoomDistance> distances = new List<RoomDistance>();
        foreach(Room room in placedRooms)
        {
            RoomDistance distance = new RoomDistance();
            distance.room = room;
            distance.distance = Vector3.Distance(startRoom.gameObject.transform.position, room.gameObject.transform.position);
            distances.Add(distance);
        }
        distances.Sort(SortByDistance);
        //now we have the most far room in the [0]
        distances[0].room.PlaceKey();
        //clear from the list this room (not put 2 keys in the same room)
        distances.RemoveAt(0);
        //if max key to unlock is bigger than 1, place more keys
        if (keysToFinish > 1) {
            int keysPlaced = 1;
            while (keysPlaced < keysToFinish)
            {
                if (distances.Count > 0)
                {
                    int pos = Random.Range(0, distances.Count - 1);
                    RoomDistance room = distances[pos];
                    //check not place a key in the start or end room!!
                    if ((room.GetType() == typeof(StartRoom)) || (room.GetType() == typeof(EndRoom)))
                    {
                        // is a start or end room, just delete it
                        distances.RemoveAt(pos);
                    }
                    else
                    {
                        room.room.PlaceKey();
                        keysPlaced++;
                        distances.RemoveAt(pos);
                    }
                } else
                {
                    keysPlaced = keysToFinish;
                }
            }
        }
    }

    /// <summary>
    /// sort from most far distance to nearest
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    int SortByDistance(RoomDistance a, RoomDistance b)
    {
        if (a.distance > 0 && b.distance > 0)
        {
            if (a.distance > b.distance)
            {
                return -1;
            } else if (a.distance < b.distance)
            {
                return 1;
            }
        }
        return 0;
    }

    IEnumerator StartGame()
    {
        //Maybe the player is already in the game (from previous scene with DontDestroy..)
        GameObject prevPlayer = GameObject.FindGameObjectWithTag(Constants.TAG_PLAYER);
        if (prevPlayer)
        {
            prevPlayer.transform.position = startRoom.playerStart.position;
            prevPlayer.transform.rotation = startRoom.playerStart.rotation;
        }
        else if (playerPrefab)
        {
            player = Instantiate(playerPrefab) as MovePlayer;
            player.transform.position = startRoom.playerStart.position;
            player.transform.rotation = startRoom.playerStart.rotation;
        }
        yield return new WaitForEndOfFrame();

        if (weaponManager)
            weaponManager.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();

        if (inventoryManager)
            inventoryManager.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        if (uiManager)
            uiManager.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();

        if (gameCanvas)
            gameCanvas.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        if (shopCameraManager)
            shopCameraManager.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();

 
    }

    public void PickKey()
    {
        pickedKeys++;
    }
    /// <summary>
    /// Check if all the keys of this level are picked by the player
    /// </summary>
    /// <returns></returns>
    public bool AllKeysPicked()
    {
        if (keysToFinish > 0)
        {
            return (pickedKeys >= keysToFinish);
        } else
        {
            return true;
        }
    }
}
