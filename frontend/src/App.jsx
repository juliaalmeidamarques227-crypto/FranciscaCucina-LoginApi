import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Admin from './pages/Admin'
import CadastroUsuario from './pages/CadastroUsuario'
import ProtectedRoute from './components/ProtectedRoute'
import AdminRoute from './components/AdminRoute'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login />} />

        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />

        <Route
          path="/admin"
          element={
            <AdminRoute>
              <Admin />
            </AdminRoute>
          }
        />

        <Route
          path="/admin/cadastro"
          element={
            <AdminRoute>
              <CadastroUsuario />
            </AdminRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  )
}

export default App