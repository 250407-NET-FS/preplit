using Preplit.Domain;

namespace Preplit.Tests
{
    public static class SharedObjects
    {
        // static card data fields
        public static readonly Guid VALID_CARD_ID_1 = Guid.Parse("67d31a31-05fe-4f32-94ce-cd2ccf52ec30");
        public static readonly Guid VALID_CARD_ID_2 = Guid.Parse("940178fa-2168-42b8-a9a4-2dffdb218819");
        public static readonly Guid VALID_CARD_ID_3 = Guid.Parse("baef5434-a9ee-46c3-877a-7bdbedce7403");
        public static readonly Guid VALID_CARD_ID_4 = Guid.Parse("601973f6-9de5-4eee-a6b6-351eaeba29a4");
        public static readonly Guid INVALID_CARD_ID = Guid.Parse("cb3d0949-35e7-44db-8df8-c47b497ee9d6");
        // static cateogry data fields
        public static readonly Guid VALID_CATEGORY_ID_1 = Guid.Parse("3426b57b-d948-4141-90a1-e2d018fe01b7");
        public static readonly Guid VALID_CATEGORY_ID_2 = Guid.Parse("f7b74855-de9e-42df-914e-116cc6dfb3b3");
        public static readonly Guid VALID_CATEGORY_ID_3 = Guid.Parse("ca7ccedd-718f-4eb3-a04c-a887bd5cdbf3");
        public static readonly Guid VALID_CATEGORY_ID_4 = Guid.Parse("3ed382b0-bd08-485e-b5bc-663f8c97be4f");
        public static readonly Guid INVALID_CATEGORY_ID = Guid.Parse("cda7e0a9-9874-4e44-811a-248b26b0ea12");
        // static user data fields
        public static readonly Guid VALID_USER_ID_1 = Guid.Parse("a5e27962-3013-4de1-8f5d-c9c1a55e85c4");
        public static readonly Guid VALID_USER_ID_2 = Guid.Parse("507f4586-e18f-4e0d-a4c2-6fe639a38092");
        public static readonly Guid INVALID_USER_ID = Guid.Parse("7b81f553-5863-492c-9b25-009b31c6646f");

        // testing objects
        public static Card CloneValidCard1()
        {
            return new Card()
            {
                CardId = VALID_CARD_ID_1,
                Question = "Question 1",
                Answer = "Answer 1",
                CategoryId = VALID_CATEGORY_ID_1,
                UserId = VALID_USER_ID_1
            };
        }

        public static Card CloneValidCard2()
        {
            return new Card()
            {
                CardId = VALID_CARD_ID_2,
                Question = "Question 2",
                Answer = "Answer 2",
                CategoryId = VALID_CATEGORY_ID_2,
                UserId = VALID_USER_ID_1
            };
        }

        public static Card CloneValidCard3()
        {
            return new Card()
            {
                CardId = VALID_CARD_ID_3,
                Question = "Question 3",
                Answer = "Answer 3",
                CategoryId = VALID_CATEGORY_ID_3,
                UserId = VALID_USER_ID_2
            };
        }

        public static Card CloneValidCard4()
        {
            return new Card()
            {
                CardId = VALID_CARD_ID_4,
                Question = "Question 4",
                Answer = "Answer 4",
                CategoryId = VALID_CATEGORY_ID_3,
                UserId = VALID_USER_ID_2
            };
        }

        public static Category CloneValidCategory1()
        {
            return new Category()
            {
                CategoryId = VALID_CATEGORY_ID_1,
                Name = "Category 1",
                UserId = VALID_USER_ID_1
            };
        }

        public static Category CloneValidCategory2()
        {
            return new Category()
            {
                CategoryId = VALID_CATEGORY_ID_2,
                Name = "Category 2",
                UserId = VALID_USER_ID_1
            };
        }

        public static Category CloneValidCategory3()
        {
            return new Category()
            {
                CategoryId = VALID_CATEGORY_ID_3,
                Name = "Category 3",
                UserId = VALID_USER_ID_2
            };
        }

        public static Category CloneValidCategory4()
        {
            return new Category()
            {
                CategoryId = VALID_CATEGORY_ID_4,
                Name = "Category 4",
                UserId = VALID_USER_ID_2
            };
        }
    }
}