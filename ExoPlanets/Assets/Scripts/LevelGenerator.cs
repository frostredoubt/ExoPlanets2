using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


/// <summary>
/// The generator used to create new levels.
/// </summary>
public class LevelGenerator : MonoBehaviour
{

    #region Member Variables and Constants

    /// <summary>
    /// The maximum width of a level in templates.
    /// </summary>
    public const int MAX_LEVEL_TEMPLATE_WIDTH = 4;

    /// <summary>
    /// The maximum height of a level in templates.
    /// </summary>
    public const int MAX_LEVEL_TEMPLATE_HEIGHT = 4;

    /// <summary>
    /// The current height of a level in templates.
    /// </summary>
    private int levelTemplateHeight;

    /// <summary>
    /// The current width of a level in templates.
    /// </summary>
    private int levelTemplateWidth;

    /// <summary>
    /// The matrix of requirements necessary to properly generate a level.
    /// </summary>
    private TemplateRequirementSet[,] levelRequirements = null;

    /// <summary>
    /// The matrix of templates being used to compose the current level.
    /// </summary>
    private GameObject[,] levelTemplates = null;

    #endregion

    #region Editor-Exposed Member Variables

    // Note: These members should be set through the inspector, not the code

    /// <summary>
    /// Assign the empty tile.
    /// </summary>
    public GameObject tileEmpty;

    /// <summary>
    /// Assign the floor tile.
    /// </summary>
    public GameObject tileFloor;

    /// <summary>
    /// The list of level templates that are usable by the generator.
    /// </summary>
    public GameObject[] usableTemplates;

    #endregion

    /// <summary>
    /// Do very-first intialization.
    /// </summary>
    private void Awake()
    {

        // Create the associative list of templates containing exit types
        foreach (Template.Exit exit in Enum.GetValues(typeof(Template.Exit)))
        {
            Template.exitLookupSet.Add(exit, new HashSet<GameObject>());
        }

        for (int i = 0; i < usableTemplates.Length; i += 1)
        {
            TemplateAnnouncement templateAnnouncement = usableTemplates[i].GetComponent<TemplateAnnouncement>();
            for (int j = 0; j < templateAnnouncement.supportedExits.Length; j += 1)
            {
                Template.exitLookupSet[templateAnnouncement.supportedExits[j]].Add(usableTemplates[i]);
            }
        }
        GenerateLevel();
        return;
    }

    /// <summary>
    /// The entry point where intialization occurs just prior to level-loading.
    /// </summary>
    private void Start()
    {
        GenerateTemplatesCleanupPass(); // Run the cleanup pass to perform any actions requiring dynamic tiles
        return;
    }

    /// <summary>
    /// Perform the actual heavy-lifting involved with generating a new level.
    /// </summary>
    private void GenerateLevel()
    {
        SetLevelSize();
        CleanUpLevel();
        levelRequirements = GenerateBaseRequirements(); // Generate a required path through the level
        GenerateTemplatesFirstPass(Template.TEMPLATE_TILE_HEIGHT, Template.TEMPLATE_TILE_WIDTH); // Create the required tile templates
        return;
    }

    /// <summary>
    /// Set the size for the current level.
    /// </summary>
    private void SetLevelSize()
    {
        levelTemplateHeight = MAX_LEVEL_TEMPLATE_HEIGHT;
        levelTemplateWidth = MAX_LEVEL_TEMPLATE_WIDTH;
        return;
    }

    /// <summary>
    /// Clean up any remnants remaining from previous levels.
    /// </summary>
    private void CleanUpLevel()
    {
        if (levelTemplates == null) // Delete all the templates that exist in the level
        {
            levelTemplates = new GameObject[levelTemplateHeight, levelTemplateWidth];
        }
        else
        {
            for (int i = 0; i < levelTemplates.GetLength(0); i += 1)
            {
                for (int j = 0; j < levelTemplates.GetLength(1); j += 1)
                {
                    Destroy(levelTemplates[i, j]);
                }
            }
        }
        return;
    }

