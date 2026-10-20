import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'

function Admin() {
  const navigate = useNavigate()

  const [mensagem, setMensagem] = useState('')
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    async function carregarAreaAdmin() {
      try {
        const response = await api.get('/api/Usuarios/admin')
        setMensagem(response.data)
      } catch (error) {
        setMensagem('Não foi possível carregar a área administrativa.')
      } finally {
        setCarregando(false)
      }
    }

    carregarAreaAdmin()
  }, [])

  function sair() {
    localStorage.removeItem('token')
    navigate('/')
  }

  function voltarDashboard() {
    navigate('/dashboard')
  }

  function abrirCadastro() {
    navigate('/admin/cadastro')
  }

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

            <div
              className="menu-item"
              onClick={voltarDashboard}
            >
              Início
            </div>

            <div className="menu-item active">
              Administração
            </div>
          </nav>
        </div>

        <button onClick={sair} className="logout-button">
          Sair
        </button>
      </aside>

      <section className="dashboard-main">
        <header className="dashboard-header">
          <div>
            <span className="dashboard-label">
              ADMINISTRAÇÃO
            </span>

            <h1>Área administrativa</h1>
          </div>

          <div className="user-badge">
            <div className="user-avatar">
              A
            </div>

            <div>
              <strong>Administrador</strong>
              <span>Acesso autorizado</span>
            </div>
          </div>
        </header>

        {carregando ? (
          <section className="welcome-card">
            <div className="welcome-content">
              <span className="dashboard-label">
                ADMINISTRAÇÃO
              </span>

              <h2>Carregando...</h2>

              <p>
                Verificando o acesso à área administrativa.
              </p>
            </div>
          </section>
        ) : (
          <section className="welcome-card">
            <div className="welcome-content">
              <span className="dashboard-label">
                ACESSO AUTORIZADO
              </span>

              <h2>Área administrativa</h2>

              <p>{mensagem}</p>

              <button
                onClick={abrirCadastro}
                className="admin-action-button"
              >
                Cadastrar usuário
              </button>
            </div>

            <div className="welcome-mark">
              ADM
            </div>
          </section>
        )}
      </section>
    </main>
  )
}

export default Admin