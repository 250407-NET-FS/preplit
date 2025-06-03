import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useCategory } from '../contexts/CategoryContext';
import { useNavigate } from 'react-router-dom';
import { Alert, Button, Container, Grid } from '@mui/material';

const DeleteCategory = ({ id }: { id: string }) => {
    const { deleteCategory } = useCategory();
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    const navigate = useNavigate();

    const handleClick = async(e: React.FormEvent) => {
        e.preventDefault();

        try {
            await deleteCategory(id);
            setSuccessMessage('Category deleted successfully!');
            setErrorMessage(null);
            navigate("/categories");
        } catch (errorMessage: unknown) {
            setErrorMessage(errorMessage as string);
            setSuccessMessage(null);
            return;
        }
    }

    return (
        <Container>
            <h1>Are you sure you want to delete this category?</h1>

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
                    <Button variant='contained' component={Link} to="/categories">No</Button>
                </Grid>
            </Grid>       
        </Container>
    )
}

export default DeleteCategory;