using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.Interfaces
{
    /// <summary>
    /// Enumeration for movement notification types
    /// </summary>
    public enum NotificationType
    {
        Complete = 0,
        Update,
        Closeout
    }

    /// <summary>
    /// Interface for movement notification functionality
    /// </summary>
    public interface IMovementNotify
    {
        void Notify(NotificationType notification, MovementData movementData);
    }
}
