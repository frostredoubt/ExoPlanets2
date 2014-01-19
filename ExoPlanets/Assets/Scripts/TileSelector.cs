using UnityEngine;


/// <summary>
/// Represents a class of tiles that, when initialized in level generation, will randomly select one of a range of sprites.
/// </summary>
public class TileSelector : MonoBehaviour
{

    /// <summary>
    /// The different sprites that may be used for the tile.
    /// </summary>
	public Sprite[] possibilities;

	/// <summary>
	/// Initialization method that determines the sprite for the tile.
	/// </summary>
	void Awake()
    {
		Sprite sprite = possibilities[UnityEngine.Random.Range(0, possibilities.Length)];
		this.GetComponent<SpriteRenderer>().sprite = sprite;
        return;
	}
	
}
