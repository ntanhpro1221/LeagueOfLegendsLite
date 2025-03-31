using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// Indicate that <b>NO</b> state has executed exit logic <b>SUCCESSFULLY</b>.<br/>
    /// This will be converted to <see cref="StateRequireEnter"/> by <see cref="ConvertExitTag2EnterTagSystem"/>.<br/>
    /// </summary>
    public struct StateNotExitedYet : IComponentData, IEnableableComponent { }
}