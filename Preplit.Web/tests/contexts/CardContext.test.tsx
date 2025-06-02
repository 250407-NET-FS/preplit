import { reducer, initialState, CardActionTypes } from "../../src/pages/contexts/CardContext";
import type { Card } from "../../types/Card";
import sharedData from "../test-data/sharedData";

describe("card reducer", () => {
    it("returns the current state for default action types", () => {
        const prevState = { ...initialState, loading: true, error: null };
        const result = reducer(prevState, { type: "DEFAULT" });
        expect(result).toEqual(prevState);
    });

    it("handles REQUEST_START", () => {
        const prevState = { ...initialState, loading: false, error: null };
        const action = { type: CardActionTypes.REQUEST_START };
        const result = reducer(prevState, action);
        expect(result).toEqual({ ...initialState, loading: true, error: null });
    });

  it("handles FETCH_LIST_SUCCESS", () => {
    const action = {
      type: CardActionTypes.FETCH_LIST_SUCCESS,
      payload: sharedData.cardList,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      cardList: sharedData.cardList,
    });
  });

  it("handles FETCH_CARD_SUCCESS", () => {
    const card = sharedData.cardList[0];
    const action = {
      type: CardActionTypes.FETCH_CARD_SUCCESS,
      payload: card,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      selectedProperty: card,
    });
  });

  it("handles CREATE_CARD_SUCCESS", () => {
    const newCard = { 
      cardId: "baef5434-a9ee-46c3-877a-7bdbedce7403", 
      question: "New Question", 
      answer: "New Answer", 
      categoryId: sharedData.categoryList[0].categoryId, 
      userId: sharedData.userList[0].userId
    };
    const prevState = { ...initialState, loading: true, cardList: sharedData.cardList };
    const action = {
      type: CardActionTypes.CREATE_CARD_SUCCESS,
      payload: newCard,
    };
    const result = reducer(prevState, action);
    expect(result).toEqual({
      ...prevState,
      loading: false,
      cardList: [...prevState.cardList, action.payload]
    });
  });

  it("handles UPDATE_CARD_SUCCESS", () => {
    const updated = { ...sharedData.cardList[0], categoryId: sharedData.categoryList[1].categoryId };
    const action = {
      type: CardActionTypes.UPDATE_CARD_SUCCESS,
      payload: updated,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      cardList: initialState.cardList.map((card: Card) => card.cardId === action.payload.cardId ? action.payload : card),
    });
  });

  it("handles DELETE_CARD_SUCCESS", () => {
    const deleted = sharedData.cardList[0];
    const action = {
      type: CardActionTypes.DELETE_CARD_SUCCESS,
      payload: deleted,
    };
    const result = reducer({ ...initialState, loading: true }, action);
    expect(result).toEqual({
      ...initialState,
      loading: false,
      cardList: initialState.cardList.filter((card: Card) => card.cardId !== action.payload.cardId),
    });
  });

  it("handles REQUEST_ERROR", () => {
    const errorMsg = "Failed to fetch";
    const action = {
      type: CardActionTypes.REQUEST_ERROR,
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