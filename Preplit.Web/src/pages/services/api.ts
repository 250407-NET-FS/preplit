import axios from "axios";
import {toast} from "react-toastify";

const baseURL = import.meta.env.DEV 
    ? "http://localhost:5280/api" // Local .NET backend
    : "preplit-back-h3frh3h8hge0eue8.westus2-01.azurewebsites.net/api";

export const api = axios.create({
    baseURL,
    headers: {
        "Content-Type": "application/json"
    }
});
// Request interceptor to attach JWT token to every request
// This interceptor will run before every request made with the Axios instance
// It checks if a JWT token is present in local storage
// and attaches it to the Authorization header of the request
api.interceptors.request.use((config: any) => {
    const token = localStorage.getItem("jwt");

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

api.interceptors.response.use(
    (response: any) => response,
    (error: any) => {
        toast.error(error.message);
        return Promise.reject(error);
    }
);