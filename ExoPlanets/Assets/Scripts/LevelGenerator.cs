using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


/// <summary>
/// The generator used to create new levels.
/// </summary>
public class LevelGenerator : MonoBehaviour
{

    #region Member Variables and Constants

    /// <summary>
    /// The width of a level in templates.
    /// </summary>
    public const int LEVEL_TEMPLATE_WIDTH = 4;

    /// <summary>
    /// The height of a level in templates.
    /// </summary>
    public const int LEVEL_TEMPLATE_HEIGHT = 4;

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
    public GameObject[] levelTemplates;

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

        for (int i = 0; i < levelTemplates.Length; i += 1)
        {
            TemplateAnnouncement templateAnnouncement = levelTemplates[i].GetComponent<TemplateAnnouncement>();
            for (int j = 0; j < templateAnnouncement.supportedExits.Length; j += 1)
            {
                Template.exitLookupSet[templateAnnouncement.supportedExits[j]].Add(levelTemplates[i]);
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
        return;
    }

    /// <summary>
    /// Perform the actual heavy-lifting involved with generating a new level.
    /// </summary>
    private void GenerateLevel()
    {
        
        TemplateRequirementSet[,] templateRequirements = GenerateBaseRequirements(LEVEL_TEMPLATE_WIDTH, LEVEL_TEMPLATE_HEIGHT); // Generate a required path through the level

        for (int templateY = 0; templateY < LEVEL_TEMPLATE_WIDTH; templateY += 1) // Loop over all the templates in the level...TODO: Turn this into it's own routine
        {
            for (int templateX = 0; templateX < LEVEL_TEMPLATE_HEIGHT; templateX += 1)
            {
                GameObject currentTemplate;

                if (templateRequirements[templateY, templateX].exitDirections.Count == 0)
                {
                    currentTemplate = levelTemplates[UnityEngine.Random.Range(0, levelTemplates.Length)];
                }
                else
                {
                    List<GameObject> allowableTemplates = Template.GetTemplatesForDirections(templateRequirements[templateY, templateX].exitDirections);
                    if (allowableTemplates == null || allowableTemplates.Count == 0)
                    {
                        throw new Exception("Unable to retrieve a valid template for use in level generation.");
                    }
                    currentTemplate = allowableTemplates[UnityEngine.Random.Range(0, allowableTemplates.Count)];
                }

                GameObject instance = Instantiate(currentTemplate, new Vector3(templateY * Template.TEMPLATE_TILE_HEIGHT, templateX * Template.TEMPLATE_TILE_WIDTH), Quaternion.identity) as GameObject;
                instance.transform.parent = this.transform;

            }
        }

        return;
    }

    /// <summary>
    /// Generates a required path through the level and all associated template exits, as well as the level entrance and exit.
    /// </summary>
    /// <param name="levelTemplateHeight">The height of the level to generate in templates.</param>
    /// <param name="levelTemplateWidth">The width of the level to generate in templates.</param>
    private TemplateRequirementSet[,] GenerateBaseRequirements(int levelTemplateHeight, int levelTemplateWidth)
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

            if (moveDirection == null) // If the direction is null, then we should be placing an exit and ending
            {
                templateRequirements[currentRow, currentColumn].features.Add(Template.Feature.Exit);
                return templateRequirements;
            }
            templateRequirements[currentRow, currentColumn].exitDirections.Add(moveDirection.Value); // Add the move direction

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
            templateRequirements[currentRow, currentColumn].exitDirections.Add(Template.GetDirectionOpposite(moveDirection.Value)); // Add the move direction
            lastDirection = moveDirection;
        }
    }

    /// <summary>
    /// Get a random generation direction to follow based on current position in the level and the last direction moved in.
    /// </summary>
    /// <param name="currentRow">The current row position in templates.</param>
    /// <param name="currentColumn">The current column position in templates.</param>
    /// <param name="levelTemplateHeight">The maximum height of the map in templates.</param>
    /// <param name="levelTemplateWidth">The maximum width of the map in templates.</param>
    /// <param name="lastDirection">The last direction moved in (possibly null).</param>
    private Template.Direction? GetMoveDirection(int currentRow, int currentColumn, int levelTemplateHeight, int levelTemplateWidth, Template.Direction? lastDirection)
    {
        List<Template.Direction> validDirections = new List<Template.Direction>();
        Template.Direction chosenDirection;
        int randomUpper = 1;

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

        validDirections.Add(Template.Direction.Top); // Moving upwards is always a valid "choice", but if chosen on the top row, we return null (which tells the map to place an exit)
        
        chosenDirection = validDirections[UnityEngine.Random.Range(0, randomUpper)];
        if (chosenDirection == Template.Direction.Top && currentRow == levelTemplateHeight - 1) // If we're moving up on the final row, simply terminate
        {
            return null;
        }
        return chosenDirection;
    }

    // Update is called once per frame
    void Update()
    {
        return;
    }

}
