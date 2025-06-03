import { useState } from 'react';
import { Card, CardContent } from '@mui/material'
// import CardList from './cards/CategoryCardList';
import Popup from "reactjs-popup";
import type { Category } from '../../../types/Category';

const CategoryCard = ({ category }: { category: Category }) => {
    const [detailPopupOpen, setDetailPopupOpen] = useState(false);


    return (
        <>
            <Card
                sx={{
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    transition: 'transform 0.2s ease-in-out',
                    '&:hover': {
                        transform: 'scale(1.03)',
                        boxShadow: '0 6px 12px rgba(0, 0, 0, 0.15)'
                    }
                }}
                onClick={() => setDetailPopupOpen(true)}
            >
                <CardContent sx={{
                    flexGrow: 1,
                    display: 'flex',
                    flexDirection: 'column',
                    padding: 2
                }}>
                    <div style={{
                        marginBottom: '12px',
                        width: '100%',
                        height: '180px',
                        overflow: 'hidden',
                        borderRadius: '8px'
                    }}>
                    </div>
                    <h3 style={{ margin: '0 0 8px 0' }}>{category.name}</h3>
                </CardContent>
            </Card>
            <Popup
                open={detailPopupOpen}
                closeOnDocumentClick
                onClose={() => setDetailPopupOpen(false)}
                modal
                nested
                overlayStyle={{
                    background: "rgba(0, 0, 0, 0.5)",
                }}
                contentStyle={{
                    backgroundColor: "#f8f9fa",
                    borderRadius: "10px",
                    padding: "30px",
                    maxWidth: "800px",
                    width: "90%",
                    height: '80vh',
                    margin: "auto",
                    boxShadow: "0px 4px 12px rgba(0, 0, 0, 0.2)",
                    fontFamily: "Arial, sans-serif",
                    position: 'relative',
                    overflowY: 'auto',
                }}
            >
                {(
                    <div>
                        <button
                            onClick={() => setDetailPopupOpen(false)}
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
                        {/*<CategoryCardList category={category} /> */}
                    </div>
                )}
            </Popup>
        </>
    );
};

export default CategoryCard;