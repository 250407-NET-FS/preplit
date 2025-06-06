import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import './App.css'
// import Context Providers
import { AuthProvider } from './pages/contexts/AuthContext'
import { CategoryProvider } from './pages/contexts/CategoryContext'
import { CardProvider } from './pages/contexts/CardContext'
// Import Admin Components
import RequireAdmin from './pages/admin/RequireAdmin'
import Dashboard from './pages/admin/Dashboard'
import UserList from './pages/admin/UserList'
import CategoryList from './pages/admin/CategoryList'
import CardList from './pages/admin/CardList'
import Home from './pages/Home'
import UserCategoryList from './pages/categories/UserCategoryList'
import Register from './Register'
import React from 'react'

function App() {
  return (
    <AuthProvider>
      <CategoryProvider>
        <CardProvider>
          <Router>
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/admin" element={<RequireAdmin><Dashboard /></RequireAdmin>} >
                <Route index element={<RequireAdmin><Dashboard /></RequireAdmin>} />
                <Route path="/admin/user-list" element={<UserList />} />
                <Route path="/admin/category-List" element={<CategoryList />} />
                <Route path="/admin/card-list" element={<CardList />} />
              </Route>
              <Route path="/categories" element={<UserCategoryList />} />
              <Route path="/register" element={<Register />} />
            </Routes>
          </Router>
        </CardProvider>
      </CategoryProvider>
    </AuthProvider>
  )
}

export default App
