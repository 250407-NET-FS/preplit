import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
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

function App() {
  return (
    <AuthProvider>
      <CategoryProvider>
        <CardProvider>
          <Router>
            <Routes>
              {/*<Route path="/" element={<UserCategoryList />} /> */}
              <Route path="/admin/dashboard" element={<RequireAdmin><Dashboard /></RequireAdmin>} >
                <Route index element={<UserList />} />
                <Route path="UserList" element={<UserList />} />
                <Route path="CategoryList" element={<CategoryList />} />
                <Route path="CardList" element={<CardList />} />
              </Route>
            </Routes>
          </Router>
        </CardProvider>
      </CategoryProvider>
    </AuthProvider>
  )
}

export default App
