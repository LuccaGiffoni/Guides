using System;
using System.Collections.Generic;
using Scripts_v2.Data.Models;

namespace Scripts_v2.EventSystem
{
    public class EventBus
    {
        private static readonly EventBus instance = new EventBus();

        public static EventBus Instance => instance;

        private EventBus() { }

        public event Action<Operation> OperationStarted;
        public event Action OperationEnded;
        public event Action<Step> StepChanged;
        public event Action<Guid> AnchorCreated;
        public event Action<Guid> AnchorDeleted;
        public event Action<List<Guid>> AnchorsLoaded;
        public event Action<PickPositionData> PickPositionCreated;
        public event Action<List<PickPositionData>> PickPositionsLoaded;

        public void OnOperationStarted(Operation operation) => OperationStarted?.Invoke(operation);
        public void OnOperationEnded() => OperationEnded?.Invoke();
        public void OnStepChanged(Step step) => StepChanged?.Invoke(step);
        public void OnAnchorCreated(Guid anchorId) => AnchorCreated?.Invoke(anchorId);
        public void OnAnchorDeleted(Guid anchorId) => AnchorDeleted?.Invoke(anchorId);
        public void OnAnchorsLoaded(List<Guid> anchorIds) => AnchorsLoaded?.Invoke(anchorIds);
        public void OnPickPositionCreated(PickPositionData pickPosition) => PickPositionCreated?.Invoke(pickPosition);
        public void OnPickPositionsLoaded(List<PickPositionData> pickPositions) => PickPositionsLoaded?.Invoke(pickPositions);
    }
}