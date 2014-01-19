using System.Collections.Generic;

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
