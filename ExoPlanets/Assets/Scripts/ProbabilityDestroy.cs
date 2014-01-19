using UnityEngine;


/// <summary>
/// A behaviour that, when attached to a tile, allows it to randomly destroy itself during level generation.
/// </summary>
public class ProbabilityDestroy : MonoBehaviour
{

    /// <summary>
    /// The chance that this tile we be destroyed on level generation.
    /// </summary>
	public float chanceToDestroy;

	/// <summary>
	/// On initialization during level generation, determine whether or not to destroy the tile.
	/// </summary>
	void Awake()
    {
		if (Random.value < chanceToDestroy)
        {
			Destroy(this.gameObject);
		}
        return;
	}
	
}
