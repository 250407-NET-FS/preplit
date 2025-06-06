import React, { useState } from 'react';
import { useCategory } from '../contexts/CategoryContext';
import { Card, CardContent, FormControl, FormGroup, IconButton, Input } from '@mui/material'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import CheckOutlinedIcon from '@mui/icons-material/CheckOutlined';
import CloseOutlinedIcon from '@mui/icons-material/CloseOutlined';
import UserCardList from '../cards/UserCardList';
import Popup from "reactjs-popup";
import type { Category } from '../../../types/Category';
import DeleteCategory from './DeleteCategory';

const CategoryCard = ({ category }: { category: Category }) => {
    const {updateCategory} = useCategory();
    const [detailPopupOpen, setDetailPopupOpen] = useState(false);
    const [update, setUpdate] = useState(false);
    const [deletePopupOpen, setDeletePopupOpen] = useState(false);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    const [updateInfo, setUpdateInfo] = useState({
        categoryId: category.categoryId,
        name: category.name,
        userId: category.userId
    });

    const controller = new AbortController();

    const handleUpdate = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            await updateCategory(updateInfo, controller.signal);
            setSuccessMessage('Category updated successfully!');
            setErrorMessage(null);
            setUpdate(false);
        }
        catch (errorMessage: unknown) {
            setErrorMessage(errorMessage as string);
            setSuccessMessage(null);
            return;
        }
    };

    const handleCloseUpdate = () => {
        setUpdate(false);
        setErrorMessage(null);
        setSuccessMessage(null);
        // Set back to default values
        setUpdateInfo({
            categoryId: category.categoryId,
            name: category.name,
            userId: category.userId
        });
    };

    return (
        <>
           {/* Alert Messages  for Update Operations*/}
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
                    { !update ?
                    <>
                        <IconButton onClick={() => setUpdate(true)} 
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
                        <h3 style={{ margin: '0 0 8px 0' }}>{category.name}</h3>
                    </> :
                    <form onSubmit={handleUpdate}>
                        <FormGroup>
                            <FormControl>
                                <Input
                                    type="text"
                                    value={updateInfo.name}
                                    onChange={(e) => setUpdateInfo({ ...updateInfo, name: e.target.value })}
                                />
                            </FormControl>
                            <FormGroup>
                                <Input type="submit" 
                                    value={<CheckOutlinedIcon></CheckOutlinedIcon>} 
                                    color='primary'
                                />
                                <Input type="button" 
                                    value={<CloseOutlinedIcon></CloseOutlinedIcon>} 
                                    color='secondary' onClick={handleCloseUpdate} />
                            </FormGroup>
                        </FormGroup>
                    </form>
                    }     
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
                        <UserCardList category={category} />
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
                <DeleteCategory id={category.categoryId} />
            </Popup>
        </>
    );
};

export default CategoryCard;