    /// <summary>
    /// Generates a required path through the level and all associated template exits, as well as the level entrance and exit.
    /// </summary>
    private TemplateRequirementSet[,] GenerateBaseRequirements()
    {
        TemplateRequirementSet[,] templateRequirements = new TemplateRequirementSet[levelTemplateHeight, levelTemplateWidth];
        Template.Direction? lastDirection = null;
        int currentRow = 0, currentColumn = UnityEngine.Random.Range(0, levelTemplateWidth);

        // Set up new requirement sets for all templates
        for (int i = 0; i < levelTemplateHeight; i += 1)
        {
            for (int j = 0; j < levelTemplateWidth; j += 1)
            {
                templateRequirements[i, j] = new TemplateRequirementSet();
            }
        }

        templateRequirements[currentRow, currentColumn].features.Add(Template.Feature.Entrance); // Set up the entrance

        while (true)
        {
            Template.Direction? moveDirection = GetMoveDirection(currentRow, currentColumn, levelTemplateHeight, levelTemplateWidth, lastDirection); // Get the direction to move in
            Template.Exit templateExit;

            if (moveDirection == null) // If the direction is null, then we should be placing an exit and ending
            {
                templateRequirements[currentRow, currentColumn].features.Add(Template.Feature.Exit);
                return templateRequirements;
            }

            templateExit = Template.GetRandomExitFromDirection(moveDirection.Value);
            templateRequirements[currentRow, currentColumn].exits.Add(templateExit); // Add the move direction

            switch (moveDirection.Value) // Move in the desired direction
            {
                case Template.Direction.Left:
                    currentColumn -= 1;
                    break;
                case Template.Direction.Right:
                    currentColumn += 1;
                    break;
                case Template.Direction.Top:
                    currentRow += 1;
                    break;
                default:
                    throw new Exception("Selected move direction is invalid for the current state of the level generator.");
            }

            templateRequirements[currentRow, currentColumn].exits.Add(Template.GetExitOpposite(templateExit)); // Add the move direction
            lastDirection = moveDirection;

        }
    }

    /// <summary>
    /// Get a random generation direction to follow based on current position in the level and the last direction moved in.
    /// </summary>
    /// <param name="currentRow">The current row position in templates.</param>
    /// <param name="currentColumn">The current column position in templates.</param>
    /// <param name="lastDirection">The last direction moved in (possibly null).</param>
    private Template.Direction? GetMoveDirection(int currentRow, int currentColumn, int levelTemplateHeight, int levelTemplateWidth, Template.Direction? lastDirection)
    {
        List<Template.Direction> validDirections = new List<Template.Direction>();
        Template.Direction chosenDirection;
        int randomUpper = 1;

        validDirections.Add(Template.Direction.Top); // Moving upwards is always a valid "choice", but if chosen on the top row, we return null (which tells the map to place an exit)

        if ((lastDirection == null || lastDirection != Template.Direction.Right) && (currentColumn > 0)) // Add moving left as a valid direction
        {
            validDirections.Add(Template.Direction.Left); // Cheat to allow a random distribution
            validDirections.Add(Template.Direction.Left);
            randomUpper += 2;
        }

        if ((lastDirection == null || lastDirection != Template.Direction.Left) && (currentColumn < levelTemplateWidth - 1)) // Add moving left as a valid direction
        {
            validDirections.Add(Template.Direction.Right); // Cheat to allow a random distribution
            validDirections.Add(Template.Direction.Right);
            randomUpper += 2;
        }

        chosenDirection = validDirections[UnityEngine.Random.Range(0, randomUpper)];
        if (chosenDirection == Template.Direction.Top && currentRow == levelTemplateHeight - 1) // If we're moving up on the final row, simply terminate
        {
            return null;
        }
        return chosenDirection;
    }

    /// <summary>
    /// Generate the templates for the required path through the level.
    /// </summary>
    /// <param name="templateTileHeight">The height of a template in tiles.</param>
    /// <param name="templateTileWidth">The width of a template in tiles.</param>
    private void GenerateTemplatesFirstPass(int templateTileHeight, int templateTileWidth)
    {

        for (int i = 0; i < levelRequirements.GetLength(0); i += 1)
        {

            for (int j = 0; j < levelRequirements.GetLength(1); j += 1)
            {
                GameObject currentTemplate;
                if (levelRequirements[i, j].exits.Count == 0) // Skip over any non-required tiles for now
                {
                    continue;
                }

                List<GameObject> allowableTemplates = Template.GetTemplatesForExits(levelRequirements[i, j].exits);
                if (allowableTemplates == null || allowableTemplates.Count == 0)
                {
                    throw new Exception("Unable to retrieve a valid template for use in level generation.");
                }
                currentTemplate = allowableTemplates[UnityEngine.Random.Range(0, allowableTemplates.Count)];
                InstantiateTemplate(currentTemplate, levelRequirements, i, j, templateTileHeight, templateTileWidth);
            }
        }
        return;
    }

