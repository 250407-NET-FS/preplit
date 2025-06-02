let validCardId1 = "67d31a31-05fe-4f32-94ce-cd2ccf52ec30";
let validCardId2 = "940178fa-2168-42b8-a9a4-2dffdb218819";
let validCategoryId1 = "3426b57b-d948-4141-90a1-e2d018fe01b7";
let validCategoryId2 = "f7b74855-de9e-42df-914e-116cc6dfb3b3";
let validUserId1 = "a5e27962-3013-4de1-8f5d-c9c1a55e85c4";
let validUserId2 = "507f4586-e18f-4e0d-a4c2-6fe639a38092";

const sharedData = {
    cardList: [
        {
            cardId: validCardId1,
            question: "Question 1",
            answer: "Answer 1",
            categoryId: validCategoryId1,
            userId: validUserId1
        },
        {
            cardId: validCardId2,
            question: "Question 2",
            answer: "Answer 2",
            categoryId: validCategoryId2,
            userId: validUserId1
        }
    ],
    categoryList: [
        {
            categoryId: validCategoryId1,
            name: "Category 1",
            userId: validUserId1
        },
        {
            categoryId: validCategoryId2,
            name: "Category 2",
            userId: validUserId1
        }
    ],
    userList: [
        {
            userId: validUserId1,
            userName: "user1",
            email: "user1@test.com",
        },
        {
            userId: validUserId2,
            userName: "user2",
            email: "user2@test.com",
        }
    ]
}

export default sharedData;