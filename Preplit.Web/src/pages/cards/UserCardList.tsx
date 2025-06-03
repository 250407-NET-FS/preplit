import {Container, Grid, IconButton} from "@mui/material";
import AddIcon from '@mui/icons-material/Add';
import {useCard} from "../contexts/CardContext";
import {useAuth} from "../contexts/AuthContext";
import {useEffect, useState, type JSX} from "react";
import Popup from "reactjs-popup";
import CreateCard from "./CreateCard";
import VisualCard from "./VisualCard";
import type { FlashCard} from "../../../types/FlashCard";
import type { Category } from "../../../types/Category";

function UserCardList({category}: {category: Category}) {
    const { user } = useAuth();
    const { cardList, selectedCard, fetchCardList, fetchCard, createCard, updateCard, deleteCard } = useCard();

    const [selectedProp, setSelectedProp] = useState(null);
    const [createPopupOpen, setCreatePopupOpen] = useState(false);
    const [updatepopupOpen, setUpdatePopupOpen] = useState(false);
    const [deletePopupOpen, setDeletePopupOpen] = useState(false);

    useEffect(() => {
        fetchCardList(category.categoryId);
    }, [fetchCardList]);

    let cardNodeList: JSX.Element[] = cardList.map((card: FlashCard) => (
        <Grid size={{ xs: 12, sm: 6, md: 4, lg: 3}} key={card.cardId}>
            <VisualCard card={card} />
        </Grid>
    ));

    const handleCreate = () => {
        setCreatePopupOpen(true);
    };

    const handleUpdate = (cardId: string) => {
        const card = cardList.find((card: FlashCard) => card.cardId === cardId);
        if (card) {
            setSelectedProp(card);
            setUpdatePopupOpen(true);
        }
    };

    const handleDelete = (cardId: string) => {
        const card = cardList.find((card: FlashCard) => card.cardId === cardId);
        if (card) {
            setSelectedProp(card);
            setDeletePopupOpen(true);
        }
    };

    return (
        <Container>
            <Grid container spacing={2}>
                <Grid size={8}>
                    <h3>{category.name}</h3>
                </Grid>
                <Grid size={4}>
                    <IconButton onClick={handleCreate}>
                        <AddIcon />
                        <p>Add Card</p>
                    </IconButton>
                </Grid>
                {cardNodeList}
            </Grid>
            <Popup
                open={createPopupOpen}
                closeOnDocumentClick
                onClose={() => setCreatePopupOpen(false)}
                modal
                nested
                overlayStyle={{
                    background: "rgba(0, 0, 0, 0.5)",
                }}
                contentStyle={{
                    borderRadius: "10px",
                    padding: "30px",
                    maxWidth: "80vw",
                    width: "80%",
                    height: '80vh',
                    margin: "auto",
                    boxShadow: "0px 4px 12px rgba(0, 0, 0, 0.2)",
                    fontFamily: "Arial, sans-serif",
                    position: 'relative',
                    overflowY: 'auto',
                }}
            >
                <div>
                    <button
                        onClick={() => setCreatePopupOpen(false)}
                        style={{
                            position: 'absolute',
                            top: '10px',
                            right: '10px',
                            background: 'none',
                            border: 'none',
                            fontSize: '24px',
                            cursor: 'pointer',
                            color: 'black',
                        }}
                    >
                        ×
                    </button>
                    <CreateCard />
                </div>
            </Popup>
        </Container>
    )
}

export default UserCardList;