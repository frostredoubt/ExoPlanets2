using UnityEngine;
using System.Collections;

/// <summary>
/// An abstract representation of a tile element in a level.
/// </summary>
public class Tile
{

    /// <summary>
    /// Different types that tile elements may take on.
    /// </summary>
    public enum TileOption
    {
        /// <summary>
        /// An empty tile with nothing in it.
        /// </summary>
        Empty,

        /// <summary>
        /// A solid tile representing a piece of floor.
        /// </summary>
        Floor
    }

    /// <summary>
    /// The type of the tile.
    /// </summary>
    public TileOption type;

    /// <summary>
    /// Create a new tile.
    /// </summary>
    /// <param name="type">The type of tile to create.</param>
    public Tile(TileOption type)
    {
        this.type = type;
        return;
    }

}
