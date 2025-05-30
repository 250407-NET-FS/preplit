import axios, { type AxiosResponse, type InternalAxiosRequestConfig } from "axios";
import {toast} from "react-toastify";

const baseURL = import.meta.env.DEV ? "https://localhost:5280/api" : "https://preplitapi.azurewebsites.net/api";

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
api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem("jwt");

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

api.interceptors.response.use(
    (response: AxiosResponse<any, any>) => response,
    (error: { response: { data: { message: string; }; }; }) => {
        toast.error(error.response.data.message);
        return Promise.reject(error);
    }
);