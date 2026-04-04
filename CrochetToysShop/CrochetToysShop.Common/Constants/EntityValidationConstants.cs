namespace CrochetToysShop.Common.Constants
{
    public static class EntityValidationConstants
    {
        public static class Category
        {
            public const int NameMaxLength = 40;
        }

        public static class Toy
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 60;
            public const int DescriptionMinLength = 20;
            public const int DescriptionMaxLength = 2000;
            public const string PriceMinValue = "0.01";
            public const string PriceMaxValue = "100000";
            public const int SizeMinCm = 1;
            public const int SizeMaxCm = 200;
            public const int DifficultyMaxLength = 20;
            public const string DefaultDifficulty = "Easy";
            public const string ImageUrlPattern = @"^/images/toys/.*\.(jpg|jpeg|png|webp)$";
            public const string ImageUrlPatternErrorMessage = "Моля въведи път като /images/toys/име.jpg";
        }

        public static class Order
        {
            public const int CustomerNameMaxLength = 40;
            public const int PhoneNumberMaxLength = 10;
            public const int AddressMaxLength = 200;
            public const int StatusMaxLength = 20;
        }

        public static class OrderRequest
        {
            public const int CustomerNameMaxLength = 60;
            public const int MessageMinLength = 5;
            public const int MessageMaxLength = 2000;
            public const int QuantityMin = 1;
            public const int QuantityMax = 100;
            public const int StatusMaxLength = 20;
        }
    }
}
