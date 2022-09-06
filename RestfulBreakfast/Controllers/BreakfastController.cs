using Microsoft.AspNetCore.Mvc;
using RestfulBreakfast.Models;
using RestfulBreakfast.Contracts.Breakfast;
using RestfulBreakfast.Services.Breakfasts;
using ErrorOr;
using RestfulBreakfast.ServiceErrors;

namespace RestfulBreakfast.Controllers
{
    public class BreakfastsController : ApiController
    {
        private readonly IBreakfastService _breakfastService;
        public BreakfastsController(IBreakfastService breakfastService)
        {
            // Dependancy Injection
            _breakfastService = breakfastService;
        }

        [HttpPost]
        public IActionResult CreateBreakfast(CreateBreakfastRequest request)
        {
            // map to C# object
            ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(request);

            if(requestToBreakfastResult.IsError)
            {
                return Problem(requestToBreakfastResult.Errors);
            }
            // save to DB (in our case memory)
            var breakfast = requestToBreakfastResult.Value;
            ErrorOr<Created> createBreakfastResult = _breakfastService.CreateBreakfast(breakfast);
            if (createBreakfastResult.IsError)
            {
                return Problem(createBreakfastResult.Errors);
            }

            return createBreakfastResult.Match(
                created => CreatedAtGetBreakfast(breakfast),
                errors => Problem(errors));
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetBreakfast(Guid id)
        {
            ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

            return getBreakfastResult.Match(
                breakfast => Ok(MapBreakfastResponse(breakfast)),
                errors => Problem(errors));
        }
        [HttpPut("{id:guid}")]
        public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
        {
            ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(id, request);

            if (requestToBreakfastResult.IsError)
            {
                return Problem(requestToBreakfastResult.Errors);
            }

            var breakfast = requestToBreakfastResult.Value;
            ErrorOr<UpsertedBreakfast> upsertBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);
            
            return upsertBreakfastResult.Match(
                upserted => upserted.IsNewlyCreated ? CreatedAtGetBreakfast(breakfast) : NoContent(),
                errors => Problem(errors));
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteBreakfast(Guid id)
        {
            ErrorOr<Deleted> deleteBreakfastResult = _breakfastService.DeleteBreakfast(id);

            return deleteBreakfastResult.Match(
                deleted => NoContent(),
                errors => Problem(errors));
        }
        private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
        {
            return new BreakfastResponse(
                breakfast.Id,
                breakfast.Name,
                breakfast.Description,
                breakfast.StartDateTime,
                breakfast.EndDateTime,
                breakfast.LastModifiedDateTime,
                breakfast.Savory,
                breakfast.Sweet
                );
        }
        private CreatedAtActionResult CreatedAtGetBreakfast(Breakfast breakfast)
        {
            return CreatedAtAction(
                actionName: nameof(GetBreakfast),
                routeValues: new { id = breakfast.Id },
                value: MapBreakfastResponse(breakfast));
        }

    }
}
