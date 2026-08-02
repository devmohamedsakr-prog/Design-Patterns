using OrderValidation.After.Models;

namespace OrderValidation.After.Handlers
{
    /// <summary>
    /// ValidationHandler: Abstract base handler for chain of responsibility
    /// SRP: Provides chain interface for validation handlers
    /// </summary>
    public abstract class ValidationHandler
    {
        protected ValidationHandler _nextHandler;

        /// <summary>
        /// Set the next handler in the chain
        /// </summary>
        public ValidationHandler SetNext(ValidationHandler nextHandler)
        {
            _nextHandler = nextHandler;
            return nextHandler;
        }

        /// <summary>
        /// Handle validation and pass to next handler
        /// </summary>
        public abstract ValidationResult Handle(Order order);

        /// <summary>
        /// Call next handler or return success if no next handler
        /// </summary>
        protected ValidationResult PassToNext(Order order)
        {
            if (_nextHandler != null)
                return _nextHandler.Handle(order);

            return new ValidationResult(true, "", GetType().Name);
        }
    }
}
