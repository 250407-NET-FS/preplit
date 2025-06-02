import { useState, useEffect } from "react";
import { api } from "../services/api";
import type {Category } from "../../../types/Category";

function CategoryList() {
  const [categories, setCategories] = useState([] as Category[]);
  const token = localStorage.getItem("jwt");

  useEffect(() => {
    api
      .get("properties/admin")
      .then((res) => setCategories(res.data))
      .catch((err) => console.error(err));
  }, []);

  const deleteHandler = (categoryId: string, ownerId: string) => {
    api
      .delete(
        `properties/admin/${categoryId}/${ownerId}`)
      .then(() => {
        setCategories((prevCards: Category[]) =>
          prevCards.filter((c) => c.categoryId !== categoryId)
        );
      })
      .catch((err) => console.error("Delete failed", err));
  };

  return (
    <>
      <div className="container">
        <h1 className="admin-options">Property List</h1>
        <ul className="cards" style={{listStyle: "none"}}>
          {categories.map((c) => (
            <CategoryCard
              key={c.categoryId}
              id={c.categoryId}
              name={c.name}
              ownerId={c.userId}
              deleteHandler={() => deleteHandler(c.categoryId, c.userId)}
            ></CategoryCard>
          ))}
        </ul>
      </div>
    </>
  );
}

function CategoryCard({
  id,
  name,
  ownerId,
  deleteHandler,
}:
  {
    id: string;
    name: string;
    ownerId: string;
    deleteHandler: () => void;
  }) {
  return (
    <li className="card">
      <h4>{id}</h4>
      <p>Name: {name}</p>
      <p>Owner: {ownerId}</p>{" "}
      {/*Need to make it so we can go to that view imediatly and then choose to ban or not */}
      <p>Owner Id: {ownerId}</p>
      <div className="button-group">
      <button onClick={deleteHandler}>Delete</button>
      </div>
    </li>
  );
}

export default CategoryList;