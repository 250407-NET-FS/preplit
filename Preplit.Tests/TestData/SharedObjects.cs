using Preplit.Domain;

namespace Preplit.Tests
{
    public static class SharedObjects
    {
        // static card data fields
        public static readonly Guid VALID_CARD_ID_1 = Guid.Parse("b5b9c1e1-5f9c-4b0c-8c2d-0f1e2d3c4b5a");
        public static readonly Guid VALID_CARD_ID_2 = Guid.Parse("b5b9c1e1-5f9c-4b0c-8c2d-0f1e2d3c4b5b");
        public static readonly Guid VALID_CARD_ID_3 = Guid.Parse("b5b9c1e1-5f9c-4b0c-8c2d-0f1e2d3c4b5c");
        public static readonly Guid INVALID_CARD_ID = Guid.Parse("b5b9c1e1-5f9c-4b0c-8c2d-0f1e2d3c4b5d");
        // static cateogry data fields
        public static readonly Guid VALID_CATEGORY_ID_1 = Guid.Parse("c4b5a1e1-5f9c-4b0c-8c2d-0f1e2d3c4b5a");
        public static readonly Guid VALID_CATEGORY_ID_2 = Guid.Parse("c4b5a1e1-5f9c-4b0c-8c2d-0f1e2d3c4b5b");
        public static readonly Guid VALID_CATEGORY_ID_3 = Guid.Parse("c4b5a1e1-5f9c-4b0c-8c2d-0f1e2d3c4b5c");
        public static readonly Guid INVALID_CATEGORY_ID = Guid.Parse("c4b5a1e1-5f9c-4b0c-8c2d-0f1e2d3c4b5d");
        // static user data fields
        public static readonly Guid VALID_USER_ID_1 = Guid.Parse("69c47d10-1f90-43ab-8838-7d628a597d24");
        public static readonly Guid VALID_USER_ID_2 = Guid.Parse("71e70c14-2ef1-4df9-8094-031d49cf1864");
        public static readonly Guid INVALID_USER_ID = Guid.Parse("4a00312e-8361-4703-89bd-09ecdc4c5771");

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
    }
}