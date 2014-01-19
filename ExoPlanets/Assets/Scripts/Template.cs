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
                throw new ArgumentException("Unable to provide opposite direction for invalid input.");
        }
    }
}

/// <summary>
/// A class that represents the requirements for a template during level generation.
/// </summary>
public class TemplateRequirementSet
{

    /// <summary>
    /// The required directions where exits must exist for a particular template during level generation.
    /// </summary>
    public HashSet<Template.Direction> exitDirections;

    /// <summary>
    /// The required features for a particular template during level generation.
    /// </summary>
    public HashSet<Template.Feature> features;

    /// <summary>
    /// Create a template that has no exit direction or feature requirements.
    /// </summary>
    public TemplateRequirementSet()
        : this(new HashSet<Template.Direction>(), new HashSet<Template.Feature>())
    {
        return;
    }

    /// <summary>
    /// Create a template that has a set of known exit direction requirements.
    /// </summary>
    /// <param name="exitDirections">The list of directions that require an exit for the template.</param>
    public TemplateRequirementSet(IEnumerable<Template.Direction> exitDirections)
        : this(exitDirections, new HashSet<Template.Feature>())
    {
        return;
    }

    /// <summary>
    /// Create a template that has a set of known exit direction and feature requirements.
    /// </summary>
    /// <param name="exitDirections">The list of directions that require an exit for the template.</param>
    /// <param name="features">The list of required features for a template.</param>
    public TemplateRequirementSet(IEnumerable<Template.Direction> exitDirections, IEnumerable<Template.Feature> features)
    {
        this.exitDirections = new HashSet<Template.Direction>(exitDirections);
        this.features = new HashSet<Template.Feature>(features);
        return;
    }

}
