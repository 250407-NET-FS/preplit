import { reducer, initialState, CategoryActionTypes } from "../../pages/contexts/CategoryContext";
import type { Category } from "../../pages/contexts/CategoryContext";
import sharedData from "../test-data/sharedData";

describe("card reducer", () => {
    it("returns the current state for default action types", () => {
        const prevState = { ...initialState, loading: true, error: null };
        const result = reducer(prevState, { type: "DEFAULT" });
        expect(result).toEqual(prevState);
    });

    it("handles REQUEST_START", () => {
        const prevState = { ...initialState, loading: false, error: null };
        const action = { type: CategoryActionTypes.REQUEST_START };
        const result = reducer(prevState, action);
        expect(result).toEqual({ ...initialState, loading: true, error: null });
    });

  it("handles FETCH_LIST_SUCCESS", () => {
    const action = {
      type: CategoryActionTypes.FETCH_LIST_SUCCESS,
      payload: sharedData.categoryList,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      propertyList: sharedData.categoryList,
    });
  });

  it("handles FETCH_CATEGORY_SUCCESS", () => {
    const property = sharedData.categoryList[0];
    const action = {
      type: CategoryActionTypes.FETCH_CATEGORY_SUCCESS,
      payload: property,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      selectedProperty: property,
    });
  });

  it("handles CREATE_CATEGORY_SUCCESS", () => {
    const newCategory = {
      categoryId: "ca7ccedd-718f-4eb3-a04c-a887bd5cdbf3",
      name: "New Category",
      userId: sharedData
    };
    const prevState = { ...initialState, loading: true, categoryList: sharedData.categoryList };
    const action = {
      type: CategoryActionTypes.CREATE_CATEGORY_SUCCESS,
      payload: newCategory,
    };
    const result = reducer(prevState, action);
    expect(result).toEqual({
      ...prevState,
      loading: false,
      categoryList: [...prevState.categoryList, action.payload],
    });
  });

  it("handles UPDATE_CATEGORY_SUCCESS", () => {
    const updated = { ...sharedData.categoryList[0], name: "Updated Category" };
    const action = {
      type: CategoryActionTypes.UPDATE_CATEGORY_SUCCESS,
      payload: updated,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      categoryList: initialState.categoryList.map((category: Category) => category.categoryId === action.payload.categoryId ? action.payload : category),
    });
  });

  it("handles DELETE_CATEGORY_SUCCESS", () => {
    const deleted = sharedData.categoryList[0];
    const action = {
      type: CategoryActionTypes.DELETE_CATEGORY_SUCCESS,
      payload: deleted,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      categoryList: initialState.categoryList.filter((category: Category) => category.categoryId !== action.payload.categoryId),
    });
  });

  it("handles REQUEST_ERROR", () => {
    const errorMsg = "Failed to fetch";
    const action = {
      type: CategoryActionTypes.REQUEST_ERROR,
      payload: errorMsg,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      error: errorMsg,
    });
  });
});