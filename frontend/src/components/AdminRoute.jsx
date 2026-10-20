import { Navigate } from 'react-router-dom'

function lerPerfilDoToken(token) {
  try {
    const parte = token.split('.')[1]
    const base64 = parte.replace(/-/g, '+').replace(/_/g, '/')
    const dados = JSON.parse(atob(base64))

    return (
      dados.role ||
      dados[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ]
    )
  } catch (error) {
    return null
  }
}

function AdminRoute({ children }) {
  const token = localStorage.getItem('token')

  if (!token) {
    return <Navigate to="/" replace />
  }

  const perfil = lerPerfilDoToken(token)

  if (perfil !== 'Admin') {
    return <Navigate to="/dashboard" replace />
  }

  return children
}

export default AdminRoute