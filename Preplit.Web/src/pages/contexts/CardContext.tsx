import React, { createContext, useReducer, useContext, useCallback} from 'react';
import { api } from "../services/api"; 
import type { FlashCard } from "../../../types/FlashCard";

const initialState = {
    cardList: [] as FlashCard[],
    selectedCard: {} as FlashCard,
    loading: false,
    error: null as string | null
};
// These action types should be exclusive for card requests
const CardActionTypes = {
    REQUEST_START: "REQUEST_START",
    FETCH_LIST_SUCCESS: "FETCH_LIST_SUCCESS",
    FETCH_CARD_SUCCESS: "FETCH_CARD_SUCCESS",
    CREATE_CARD_SUCCESS: "CREATE_CARD_SUCCESS",
    UPDATE_CARD_SUCCESS: "UPDATE_CARD_SUCCESS",
    DELETE_CARD_SUCCESS: "DELETE_CARD_SUCCESS",
    REQUEST_ERROR: "REQUEST_ERROR"
};

const reducer = (state: typeof initialState, action: { type: string; payload?: unknown; }) => {
    switch (action.type) {
        case CardActionTypes.REQUEST_START:
            return { ...state, loading: true, error: null };
        case CardActionTypes.FETCH_LIST_SUCCESS:
            return { ...state, loading: false, cardList: action.payload as FlashCard[] };
        case CardActionTypes.FETCH_CARD_SUCCESS:
            return { ...state, loading: false, selectedCard: action.payload as FlashCard };
        case CardActionTypes.CREATE_CARD_SUCCESS:
            return { ...state, loading: false, cardList: [...state.cardList, action.payload as FlashCard]};
        case CardActionTypes.UPDATE_CARD_SUCCESS:
            return { ...state, loading: false, cardList: state.cardList.map((card: FlashCard) => card.cardId === (action.payload as FlashCard).cardId ? action.payload as FlashCard : card)};
        case CardActionTypes.DELETE_CARD_SUCCESS:
            return { ...state, loading: false,cardList: state.cardList.filter((card: FlashCard) => card.cardId !== (action.payload as FlashCard).cardId)};
        case CardActionTypes.REQUEST_ERROR:
            return { ...state, loading: false, error: action.payload as string };
        default:
            return state;
    }
}

const CardContext = createContext({} as any);

export function CardProvider({children} : {children: React.ReactNode}) {
    const [state, dispatch] = useReducer(reducer, initialState);

    // Obtain the full list of cards for the card list page (admin)
    const fetchAdminCardList = useCallback(async(signal: AbortSignal) => {
        dispatch({type: CardActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's GetAllCards() to our state
        await api.get("cards/admin", {signal: signal} as any)
        .then ((res: any) => res.data)
        .then((data: FlashCard[]) => dispatch({type: CardActionTypes.FETCH_LIST_SUCCESS, payload: data}))
        .catch((err: any) => {
            dispatch({type: CardActionTypes.REQUEST_ERROR, payload: err.message});
        });
    }, []);
    // Obtain the full list of cards for the card list page by category (user)
    const fetchCardList = useCallback(async(categoryId: any, signal: AbortSignal) => {
        dispatch({type: CardActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's GetCards() to our state
        await api.get(`cards/category/${categoryId}`, {signal: signal} as any)
        .then ((res: any) => res.data)
        .then((data: FlashCard[]) => dispatch({type: CardActionTypes.FETCH_LIST_SUCCESS, payload: data}))
        .catch((err: any) => {
            dispatch({type: CardActionTypes.REQUEST_ERROR, payload: err.message});
        });
    }, []);
    // Obtain a specific card after selecting it from a card list
    const fetchCard = useCallback(async(cardId: any, signal: AbortSignal) => {
        dispatch({type: CardActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's GetCardById() to our state
        await api.get(`cards/${cardId}`, {signal: signal} as any)
        .then ((res: any) => res.data)
        .then((data: FlashCard) => dispatch({type: CardActionTypes.FETCH_CARD_SUCCESS, payload: data}))
        .catch((err: any) => {
            dispatch({type: CardActionTypes.REQUEST_ERROR, payload: err.message});
        });
    }, []);
    // Create a card in the app's "create card page"
    const createCard = useCallback(async(cardInfo: any, signal: AbortSignal) => {
        dispatch({type: CardActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's CreateCard() to our state
        await api.post(`cards`, cardInfo, {signal: signal} as any)
        .then ((res: any) => res.data)
        .then((data: FlashCard) => dispatch({type: CardActionTypes.CREATE_CARD_SUCCESS, payload: data}))
        .catch((err: any) => {
            dispatch({type: CardActionTypes.REQUEST_ERROR, payload: err.message});
        });
    }, []);
    // Update a card in the app's "update card page"
    const updateCard = useCallback(async(cardInfo: any, signal: AbortSignal) => {
        dispatch({type: CardActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's UpdateCard() to our state
        await api.put(`cards`, cardInfo, {signal: signal} as any)
        .then ((res: any) => res.data)
        .then((data: FlashCard) => dispatch({type: CardActionTypes.UPDATE_CARD_SUCCESS, payload: data}))
        .catch((err: any) => {
            dispatch({type: CardActionTypes.REQUEST_ERROR, payload: err.message});
        });
    }, []);
    // Delete a card in the app's "update card page"
    const deleteCard = useCallback(async(cardId: any, signal: AbortSignal) => {
        dispatch({type: CardActionTypes.REQUEST_START});
        // Try to fetch and pass the results of controller's DeleteCard() to our state
        await api.delete(`cards/${cardId}`, {signal: signal} as any)
        .then ((res: any) => res.data)
        .then((data: FlashCard) => dispatch({type: CardActionTypes.DELETE_CARD_SUCCESS, payload: data}))
        .catch((err: any) => {
            dispatch({type: CardActionTypes.REQUEST_ERROR, payload: err.message});
        })
    }, []);

    return (
        <CardContext.Provider value={{
            ...state, 
            dispatch, 
            fetchAdminCardList,
            fetchCardList, 
            fetchCard, 
            createCard, 
            updateCard, 
            deleteCard
        }}>
            {children}
        </CardContext.Provider>
    );
}

export const useCard = () => {
    const cardContext = useContext(CardContext);

    if (!cardContext) {
        throw new Error("useCard must be used within a CardProvider");
    }

    return cardContext;
};

export {reducer, initialState, CardActionTypes, CardContext};