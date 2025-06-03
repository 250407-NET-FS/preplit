import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useCard } from '../contexts/CardContext';
import type { FlashCard } from '../../../types/FlashCard';
import { useNavigate } from 'react-router-dom';
import { Alert, Button, Container, Grid } from '@mui/material';

const DeleteCard = ({ card }: { card: FlashCard }) => {
    const { deleteCard } = useCard();
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    const navigate = useNavigate();

    const handleClick = async(e: React.FormEvent) => {
        e.preventDefault();

        try {
            await deleteCard(card.cardId);
            setSuccessMessage('Card deleted successfully!');
            setErrorMessage(null);
            navigate(`/categories/${card.categoryId}`);
        } catch (errorMessage: unknown) {
            setErrorMessage(errorMessage as string);
            setSuccessMessage(null);
            return;
        }
    }

    return (
        <Container>
            <h1>Are you sure you want to delete this card?</h1>

            {/* Alert Messages */}
            {errorMessage && (
                <Alert variant="outlined">
                {errorMessage}
                </Alert>
            )}
            {successMessage && (
                <Alert variant="outlined">
                {successMessage}
                </Alert>
            )}

            <hr />
            <Grid container>
                <Grid>
                    <Button variant='contained' onClick={handleClick}>Yes</Button>
                    <Button variant='contained' component={Link} to={`/categories/${card.categoryId}`} state={card.categoryId}>No</Button>
                </Grid>
            </Grid>       
        </Container>
    )           
}

export default DeleteCard