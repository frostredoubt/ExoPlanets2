using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that abstractly represents templates.
/// </summary>
public static class Template
{

    /// <summary>
    /// The directions that exist relative to any template.
    /// </summary>
    public enum Direction
    {

        /// <summary>
        /// A template-step horizontally to the left.
        /// </summary>
        Bottom,

        /// <summary>
        /// A template-step horizontally to the left.
        /// </summary>
        Left,

        /// <summary>
        /// A template-step horizontally to the right.
        /// </summary>
        Right,

        /// <summary>
        /// A template-step vertically updwards.
        /// </summary>
        Top

    }

    /// <summary>
    /// The supported exit paths out of a template as defined inside the Inspector for a prefab.
    /// </summary>
    public enum Exit
    {

        /// <summary>
        /// Exit on the bottom at the left.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Exit on the bottom at the middle.
        /// </summary>
        BottomMiddle,

        /// <summary>
        /// Exit on the bottom at the right.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Exit on the left at the bottom.
        /// </summary>
        LeftBottom,

        /// <summary>
        /// Exit on the left at the middle.
        /// </summary>
        LeftMiddle,

        /// <summary>
        /// Exit on the left at the top.
        /// </summary>
        LeftTop,

        /// <summary>
        /// Exit on the right at the bottom.
        /// </summary>
        RightBottom,

        /// <summary>
        /// Exit on the right at the middle.
        /// </summary>
        RightMiddle,

        /// <summary>
        /// Exit on the right at the top.
        /// </summary>
        RightTop,

        /// <summary>
        /// Exit on the top at the left.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Exit on the top at the middle.
        /// </summary>
        TopMiddle,

        /// <summary>
        /// Exit on the top at the right.
        /// </summary>
        TopRight

    }

    /// <summary>
    /// An interesting feature of a template.
    /// </summary>
    public enum Feature
    {

        /// <summary>
        /// Template contains the entrance of the level.
        /// </summary>
        Entrance,

        /// <summary>
        /// Template contains the exit of the level.
        /// </summary>
        Exit

    }

    /// <summary>
    /// The dictionary that allows templates to announce which exits they use.
    /// </summary>
    public static Dictionary<Exit, HashSet<GameObject>> exitLookupSet = new Dictionary<Exit, HashSet<GameObject>>();

    /// <summary>
    /// The width of a template in tiles.
    /// </summary>
    public const int TEMPLATE_TILE_WIDTH = 16;

    /// <summary>
    /// The height of a template in tiles.
    /// </summary>
    public const int TEMPLATE_TILE_HEIGHT = 16;

    /// <summary>
    /// Get the opposite direction of a provided direction.
    /// </summary>
    /// <param name="direction">The direction to get the opposite of.</param>
    /// <returns>The opposite direction of the current direction.</returns>
    public static Direction GetDirectionOpposite(Direction direction)
    {
        switch (direction)
        {
            case Direction.Bottom:
                return Direction.Top;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Top:
                return Direction.Bottom;
            default:
                throw new Exception("Unable to provide opposite direction for invalid input.");
        }
    }

    /// <summary>
    /// Get the attached opposite exit of a provided exit.
    /// </summary>
    /// <param name="direction">The exit to get the attached opposite of.</param>
    /// <returns>The attached opposite exit of the current exit.</returns>
    public static Exit GetExitOpposite(Exit exit)
    {
        switch (exit) // Hopefully this gets compiler-optimized into a lookup table (it almost certainly will lol)
        {
            case Exit.BottomLeft:
                return Exit.TopLeft;
            case Exit.BottomMiddle:
                return Exit.TopMiddle;
            case Exit.BottomRight:
                return Exit.TopRight;
            case Exit.LeftBottom:
                return Exit.RightBottom;
            case Exit.LeftMiddle:
                return Exit.RightMiddle;
            case Exit.LeftTop:
                return Exit.RightTop;
            case Exit.RightBottom:
                return Exit.LeftBottom;
            case Exit.RightMiddle:
                return Exit.LeftMiddle;
            case Exit.RightTop:
                return Exit.LeftTop;
            case Exit.TopLeft:
                return Exit.BottomLeft;
            case Exit.TopMiddle:
                return Exit.BottomMiddle;
            case Exit.TopRight:
                return Exit.BottomRight;
            default:
                throw new Exception("Unable to provide attached opposite exit for invalid input.");
        }
    }

