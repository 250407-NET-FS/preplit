import React, { useState } from 'react';
import { useCard } from '../contexts/CardContext';
import { useNavigate } from 'react-router-dom';
import { Alert, Container, FormControl, FormGroup, FormLabel, Grid, Input } from '@mui/material';
import type { FlashCard } from '../../../types/FlashCard';

function UpdateCard({ card }: { card: FlashCard }) {
    const { updateCard } = useCard();
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [updateInfo, setUpdateInfo] = useState({
        cardId: card.cardId,
        question: card.question,
        answer: card.answer,
        categoryId: card.categoryId
    });

    const navigate = useNavigate();
    const controller = new AbortController();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            await updateCard(updateInfo, controller.signal);
            setSuccessMessage('Card updated successfully!');
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
            <h1>Update Card</h1>

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
                <Grid size={12}>
                    <form method='post' onSubmit={handleSubmit}>
                        <FormGroup>
                            <FormLabel>Question</FormLabel>
                            <FormControl>
                                <Input type="text" name="question" 
                                    value={updateInfo.question} 
                                    onChange={(e) => setUpdateInfo({ ...updateInfo, question: e.target.value })} 
                                    required
                                />
                            </FormControl>
                        </FormGroup>
                        <FormGroup>
                            <FormLabel>Answer</FormLabel>
                            <FormControl>
                                <Input type="text" name="answer" 
                                    value={updateInfo.answer} 
                                    onChange={(e) => setUpdateInfo({ ...updateInfo, answer: e.target.value })} 
                                />
                            </FormControl>
                        </FormGroup>
                        <FormGroup>
                            <Input type="submit" value="Update Card" color='primary'/>
                        </FormGroup>
                    </form>
                </Grid>
            </Grid>
        </Container>
    )
}

export default UpdateCard;