import { Container, Grid, IconButton } from "@mui/material";
import AddIcon from '@mui/icons-material/Add';
import { useCategory } from "../contexts/CategoryContext";
import { useAuth } from "../contexts/AuthContext";
import React, { useEffect, useState, type JSX } from "react";
import Popup from "reactjs-popup";
//import CreateCategory from "./CreateCategory";
import NavBar from "../shared/NavBar";
import CategoryCard from "./CategoryCard";
import CreateCategory from "./CreateCategory";
import type { Category } from "../../../types/Category";

function UserCategoryList() {
    const { user } = useAuth();
    const { categoryList, fetchCategoryList } = useCategory();
    const controller = new AbortController();

    const [createPopupOpen, setCreatePopupOpen] = useState(false);

    useEffect(() => {
        fetchCategoryList(controller.signal);
    }, [fetchCategoryList]);

    let categoryNodeList: JSX.Element[] = categoryList.map((category: Category) => (
        <Grid size={{ xs: 12, sm: 6, md: 4, lg: 3}} key={category.categoryId}>
            <CategoryCard category={category} />
        </Grid>
    ));

    const handleCreate = () => {
        setCreatePopupOpen(true);
    };

    return (
        <div className="page">
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
                        <CreateCategory />
                    </div>
                </Popup>
            </Container>
        </div>
    )
}

export default UserCategoryList;