import { Button, Container, Grid, IconButton } from "@mui/material";
import AddIcon from '@mui/icons-material/Add';
import { useCategory } from "../contexts/CategoryContext";
import { useAuth } from "../contexts/AuthContext";
import { useEffect, useState, type JSX } from "react";
import Popup from "reactjs-popup";
//import CreateCategory from "./CreateCategory";
import NavBar from "../shared/NavBar";
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';
import { api } from "../services/api";
import CategoryCard from "./CategoryCard";
import type { Category } from "../../../types/Category";

function UserCategoryList() {
    const { user } = useAuth();
    const { categoryList, selectedCategory, fetchCategoryList, fetchCategory, createCategory, updateCategory, deleteCategory } = useCategory();

    const [selectedProp, setSelectedProp] = useState(null);
    const [cardListOpen, setCardListOpen] = useState(false);
    const [createPopupOpen, setCreatePopupOpen] = useState(false);
    const [update, setUpdate] = useState(false);
    const [deletePopupOpen, setDeletePopupOpen] = useState(false);

    useEffect(() => {
        fetchCategoryList();
    }, [fetchCategoryList]);

    let categoryNodeList: JSX.Element[] = categoryList.map((category: Category) => (
        <Grid size={{ xs: 12, sm: 6, md: 4, lg: 3}} key={category.categoryId}>
            <CategoryCard category={category} />
        </Grid>
    ));

    const handleClick = (categoryId: string) => {
        const category = categoryList.find((category: Category) => category.categoryId === categoryId);
        if (category) {
            setSelectedProp(category);
            setCardListOpen(true);
        }
    };

    const handleCreate = () => {
        setCreatePopupOpen(true);
    };

    const handleUpdate = (categoryId: string) => {
        const category = categoryList.find((category: Category) => category.categoryId === categoryId);
        if (category) {
            setSelectedProp(category);
            setUpdate(true);
        }
    };

    const handleDelete = (categoryId: string) => {
        const category = categoryList.find((category: Category) => category.categoryId === categoryId);
        if (category) {
            setSelectedProp(category);
            setDeletePopupOpen(true);
        }
    };

    return (
        <>
            <NavBar />
            <Container maxWidth="md">
                <Grid container spacing={2}>
                    <Grid size={8}>
                        <h3>Your Categories</h3>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, md: 4, lg: 3}}>
                        {user && (
                            <IconButton onClick={handleCreate} sx={{ all: 'unset', cursor: 'pointer', marginTop: '15px' }}>
                                <AddIcon />
                                <span>Create Category</span>
                            </IconButton>
                        )}
                    </Grid>
                    {categoryNodeList}
                </Grid>
            </Container>
        </>
    )
}

export default UserCategoryList;