    /// <summary>
    /// Return all of the possible exits that are associated with a particular direction.
    /// </summary>
    /// <param name="direction">The direction to return exits for.</param>
    /// <returns>A list of exits that occur on the provided direction.</returns>
    public static List<Exit> GetExitsFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Bottom:
                return new List<Exit>() { Exit.BottomLeft, Exit.BottomMiddle, Exit.BottomRight };
            case Direction.Left:
                return new List<Exit>() { Exit.LeftBottom, Exit.LeftMiddle, Exit.LeftTop };
            case Direction.Right:
                return new List<Exit>() { Exit.RightBottom, Exit.RightMiddle, Exit.RightTop };
            case Direction.Top:
                return new List<Exit>() { Exit.TopLeft, Exit.TopMiddle, Exit.TopRight };
            default:
                throw new Exception("Unable to provide opposite direction for invalid input.");
        }
    }

    /// <summary>
    /// Get a random exit on the side of a direction.
    /// </summary>
    /// <param name="direction">The direction to get a random exit for.</param>
    /// <returns>A random exit on the side of the direction.</returns>
    public static Exit GetRandomExitFromDirection(Direction direction)
    {
        List<Exit> exits = GetExitsFromDirection(direction);
        return exits[UnityEngine.Random.Range(0, exits.Count)];
    }

    /// <summary>
    /// Get a hash set of all of the valid possible templates associated with an exit type.
    /// </summary>
    /// <param name="exits">The list of exits to obtain a set of templates for.</param>
    /// <returns>A hash set containing all valid templates for a list of exit types.</returns>
    public static List<GameObject> GetTemplatesForExits(IEnumerable<Exit> exits)
    {
        IEnumerator<Exit> iterator = exits.GetEnumerator();
        HashSet<GameObject> returnSet = new HashSet<GameObject>();

        if (!iterator.MoveNext()) // If the provided collection is empty, return empty
        {
            return new List<GameObject>();
        }
        
        returnSet.UnionWith(exitLookupSet[iterator.Current]); // Add all the elements of the first present set

        while (iterator.MoveNext()) // Intersect with all the remaining sets
        {
            returnSet.IntersectWith(exitLookupSet[iterator.Current]);
        }

        return new List<GameObject>(returnSet);
    }

    /// <summary>
    /// Get a hash set of all of the valid possible templates associated with an exit type.
    /// </summary>
    /// <param name="directions">The list of directions to obtain a set of templates for.</param>
    /// <returns>A hash set containing all valid templates for a list of exit types.</returns>
    public static List<GameObject> GetTemplatesForDirections(IEnumerable<Direction> directions)
    {
        List<Exit> exitList = new List<Exit>();
        foreach (Direction direction in directions)
        {
            
            exitList.AddRange(GetExitsFromDirection(direction));
        }
        return GetTemplatesForExits(exitList);
    }

}

/// <summary>
/// A class that represents the requirements for a template during level generation.
/// </summary>
public class TemplateRequirementSet
{

    /// <summary>
    /// The required exits that must exist for a particular template during level generation.
    /// </summary>
    public HashSet<Template.Exit> exits;

    /// <summary>
    /// The required features for a particular template during level generation.
    /// </summary>
    public HashSet<Template.Feature> features;

    /// <summary>
    /// Create a template that has no exit direction or feature requirements.
    /// </summary>
    public TemplateRequirementSet()
        : this(new HashSet<Template.Exit>(), new HashSet<Template.Feature>())
    {
        return;
    }

    /// <summary>
    /// Create a template that has a set of known exit direction requirements.
    /// </summary>
    /// <param name="exits">The list of directions that require an exit for the template.</param>
    public TemplateRequirementSet(IEnumerable<Template.Exit> exits)
        : this(exits, new HashSet<Template.Feature>())
    {
        return;
    }

    /// <summary>
    /// Create a template that has a set of known exit direction and feature requirements.
    /// </summary>
    /// <param name="exits">The list of directions that require an exit for the template.</param>
    /// <param name="features">The list of required features for a template.</param>
    public TemplateRequirementSet(IEnumerable<Template.Exit> exits, IEnumerable<Template.Feature> features)
    {
        this.exits = new HashSet<Template.Exit>(exits);
        this.features = new HashSet<Template.Feature>(features);
        return;
    }

}
