import React, { createContext, useReducer, useContext, useCallback} from 'react';
import { api } from "../services/api"; 
import axios from 'axios';

const initialState = {
    categoryList: [],
    selectedCategory: {},
    loading: false,
    error: null
};
// These action types should be exclusive for category requests
const CategoryActionTypes = {
    REQUEST_START: "REQUEST_START",
    FETCH_LIST_SUCCESS: "FETCH_LIST_SUCCESS",
    FETCH_CATEGORY_SUCCESS: "FETCH_CATEGORY_SUCCESS",
    CREATE_CATEGORY_SUCCESS: "CREATE_CATEGORY_SUCCESS",
    UPDATE_CATEGORY_SUCCESS: "UPDATE_CATEGORY_SUCCESS",
    DELETE_CATEGORY_SUCCESS: "DELETE_CATEGORY_SUCCESS",
    REQUEST_ERROR: "REQUEST_ERROR"
};

const reducer = (state: typeof initialState , action: { type: string; payload?: any; }) => {
    switch (action.type) {
        case CategoryActionTypes.REQUEST_START:
            return { ...state, loading: true, error: null };
        case CategoryActionTypes.FETCH_LIST_SUCCESS:
            return { ...state, loading: false, categoryList: action.payload };
        case CategoryActionTypes.FETCH_CATEGORY_SUCCESS:
            return { ...state, loading: false, selectedCategory: action.payload};
        case CategoryActionTypes.CREATE_CATEGORY_SUCCESS:
            return { ...state, loading: false, categoryList: [...state.categoryList, action.payload]};
        case CategoryActionTypes.UPDATE_CATEGORY_SUCCESS:
            return { ...state, loading: false, categoryList: state.categoryList.map((category: any) => category.id === action.payload.id ? action.payload : category)};
        case CategoryActionTypes.DELETE_CATEGORY_SUCCESS:
            return { ...state, loading: false,categoryList: state.categoryList.filter((category: any) => category.id !== action.payload)};
        case CategoryActionTypes.REQUEST_ERROR:
            return { ...state, loading: false, error: action.payload };
        default:
            return state;
    }
}

const CategoryContext = createContext({} as any);

export function CategoryProvider({children} : {children: React.ReactNode}) {
    const [state, dispatch] = useReducer(reducer, initialState);

    // Obtain the full list of categories for the category list page (admin)
    const fetchAdminCategoryList = useCallback(async(signal: AbortSignal) => {
        dispatch({type: CategoryActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's GetCategoriesAdmin() to our state
        await api.get("categories/admin", {signal: signal})
        .then (res => res.data)
        .then(data => dispatch({type: CategoryActionTypes.FETCH_LIST_SUCCESS, payload: data}))
        .catch(err => {
            if (axios.isCancel(err)) {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: "Operation cancelled"});
            }
            else {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: err.message});
            }
        });
    }, []);
    // Obtain the full list of categorys for the category list page by category (user)
    const fetchCategoryList = useCallback(async(categoryId: any, signal: AbortSignal) => {
        dispatch({type: CategoryActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's Getcategorys() to our state
        await api.get(`categories`, {params: {categoryId: categoryId, signal: signal}} )
        .then (res => res.data)
        .then(data => dispatch({type: CategoryActionTypes.FETCH_LIST_SUCCESS, payload: data}))
        .catch(err => {
            if (axios.isCancel(err)) {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: "Operation cancelled"});
            }
            else {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: err.message});
            }
        });
    }, []);
    // Obtain a specific category after selecting it from a category list
    const fetchCategory = useCallback(async(categoryId: any, signal: AbortSignal) => {
        dispatch({type: CategoryActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's GetcategoryById() to our state
        await api.get(`categories/${categoryId}`, {signal: signal} )
        .then (res => res.data)
        .then(data => dispatch({type: CategoryActionTypes.FETCH_CATEGORY_SUCCESS, payload: data}))
        .catch(err => {
            if (axios.isCancel(err)) {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: "Operation cancelled"});
            }
            else {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: err.message});
            }
        });
    }, []);
    // Create a category in the app's "create category page"
    const createCategory = useCallback(async(categoryInfo: any, signal: AbortSignal) => {
        dispatch({type: CategoryActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's CreateCategory() to our state
        await api.post(`categories`, {categoryInfo: categoryInfo, signal: signal} )
        .then (res => res.data)
        .then(data => dispatch({type: CategoryActionTypes.CREATE_CATEGORY_SUCCESS, payload: data}))
        .catch(err => {
            if (axios.isCancel(err)) {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: "Operation cancelled"});
            }
            else {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: err.message});
            }
        });
    }, []);
    // Update a category in the app's "update category page"
    const updateCategory = useCallback(async(categoryInfo: any, signal: AbortSignal) => {
        dispatch({type: CategoryActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's Updatecategory() to our state
        await api.put(`categories`, {categoryInfo: categoryInfo, signal: signal} )
        .then (res => res.data)
        .then(data => dispatch({type: CategoryActionTypes.UPDATE_CATEGORY_SUCCESS, payload: data}))
        .catch(err => {
            if (axios.isCancel(err)) {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: "Operation cancelled"});
            }
            else {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: err.message});
            }
        });
    }, []);
    // Delete a category in the app's "update category page"
    const deleteCategory = useCallback(async(categoryId: any, signal: AbortSignal) => {
        dispatch({type: CategoryActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's DeleteCategory() to our state
        await api.delete(`categories/${categoryId}`, {signal: signal} )
        .then (res => res.data)
        .then(data => dispatch({type: CategoryActionTypes.DELETE_CATEGORY_SUCCESS, payload: data}))
        .catch(err => {
            if (axios.isCancel(err)) {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: "Operation cancelled"});
            }
            else {
                dispatch({type: CategoryActionTypes.REQUEST_ERROR, payload: err.message});
            }
        })
    }, []);

    return (
        <CategoryContext.Provider value={{
            ...state, 
            dispatch, 
            fetchAdminCategoryList,
            fetchCategoryList, 
            fetchCategory, 
            createCategory, 
            updateCategory, 
            deleteCategory
        }}>
            {children}
        </CategoryContext.Provider>
    );
}

export const usecategory = () => {
    const categoryContext = useContext(CategoryContext);

    if (!categoryContext) {
        throw new Error("usecategory must be used within a categoryProvider");
    }

    return categoryContext;
};

export {reducer, initialState, CategoryActionTypes, CategoryContext};