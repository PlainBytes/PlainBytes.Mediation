namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// The central interface for the mediator pattern implementation, providing a unified entry point
    /// for sending requests, publishing notifications, and executing queries.
    /// </summary>
    /// <remarks>
    /// This interface combines the functionality of <see cref="ISender"/>, <see cref="IPublisher"/>, 
    /// and <see cref="IGetter"/> to provide a single facade for all mediation operations.
    /// The mediator pattern helps reduce coupling between components by ensuring they don't
    /// directly reference each other, instead communicating through the mediator.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Injecting and using the mediator
    /// public class OrderController
    /// {
    ///     private readonly IMediator _mediator;
    /// 
    ///     public OrderController(IMediator mediator)
    ///     {
    ///         _mediator = mediator;
    ///     }
    /// 
    ///     public async Task&lt;IActionResult&gt; CreateOrder(CreateOrderCommand command)
    ///     {
    ///         var result = await _mediator.Send(command);
    ///         await _mediator.Publish(new OrderCreatedNotification(result.Id));
    ///         return Ok(result);
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IMediator : ISender, IPublisher, IGetter;
}