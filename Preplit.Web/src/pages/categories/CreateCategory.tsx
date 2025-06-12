import React, { useState } from 'react';
import { useCategory } from '../contexts/CategoryContext';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { Container, Grid, FormGroup, FormControl, FormLabel, Input } from '@mui/material';

function CreateCategory() {
    const { createCategory } = useCategory();
    const {user} = useAuth();

    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [categoryInfo, setCategoryInfo] = useState({
        name: "",
        userId: user?.id
    });

    const navigate = useNavigate();
    const controller = new AbortController();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            await createCategory(categoryInfo, controller.signal);
            setSuccessMessage('Category created successfully!');
            setErrorMessage(null);
            navigate("/categories");
            window.location.reload();
        } catch (errorMessage: unknown) {
            setErrorMessage(errorMessage as string);
            setSuccessMessage(null);
            return;
        }
    }

    return (
        <Container>
            <h1>Create Category</h1>
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
                                <FormLabel>Category Name</FormLabel>
                                <Input
                                    type="text"
                                    name="name"
                                    value={categoryInfo.name}
                                    onChange={(e) => setCategoryInfo({ ...categoryInfo, name: e.target.value })}
                                />
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

export default CreateCategory;