import { useEffect } from "react";
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
        fetchCategoryList();
    }, [fetchCategoryList]);

    let length = (categoryList.length > 6)? 6 : categoryList.length;

    if (!user) return <div>Please login or register to start using Preplit!</div>

    return (
        <div className="page">
            <NavBar />
            <section className="home">
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
                            border: '10px solid #ccc',
                            borderRadius: '10px',
                            gap: '15px',
                            backgroundColor: '#ccc',
                        }}>
                            {categoryList
                                .slice(0, length)
                                .map((category: Category) => (
                                    <Grid size={6}>
                                        <div style={{flex: '1 0 20%', boxSizing: 'border-box'}}>
                                            <CategoryCard key={category.categoryId} category={category} />
                                        </div>
                                    </Grid>
                                ))}
                        </Grid>
                    </Container> 
                </div>
            </main>             
        </div>
    )
}
