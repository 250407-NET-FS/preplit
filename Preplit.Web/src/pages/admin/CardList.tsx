import React, { useState, useEffect } from "react";
import { api } from "../services/api";
import type {FlashCard } from "../../../types/FlashCard";

function CardList() {
  const [cards, setCards] = useState([] as FlashCard[]);

  useEffect(() => {
    api
      .get("cards/admin")
      .then((res: any) => setCards(res.data))
      .catch((err) => console.error(err));
  }, []);

  const deleteHandler = (cardId: string, ownerId: string) => {
    api
      .delete(
        `cards/admin/${cardId}/${ownerId}`)
      .then(() => {
        setCards((prevCards: FlashCard[]) =>
          prevCards.filter((c) => c.cardId !== cardId)
        );
      })
      .catch((err) => console.error("Delete failed", err));
  };

  return (
    <>
      <div className="container">
        <h1 className="admin-options">Card List</h1>
        <ul className="cards" style={{listStyle: "none"}}>
          {cards.map((c) => (
            <VisualCard
              key={c.cardId}
              id={c.cardId}
              question={c.question}
              answer={c.answer}
              ownerId={c.userId}
              deleteHandler={() => deleteHandler(c.cardId, c.userId)}
            ></VisualCard>
          ))}
        </ul>
      </div>
    </>
  );
}

function VisualCard({
  id,
  question,
  answer,
  ownerId,
  deleteHandler,
}:
  {
    id: string;
    question: string;
    answer: string;
    ownerId: string;
    deleteHandler: () => void;
  }) {
  return (
    <li className="card" style={{ color: "white"}}>
      <h4>{id}</h4>
      <p>Question: {question}</p>
      <p>Answer: {answer}</p>
      {/*Need to make it so we can go to that view imediatly and then choose to ban or not */}
      <p>Owner Id: {ownerId}</p>
      <div className="button-group">
      <button onClick={deleteHandler}>Delete</button>
      </div>
    </li>
  );
}

export default CardList;