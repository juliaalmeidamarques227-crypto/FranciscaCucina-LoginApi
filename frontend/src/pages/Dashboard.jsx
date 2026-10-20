import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'

function lerUsuarioDoToken(token) {
  try {
    const parte = token.split('.')[1]
    const base64 = parte.replace(/-/g, '+').replace(/_/g, '/')

    const bytes = Uint8Array.from(atob(base64), (caractere) =>
      caractere.charCodeAt(0)
    )

    return JSON.parse(new TextDecoder().decode(bytes))
  } catch (error) {
    return null
  }
}

function Dashboard() {
  const navigate = useNavigate()

  const [usuario, setUsuario] = useState(null)
  const [mensagem, setMensagem] = useState('')
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    async function carregarDashboard() {
      const token = localStorage.getItem('token')

      if (!token) {
        navigate('/')
        return
      }

      const dadosUsuario = lerUsuarioDoToken(token)

      if (dadosUsuario) {
        setUsuario({
          nome:
            dadosUsuario.name ||
            dadosUsuario[
              'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
            ],
          email:
            dadosUsuario.email ||
            dadosUsuario[
              'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
            ],
          perfil:
            dadosUsuario.role ||
            dadosUsuario[
              'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
            ]
        })
      }

      try {
        const response = await api.get('/api/Usuarios/protegido')
        setMensagem(response.data)
      } catch (error) {
        setMensagem('Não foi possível acessar o sistema.')
      } finally {
        setCarregando(false)
      }
    }

    carregarDashboard()
  }, [navigate])

  function sair() {
    localStorage.removeItem('token')
    navigate('/')
  }

  function irParaAdmin() {
    navigate('/admin')
  }

  const primeiraLetra = usuario?.nome?.charAt(0).toUpperCase() || 'U'

  return (
    <main className="dashboard-page">
      <aside className="sidebar">
        <div>
          <div className="sidebar-brand">
            <div className="brand-symbol">FC</div>

            <div className="sidebar-brand-text">
              <strong>Francisca</strong>
              <span>Cucina</span>
            </div>
          </div>

          <nav className="sidebar-menu">
            <span className="menu-title">MENU</span>

            <div className="menu-item active">
              Início
            </div>

            {usuario?.perfil === 'Admin' && (
              <div
                className="menu-item"
                onClick={irParaAdmin}
              >
                Administração
              </div>
            )}
          </nav>
        </div>

        <button onClick={sair} className="logout-button">
          Sair
        </button>
      </aside>

      <section className="dashboard-main">
        <header className="dashboard-header">
          <div>
            <span className="dashboard-label">PAINEL</span>
            <h1>Dashboard</h1>
          </div>

          <div className="user-badge">
            <div className="user-avatar">
              {primeiraLetra}
            </div>

            <div>
              <strong>{usuario?.nome || 'Usuário'}</strong>
              <span>{usuario?.perfil || 'Usuário'}</span>
            </div>
          </div>
        </header>

        <section className="welcome-card">
          <div className="welcome-content">
            <span className="dashboard-label">
              BEM-VINDA DE VOLTA
            </span>

            <h2>
              Olá, {usuario?.nome || 'usuário'}.
            </h2>

            {carregando ? (
              <p>Verificando seu acesso...</p>
            ) : (
              <p>{mensagem}</p>
            )}
          </div>

          <div className="welcome-mark">
            FC
          </div>
        </section>

        <section className="dashboard-section">
          <div className="section-header">
            <span className="dashboard-label">SUA CONTA</span>
            <h2>Informações de acesso</h2>
          </div>

          <div className="info-grid">
            <article className="info-card">
              <span className="info-number">01</span>

              <div>
                <h3>Nome</h3>
                <p>{usuario?.nome || 'Não informado'}</p>
              </div>
            </article>

            <article className="info-card">
              <span className="info-number">02</span>

              <div>
                <h3>E-mail</h3>
                <p>{usuario?.email || 'Não informado'}</p>
              </div>
            </article>

            <article className="info-card">
              <span className="info-number">03</span>

              <div>
                <h3>Perfil</h3>
                <p>{usuario?.perfil || 'Não informado'}</p>
              </div>
            </article>
          </div>
        </section>
      </section>
    </main>
  )
}

export default Dashboard