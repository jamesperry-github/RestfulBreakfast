using ErrorOr;

namespace RestfulBreakfast.ServiceErrors;

public static class Errors
{
    public static class Breakfast
    {
        public static Error NotFound => Error.NotFound(
            code: "Breakfast.NotFound",
            description: "Breakfast not found"
            );
        public static Error InvalidName => Error.NotFound(
            code: "Breakfast.InvalidName",
            description: $"Breakfast name must be atleast {Models.Breakfast.MinNameLength} characters long" +
            $" and at most {Models.Breakfast.MaxNameLength} characters."
            );
        public static Error InvalidDescription => Error.NotFound(
            code: "Breakfast.InvalidName",
            description: $"Breakfast description must be atleast {Models.Breakfast.MinDescriptionLength} characters long" +
            $" and at most {Models.Breakfast.MaxDescriptionLength} characters."
            );
    }
}
