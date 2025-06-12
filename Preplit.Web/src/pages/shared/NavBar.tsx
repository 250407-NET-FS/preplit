import React from "react";
import logo from "../../assets/logo.png";
import { Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext"
import Login from "../Login";
import Popup from "reactjs-popup";
import { AppBar, Container, Toolbar, Box, Button, IconButton, Typography, Divider, List, ListItem, ListItemButton, ListItemText, Drawer } from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';

export default function NavBar() {
    const pages = [
        { name: "Categories", path: "/categories" },
    ];
    const { user, logout } = useAuth();

    const [open, setOpen] = React.useState(false);
    const handleDrawerToggle = () => {
        setOpen(!open);
    };

  const drawer = (
    <Box onClick={handleDrawerToggle} sx={{ textAlign: 'center', backgroundColor: "#4f86f7" }}>
      <Typography variant="h5" sx={{ my: 2 }} component={Link} to="/" className="logo-text">
        <strong>Preplit</strong>
      </Typography>
      <Divider />
      <List>
        {pages.map((item) => (
          <ListItem key={item.name} disablePadding>
            <ListItemButton sx={{ textAlign: 'center', color: "white" }}                                     
                            key={item.name}
                            component={Link}
                            to={item.path}
                            disableRipple
            >
              <ListItemText primary={item.name} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  );

    const container = window !== undefined ? () => window.document.body : undefined;

    return (
        <>
            <AppBar position="static" color="transparent" sx={{ backgroundColor: "#4f86f7", width: "100vw" }}>
                <Container maxWidth={false} disableGutters>
                    <Toolbar>
                        <IconButton component={Link} to="/" disableRipple>
                            <img src={logo} alt="Preplit Logo" className="logo-image" />
                        </IconButton>
                        <Typography
                            variant="h4"
                            component={Link}
                            to="/"
                            className="logo-text"
                        >
                            <strong>Preplit</strong>
                        </Typography>
                        <IconButton
                            color="inherit"
                            aria-label="open drawer"
                            edge="start"
                            onClick={handleDrawerToggle}
                            sx={{ color: "white", position: "relative", marginLeft: "auto", display: { sm: 'flex', md: 'none', lg: 'none', xl: 'none' } }}
                        >
                            <MenuIcon />
                        </IconButton>
                        <Box sx={{ flexGrow: 1, marginLeft: "1.5rem", display: { xs: 'none', sm: 'none', md: 'flex', lg: 'flex', xl: 'flex' } }}>
                            {pages.map((page) => (
                                <Button
                                    key={page.name}
                                    component={Link}
                                    to={page.path}
                                    disableRipple
                                    sx={{ color: "white" }}
                                >
                                    {page.name}
                                </Button>
                            ))}
                        </Box>
                        <Box sx={{ marginRight: "1rem", display: { xs: 'none', sm: 'none', md: 'flex', lg: 'flex', xl: 'flex' } }}>
                            <Typography sx={{ color: "white" }}>{user ? `Welcome, ${user.fullName}` : "Welcome, Guest"}</Typography>
                        </Box>
                        <Box sx={{ display: { xs: 'none', sm: 'none', md: 'flex', lg: 'flex', xl: 'flex' } }}>
                            {user?.role === "Admin" && (
                                <Button component={Link} to="/admin" sx={{ color: 'white' }}>
                                    Admin Dashboard
                                </Button>
                            )}
                            {user?.fullName ? (
                                <Button component={Link} onClick={logout} sx={{ color: 'white' }} to="/">Logout</Button>
                            ) : (
                                <Popup
                                    trigger={<Button sx={{ color: 'white' }}>Login</Button>}
                                    modal
                                    nested
                                    overlayStyle={{ background: "rgba(0, 0, 0, 0.5)" }}
                                    contentStyle={{
                                        backgroundColor: "#f8f9fa",
                                        borderRadius: "10px",
                                        padding: "30px",
                                        maxWidth: "450px",
                                        margin: "100px auto",
                                        boxShadow: "0px 4px 12px rgba(0, 0, 0, 0.2)",
                                        fontFamily: "Arial, sans-serif",
                                    }}
                                >
                                    <div className="Login-form">
                                        <Login/>
                                    </div>
                                </Popup>
                            )}
                        </Box>
                    </Toolbar>
                </Container>
            </AppBar>
            <nav>
                <Drawer
                container={container}
                variant="temporary"
                open={open}
                onClose={handleDrawerToggle}
                ModalProps={{
                    keepMounted: true, // Better open performance on mobile.
                }}
                sx={{
                    display: { xs: 'block', sm: 'block', md: 'none', lg: 'none', xl: 'none' },
                    '& .MuiDrawer-paper': { boxSizing: 'border-box', height: "auto" },
                }}
                >
                    {drawer}
                </Drawer>
            </nav>
        </>
    );
}
