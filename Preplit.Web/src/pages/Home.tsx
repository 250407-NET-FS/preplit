import React, { useEffect } from "react";
import NavBar from "./shared/NavBar";
import { useAuth } from "./contexts/AuthContext";
import { useCategory } from "./contexts/CategoryContext";
import CategoryCard from "./categories/CategoryCard";
import "../App.css";
import type { Category } from "../../types/Category";
import { Container, Grid } from "@mui/material";

export default function Home() {
    const { user } = useAuth();
    const { categoryList, fetchCategoryList } = useCategory();

    useEffect(() => {
        if (!user) return;
        const controller = new AbortController();
        fetchCategoryList(controller.signal);
        return () => controller.abort();
    }, [fetchCategoryList]);

    let length = (categoryList.length > 6)? 6 : categoryList.length;

    if (!user) return (
        <div className="page">
            <NavBar />
            <section className="home">
                <div>Please login or register to start using Preplit!</div>
            </section>
        </div>
    );

    return (
        <div className="page">
            <NavBar />
            <section className="home" style={{ textAlign: "center" }}>
                <h1 className="hero-title">Welcome to Preplit</h1>
                <h2 className="hero-subtitle">Create the Study Guide that Fits You</h2>
            </section>

            <main className="categories">
                <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    marginBottom: '15px'
                }}>

                    <Container>
                        <Grid container style={{
                            display: "flex",
                            justifyContent: 'center',
                            border: '5px solid rgb(79,134,247)',
                            borderRadius: '10px',
                            gap: '15px',
                            padding: '15px'
                        }}>
                            {categoryList
                                .slice(0, length)
                                .map((category: Category) => (
                                    <Grid size={6} key={category.categoryId}>
                                        <div style={{flex: '1 0 20%', boxSizing: 'border-box'}}>
                                            <CategoryCard key={category.categoryId} category={category} />
                                        </div>
                                    </Grid>
                                ))}
                        </Grid>
                    </Container> 
                </div>
            </main>        

            <footer className="footer">
                <p>© 2025 Preplit · All rights reserved.</p>
            </footer>     
        </div>
    )
}
