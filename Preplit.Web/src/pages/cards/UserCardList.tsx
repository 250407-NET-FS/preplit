import {Container, Grid, IconButton} from "@mui/material";
import AddIcon from '@mui/icons-material/Add';
import CloseOutlinedIcon from '@mui/icons-material/CloseOutlined';
import {useCard} from "../contexts/CardContext";
import React, {useEffect, useState, type JSX} from "react";
import Popup from "reactjs-popup";
import CreateCard from "./CreateCard";
import VisualCard from "./VisualCard";
import type { FlashCard} from "../../../types/FlashCard";
import type { Category } from "../../../types/Category";

function UserCardList({category}: {category: Category}) {
    const { cardList, fetchCardList } = useCard();

    const [createPopupOpen, setCreatePopupOpen] = useState(false);

    useEffect(() => {
        const controller = new AbortController();
        fetchCardList(category.categoryId, controller.signal);
        return () => controller.abort();
    }, [fetchCardList]);

    let cardNodeList: JSX.Element[] = cardList.map((card: FlashCard) => (
        <Grid size={{ xs: 12, sm: 8, md: 6, lg: 4}} key={card.cardId}>
            <VisualCard card={card} />
        </Grid>
    ));

    const handleCreate = () => {
        setCreatePopupOpen(true);
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
                    background: "rgba(252, 90, 141, 0.75)",
                    boxShadow: "0px 4px 12px rgba(0, 0, 0, 0.2)",
                    fontFamily: "Arial, sans-serif",
                    position: 'relative',
                    overflowY: 'auto',
                }}
            >
                <div>
                    <IconButton
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
                        <CloseOutlinedIcon />
                    </IconButton>
                    <CreateCard categoryId={category.categoryId}/>
                </div>
            </Popup>
        </Container>
    )
}

export default UserCardList;