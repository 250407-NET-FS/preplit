import { useState } from 'react';
import { Card, CardContent } from '@mui/material'
import type { FlashCard } from '../../../types/FlashCard';

const VisualCard = ({card}: {card: FlashCard}) => {
    const [flip, setFlip] = useState(false);
    
    return (
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
                {flip ? <h2 style={{transform: 'none'}}>{card.answer}</h2> : <h2 style={{transform: 'none'}}>{card.question}</h2>}
            </CardContent>
        </Card>
    );
}

export default VisualCard;