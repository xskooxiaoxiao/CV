When Adventurer is not an NPC, input WASD to operate agent actions, while WASD can also control camera movement, Q and E to control camera zoom in and out, and Spacebar toggle to control character or camera


Map Generator:
Static Constants and Variables:
	1. unitPerChunk: Represents the size of each chunk in the map.
	2. seed: Holds the seed value for random number generation.

Methods:
	1. GetNeighbors4(Vector2Int pos): Returns the four points adjacent to a given point.
	2. GetCorner4(Vector2Int pos): Returns the four points diagonally across a given point.
	3. RandomChunkLayout(int count): Generates a random map layout for the first layer. It starts from the origin (0,0) and expands by randomly selecting adjacent points until the specified count is reached.
	4. RandomChunkLayoutSecondLayer(Vector2Int[] layer1): Generates a second layer of the map layout based on the first layer generated. It creates a more detailed map with smaller units.

Generation Algorithm:
	1. The first layer generation starts from the origin (0,0) and expands randomly by selecting adjacent points as edges.
	2. The second layer generation further refines the map, creating walls and decorations based on the first layer's layout.
	3. The coastline generation adds details like walls and shorelines along the edges of the map.
	4. The map size is optionally doubled, and walls are generated accordingly.

Conditional Compilation:
	The code includes preprocessor directives (#if, #endif) to conditionally include or exclude certain sections of code based on compilation symbols like COASTLINE_GENERATE, MAKE_MAP_BIGGER, and WALL_GENERATE. This allows for easy customization and variation in map generation based on project requirements.

Randomization:
	Random numbers are generated using the Random class with a specified seed value.

Map Representation:
	The map is represented using a 2D byte array, where each byte value represents different types of terrain or features.


Agent Generator:
	Agents are generated at random locations within the map, and the AI for each agent has three behaviours, namely track and random move behaviour, flee and random move behaviour, and random move behaviour. The track behavior will start when the target distance is less than 15, while the flee behavior will start when the target distance is less than 10. When the flee behavior detects that the agent stays in the same position for more than six seconds, the agent will change direction and continue to move. All random actions continue for 1-3 seconds before changing the behavior parameters.


Video Link: https://www.bilibili.com/video/BV1LT421m7sq/?vd_source=bc7fb6e9ae81fad93c131264653670cc
