namespace CrochetToysShop.Common.Constants
{
    public static class ApplicationConstants
    {
        public static class Roles
        {
            public const string Admin = "Admin";
        }

        public static class TempDataKeys
        {
            public const string SuccessMessage = "SuccessMessage";
            public const string ErrorMessage = "ErrorMessage";
        }

        public static class SuccessMessages
        {
            public const string ToyCreated = "Играчката е добавена успешно.";
            public const string ToyEdited = "Промените са запазени успешно.";
            public const string ToyDeleted = "Играчката е изтрита успешно.";
            public const string OrderCreated = "Поръчката е изпратена успешно!";
            public const string OrderMarkedCompleted = "Поръчката е маркирана като изпълнена.";
        }

        public static class ErrorMessages
        {
            public const string ToyNotFound = "Играчката не е намерена.";
            public const string ToyNotAvailable = "Тази играчка вече е изчерпана.";
            public const string MissingDefaultConnectionString = "Connection string 'DefaultConnection' not found.";
        }

        public static class AdminSeed
        {
            public const string Email = "yoanna@admin.com";
            public const string Password = "admin1";
        }

        public static class UILabels
        {
            public const string Available = "Available";
            public const string OutOfStock = "Out of stock";
        }
    }
}
