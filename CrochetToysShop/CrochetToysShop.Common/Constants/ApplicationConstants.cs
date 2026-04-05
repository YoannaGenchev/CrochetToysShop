namespace CrochetToysShop.Common.Constants
{
    public static class ApplicationConstants
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }

        public static class TempDataKeys
        {
            public const string SuccessMessage = "SuccessMessage";
            public const string ErrorMessage = "ErrorMessage";
        }

        public static class SuccessMessages
        {
            public const string ToyCreated = "Toy created successfully.";
            public const string ToyEdited = "Changes saved successfully.";
            public const string ToyDeleted = "Toy deleted successfully.";
            public const string OrderCreated = "Order request submitted successfully!";
            public const string OrderMarkedCompleted = "Order marked as completed.";
        }

        public static class ErrorMessages
        {
            public const string ToyNotFound = "Toy not found.";
            public const string ToyNotAvailable = "This toy is out of stock.";
            public const string MissingDefaultConnectionString = "Connection string 'DefaultConnection' not found.";
        }

        public static class UILabels
        {
            public const string Available = "Available";
            public const string OutOfStock = "Out of stock";
        }
    }
}
