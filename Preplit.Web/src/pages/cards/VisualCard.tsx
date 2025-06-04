import React, { useState } from 'react';
import { Card, CardContent, IconButton } from '@mui/material'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import CloseOutlinedIcon from '@mui/icons-material/CloseOutlined';
import Popup from "reactjs-popup";
import type { FlashCard } from '../../../types/FlashCard';
import UpdateCard from './UpdateCard';
import DeleteCard from './DeleteCard';

const VisualCard = ({card}: {card: FlashCard}) => {
    const [flip, setFlip] = useState(false);
    const [deletePopupOpen, setDeletePopupOpen] = useState(false);
    const [updatePopupOpen, setUpdatePopupOpen] = useState(false);
    
    return (
        <>
            <Card sx={{
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    transition: 'transform 0.2s ease-in-out',
                    '&:hover': {
                        transform: 'scale(1.03)',
                        boxShadow: '0 6px 12px rgba(0, 0, 0, 0.15)'
                    },
                    '&:selected': {
                        transform: 'rotateY(0.5turn)'
                    }
                }}
                onClick={() => setFlip(!flip)}>
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
                    }}></div>
                    <IconButton onClick={() => setUpdatePopupOpen(true)}
                        style={{ position: 'absolute', top: '8px', right: '8px', 
                                background: 'none', border: 'none', color: 'black' 
                        }}
                    >
                        <EditOutlinedIcon />
                    </IconButton>
                    <IconButton onClick={() => setDeletePopupOpen(true)}
                        style={{ position: 'absolute', top: '8px', left: '8px', 
                                background: 'none', border: 'none', color: 'black' 
                        }}
                    >
                        <DeleteOutlinedIcon />
                    </IconButton>
                    {flip ? <h2 style={{transform: 'none'}}>{card.answer}</h2> : <h2 style={{transform: 'none'}}>{card.question}</h2>}
                </CardContent>
            </Card>
            <Popup
                open={updatePopupOpen}
                closeOnDocumentClick
                onClose={() => setUpdatePopupOpen(false)}
                modal
                nested
                overlayStyle={{
                    background: "rgba(0, 0, 0, 0.5)",
                }}
                contentStyle={{
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
                {card && (
                <div>
                    <IconButton
                    onClick={() => setUpdatePopupOpen(false)}
                    style={{
                        position: "absolute",
                        top: "10px",
                        right: "10px",
                        background: "none",
                        border: "none",
                        fontSize: "24px",
                        cursor: "pointer",
                        color: "black",
                    }}
                    >
                        <CloseOutlinedIcon />
                    </IconButton>
                    <UpdateCard card={card} />
                </div>
                )}
                
            </Popup> 
            <Popup
                open={deletePopupOpen}
                closeOnDocumentClick
                onClose={() => setDeletePopupOpen(false)}
                modal
                nested
                overlayStyle={{
                    background: "rgba(0, 0, 0, 0.5)",
                }}
                contentStyle={{
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
            {card && (
                <DeleteCard card={card} />
            )}
            </Popup> 
        </>
    );
}

export default VisualCard;