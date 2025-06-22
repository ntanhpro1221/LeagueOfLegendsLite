using NGDtuanh.Entities.StateMachine;
using Unity.Entities;
using Unity.NetCode;

public static partial class SMHelpers {
    public static partial class TryExit<TState> where TState : unmanaged, IComponentData, IEnableableComponent {
        /// <summary>
        /// Common exit behaviour for state of activable item.
        /// </summary>
        public static bool ItemCommon<TStateFilter>(
            in TStateFilter                   filter
          , in CommonExitStateAspect          common
          , in CommonExitStateAspect_Champion commonChamp
          , in ItemCommonStateData            stateData
          , in ComponentLookup<Selectable>    selectLookup
          , in NetworkTick                    curTick)
            where TStateFilter : unmanaged, IStateExitFunc<TState> {
            // DEAD STATE
            if (common.Health.IsDead) // Run out of health.
                common.State.SetDead();

            // BLOCK ALL OTHER STATE WHEN HASN'T PERFORMED YET
            else if (!stateData.performData.isPerformed)
                return false;

            // ITEM ANALYZING STATE
            else if (
                // Not have disabling activate item CC.
                common.CC.Disable.ActiveItem == 0
                // Have request.
             && commonChamp.ItemRequest.haveRequest)
                common.State.SetItemActiveAnalyzing();

            // MOVE STATE
            else if (commonChamp.Input.MoveEvent_WithData) // Have move request
                common.State.SetMove();

            // ATTACK STATE
            else if (common.Target.IsTargetExists(selectLookup)) // Have target
                common.State.SetAttack();

            // IDLE STATE
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            else if (curTick.IsNewerThan(stateData.performData.doneTick)) // Completely done skill
                common.State.SetIdle();

            else return false;

            IStateExitFunc<TState>.MarkExitExecuted(filter);
            return true;
        }
    }
}