using For.RuleEngine.Interface;

namespace For.RuleEngine.Model
{
    /// <summary>
    /// rule engine container model
    /// </summary>
    internal class ContainerModel
    {
        internal string Key { get; set; }
        internal IContainerRule Rule { get; set; }
    }
}