    /// <summary>
    /// Instantiate a template within the game world.
    /// </summary>
    /// <param name="template">The template to instantiate.</param>
    /// <param name="templateRequirements">The requirements for templates in the level.</param>
    /// <param name="column">The column the template sits at in the level matrix.</param>
    /// <param name="row">The row the template sits at in the level matrix.</param>
    /// <param name="templateTileHeight">The height of a template in tiles.</param>
    /// <param name="templateTileWidth">The width of a template in tiles.</param>
    private void InstantiateTemplate(GameObject template, TemplateRequirementSet[,] templateRequirements, int column, int row, int templateTileHeight, int templateTileWidth)
    {
        levelTemplates[column, row] = Instantiate(template, new Vector3(row * templateTileWidth, column * templateTileHeight), Quaternion.identity) as GameObject;
        levelTemplates[column, row].transform.parent = this.transform;
        return;
    }

    /// <summary>
    /// Run the cleanup pass to perform any actions requiring dynamic tiles during level generation.
    /// </summary>
    private void GenerateTemplatesCleanupPass()
    {
        for (int i = 0; i < levelTemplateHeight; i += 1)
        {
            for (int j = 0; j < levelTemplateWidth; j += 1)
            {
                foreach (Template.Exit exit in levelRequirements[i, j].exits) // Remove the required exit walls from the template
                {
                    RemoveExitWalls(levelTemplates[i, j], exit);
                }
            }
        }
        return;
    }

    /// <summary>
    /// Remove the walls from an exit on a template.
    /// </summary>
    /// <param name="template">The template to remove the walls from.</param>
    /// <param name="exit">The exit whose walls should be removed from the template.</param>
    private void RemoveExitWalls(GameObject template, Template.Exit exit)
    {
        int exitX = Template.GetExitXComponent(exit), exitY = Template.GetExitYComponent(exit);
        List<GameObject> exitTiles = Template.GetExitTiles(template);
        GameObject exitTile;
        int origX, origY;

        switch (exit) // Hopefully this gets compiler-optimized into a lookup table (it almost certainly will lol)
        {
            case Template.Exit.BottomLeft:
            case Template.Exit.TopLeft:
                while ((exitTile = GetTileByCoordinates(exitTiles, exitX, exitY)) != null)
                {
                    Destroy(exitTile);
                    exitX += 1;
                }
                break;
            case Template.Exit.BottomMiddle:
            case Template.Exit.TopMiddle:
                origX = exitX;
                while ((exitTile = GetTileByCoordinates(exitTiles, exitX, exitY)) != null)
                {
                    Destroy(exitTile);
                    exitX -= 1;
                }
                exitX = origX + 1;
                while ((exitTile = GetTileByCoordinates(exitTiles, exitX, exitY)) != null)
                {
                    Destroy(exitTile);
                    exitX += 1;
                }
                break;
            case Template.Exit.BottomRight:
            case Template.Exit.TopRight:
                while ((exitTile = GetTileByCoordinates(exitTiles, exitX, exitY)) != null)
                {
                    Destroy(exitTile);
                    exitX -= 1;
                }
                break;
            case Template.Exit.LeftBottom:
            case Template.Exit.RightBottom:
                while ((exitTile = GetTileByCoordinates(exitTiles, exitX, exitY)) != null)
                {
                    Destroy(exitTile);
                    exitY += 1;
                }
                break;
            case Template.Exit.LeftMiddle:
            case Template.Exit.RightMiddle:
                origY = exitY;
                while ((exitTile = GetTileByCoordinates(exitTiles, exitX, exitY)) != null)
                {
                    Destroy(exitTile);
                    exitY -= 1;
                }
                exitY = origY + 1;
                while ((exitTile = GetTileByCoordinates(exitTiles, exitX, exitY)) != null)
                {
                    Destroy(exitTile);
                    exitY += 1;
                }
                break;
            case Template.Exit.LeftTop:
            case Template.Exit.RightTop:
                while ((exitTile = GetTileByCoordinates(exitTiles, exitX, exitY)) != null)
                {
                    Destroy(exitTile);
                    exitY -= 1;
                }
                break;
            default:
                throw new Exception("Unable to remove exit walls for invalid input.");
        }

        return;
    }

    /// <summary>
    /// Get a tile from a list of tiles by matching coordinates.
    /// </summary>
    /// <param name="exitTiles">The tiles to check the coordinates against.</param>
    /// <param name="tileX">The template-relative X coordinate of the desired tile.</param>
    /// <param name="tileY">The template-relative y coordinate of the desired tile.</param>
    /// <returns>Returns the desired tile if it is located, and null if not.</returns>
    private GameObject GetTileByCoordinates(List<GameObject> exitTiles, int tileX, int tileY)
    {
        foreach (GameObject tile in exitTiles)
        {
            if (tile.transform.localPosition.x == tileX && tile.transform.localPosition.y == tileY)
            {
                return tile;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        return;
    }

}
