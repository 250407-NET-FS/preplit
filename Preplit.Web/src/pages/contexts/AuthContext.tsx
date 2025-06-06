import React, { createContext, useContext, useReducer, useEffect, useCallback } from "react";
import { jwtDecode, type JwtHeader, type JwtPayload } from "jwt-decode";
import { api } from "../services/api";

// cleans up claims to nice and neat strings
const normalizeClaims = (decoded: JwtPayload | JwtHeader | any) => ({
  email:
    decoded[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
    ],
  id: decoded[
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
  ],
  role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
  fullName: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
});

const initialState = {
    user: {} as object | null,
    isAuthenticated: false
}

// To be used exclusively for authentication requests
export const reducer = (state: typeof initialState, action: { type: string; payload?: any; }) => {
    switch(action.type){
        case "LOGIN":
            return {...state, user: action.payload, isAuthenticated: true};
        case "REGISTER":
            return {...state, user: action.payload, isAuthenticated: false};
        case "LOGOUT":
            return {...state, user: null, isAuthenticated: false};
        default:
            return state;
    }
};

const AuthContext = createContext({} as any);

// main component
export const AuthProvider = ({ children }: {children: React.ReactNode}) => {
    // initialize default state values
    const [state, dispatch] = useReducer(reducer, {
        user: null,
        isAuthenticated: false
    });

    // function to check if token exists and is not expired
    const checkTokenValidity = () => {
        // accesses localstorage to get token
        const token = localStorage.getItem("jwt");
        if(!token){
            return false;
        }
        try{
            const decoded = jwtDecode(token);
            return decoded.exp! * 1000 > Date.now();
        } catch(error){
            return false;
        }
    };

    // function to handle login action
    const login = useCallback(async (credentials: any, signal: AbortSignal) => {
        // sends post request to api service with base url attached to front and
        // credentials attached to body
        await api.post("auth/login", credentials, {signal: signal} as any)
        .then((res: any) => {
            if (res.status < 200 || res.status >= 300) {
                return false;
            }

            return res.data.token
        })
        .then (token => {
            // assigns token to localStorage
            localStorage.setItem("jwt", token);
            const decoded = jwtDecode(token);
            dispatch({type: "LOGIN", payload: normalizeClaims(decoded)});
            return true;
        })
        .catch(err => {
            dispatch({type: "REQUEST_ERROR", payload: err.message});
        });
    }, []);

    // function to handle logout action
    const logout = () => {
        localStorage.removeItem("jwt");
        dispatch({type: "LOGOUT"});
    };

    const register = async (credentials: any) => {
        await api.post("auth/register", credentials)
        .then((res: any) => {
            // Check if the status is successful
            if (res.status >= 200 && res.status < 300) {
                // Check if Auth Controller sends good message
                if (res.data && res.data.message) {
                    console.log(res.data.message);
                    return true;  // Registration is successful
                }

                console.error("No success message.");
                return false;
            } else {
                console.error("Failed to register:", res.status, res.data);
                return false;
            }
        })
        .catch(err => {
            // Handle errors (network issues or unexpected issues)
            if (!err.response) {
                console.error("Network error or no response from server:", err.message);
            } else {
                console.error("Registration Error:", err.response.status, err.response.data);
            }
            return false;
        });
};

    // user should remain logged in when page refreshes
    // useEffect triggers once when component is mounting
    // simply checks if the user has a valid token and
    // logs the user in if they do
    useEffect(() => {
        if(checkTokenValidity()){
            const token = localStorage.getItem("jwt");
            const decode = jwtDecode(token!);
            dispatch({type: "LOGIN", payload: normalizeClaims(decode)});
        }
    }, []);

    return(
        <AuthContext.Provider
            value = {{...state, login, logout, register, checkTokenValidity}}
        >
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const authContext = useContext(AuthContext)

    if (!authContext){
        throw new Error("useAuth must be used within AuthContextProvider");
    }

    return authContext
};