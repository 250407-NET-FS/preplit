import React, { useState } from 'react';
import { useCard } from '../contexts/CardContext';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { Container, Grid, FormGroup, FormControl, FormLabel, Input } from '@mui/material';

function CreateCard({categoryId}: {categoryId: string}) {
    const { createCard } = useCard();
    const {user} = useAuth();

    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [cardInfo, setCardInfo] = useState({
        question: "",
        answer: "",
        categoryId: categoryId,
        ownerId: user?.id
    });

    const navigate = useNavigate();
    const controller = new AbortController();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            console.log('Creating card with:', cardInfo);
            await createCard(cardInfo, controller.signal);
            setSuccessMessage('Card created successfully!');
            setErrorMessage(null);
            navigate(`/categories`);
            window.location.reload();
        } catch (errorMessage: unknown) {
            setErrorMessage(errorMessage as string);
            setSuccessMessage(null);
            return;
        }
    }

    return (
        <Container>
            <h1>Create Card</h1>
            {/* Alert Messages */}
            {errorMessage && (
                <div className="alert alert-danger" role="alert">
                {errorMessage}
                </div>
            )}

            {successMessage && (
                <div className="alert alert-success" role="alert">
                {successMessage}
                </div>
            )}
            <hr />
            <Grid container>
                <Grid size={12}>
                    <form onSubmit={handleSubmit}>
                        <FormGroup>
                            <FormControl>
                                <FormLabel>Question</FormLabel>
                                <Input type="text" value={cardInfo.question} 
                                    onChange={(e) => setCardInfo({ ...cardInfo, question: e.target.value })} 
                                    required
                                />
                            </FormControl>
                            <FormControl>
                                <FormLabel>Answer</FormLabel>
                                <Input type="text" value={cardInfo.answer} onChange={(e) => setCardInfo({ ...cardInfo, answer: e.target.value })} />
                            </FormControl>
                            <FormGroup>
                                <Input type='submit' value='Create' color='primary'/>
                            </FormGroup>
                        </FormGroup>
                    </form>
                </Grid>
            </Grid>            
        </Container>
    )
}

export default CreateCard;