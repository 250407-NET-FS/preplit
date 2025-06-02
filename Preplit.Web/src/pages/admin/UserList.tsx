import { useEffect, useState } from "react";
import { api } from "../services/api";
import { useAuth } from "../contexts/AuthContext";
import type {User} from "../../../types/User";

function UserList() {
  const [users, setUsers] = useState([] as User[]);
  const { user: admin } = useAuth();

  useEffect(() => {
    api
      .get("user/admin")
      .then((res) => setUsers(res.data))
      .catch((err) => console.error(err));
  }, []);

  const banHandler = (userId: string) => {
    api
      .post(`user/admin/ban/${userId}`)
      .then((res) => {
        const updatedUser = res.data;
        setUsers((prevUsers) =>
          prevUsers.map((user) =>
            user.id === updatedUser.id
              ? { ...user, lockoutEnabled: true }
              : user
          )
        );
      })
      .catch((err) => console.error("Ban failed: ", err.message));
  };

  const unbanHandler = (userId: string) => {
    console.log("Sending unban request for:", userId);
    api
      .post(`user/admin/unban/${userId}`)
      .then((res) => {
        const updatedUser = res.data;
        setUsers((prevUsers) =>
          prevUsers.map((user) =>
            user.id === updatedUser.id
              ? { ...user, lockoutEnabled: false }
              : user
          )
        );
      })
      .catch((err) => console.error("Unban failed: ", err.message));
  };

  const deleteHandler = (userId: string) => {
    api
      .delete(`user/admin/${userId}`)
      .then(() => {
        setUsers((prevUsers) => prevUsers.filter((u) => u.id !== userId));
      })
      .catch((err) => console.error("Delete failed ", err.message));
  };

  return (
    <>
      <div className="container">
        <h1 className="admin-options">User List</h1>
        <ul className="cards" style={{listStyle: "none"}}>
          {users.map((u) => (
            <UserCard
              key={u.id}
              name={u.userName}
              id={u.id}
              email={u.email}
              lockoutEnabled={u.lockoutEnabled}
              isAdmin={u.id === admin.id}
              banHandler={() => banHandler(u.id)}
              unbanHandler={() => unbanHandler(u.id)}
              deleteHandler={() => deleteHandler(u.id)}
            ></UserCard>
          ))}
        </ul>
      </div>
    </>
  );
}

function UserCard({
  name,
  id,
  email,
  lockoutEnabled,
  isAdmin,
  banHandler,
  unbanHandler,
  deleteHandler,
} : {
  name: string;
  id: string;
  email: string;
  lockoutEnabled: boolean;
  isAdmin: boolean;
  banHandler: () => void;
  unbanHandler: () => void;
  deleteHandler: () => void;
}) {
  return (

    <li className="card">
      <h4>{name}</h4>
      <p>Id: {id}</p>
      <p>Email: {email}</p>
      <div className="button-group">
      {!isAdmin && (
        <>
          {lockoutEnabled ? (
            <button onClick={unbanHandler}>Unban</button>
          ) : (
            <button onClick={banHandler}>Ban</button>
          )}
          <button onClick={deleteHandler}>Delete</button>
        </>
      )}
      </div>
    </li>
  );
}

export default UserList;