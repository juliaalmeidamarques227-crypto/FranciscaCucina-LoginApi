import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'

function CadastroUsuario() {
  const navigate = useNavigate()

  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [mensagem, setMensagem] = useState('')
  const [carregando, setCarregando] = useState(false)

  async function cadastrarUsuario(event) {
    event.preventDefault()

    setMensagem('')
    setCarregando(true)

    try {
      await api.post('/api/Usuarios', {
        nome: nome.trim(),
        email: email.trim(),
        senha
      })

      setMensagem('Usuário cadastrado com sucesso!')

      setNome('')
      setEmail('')
      setSenha('')
    } catch (error) {
      if (error.response?.status === 409) {
        setMensagem('Este e-mail já está cadastrado.')
      } else if (error.response?.status === 400) {
        setMensagem('Confira os dados informados.')
      } else if (error.response?.status === 401) {
        setMensagem('Sua sessão não é válida.')
      } else if (error.response?.status === 403) {
        setMensagem('Você não tem permissão para cadastrar usuários.')
      } else {
        setMensagem('Não foi possível cadastrar o usuário.')
      }
    } finally {
      setCarregando(false)
    }
  }

  function sair() {
    localStorage.removeItem('token')
    navigate('/')
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
            <span className="menu-title">ADMINISTRAÇÃO</span>

            <div
              className="menu-item"
              onClick={() => navigate('/dashboard')}
            >
              Início
            </div>

            <div
              className="menu-item active"
              onClick={() => navigate('/admin')}
            >
              Usuários
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

            <h1>Novo usuário</h1>
          </div>
        </header>

        <section className="form-card">
          <div className="form-header">
            <span className="dashboard-label">
              CADASTRO
            </span>

            <h2>Cadastrar usuário</h2>

            <p>
              Crie um novo acesso para o sistema.
            </p>
          </div>

          <form onSubmit={cadastrarUsuario}>
            <div className="input-group">
              <label htmlFor="nome">
                Nome
              </label>

              <input
                type="text"
                id="nome"
                name="nome"
                placeholder="Digite o nome completo"
                value={nome}
                onChange={(event) => setNome(event.target.value)}
                autoComplete="name"
                disabled={carregando}
                required
              />
            </div>

            <div className="input-group">
              <label htmlFor="email">
                E-mail
              </label>

              <input
                type="email"
                id="email"
                name="email"
                placeholder="seuemail@exemplo.com"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                autoComplete="email"
                disabled={carregando}
                required
              />
            </div>

            <div className="input-group">
              <label htmlFor="senha">
                Senha
              </label>

              <input
                type="password"
                id="senha"
                name="senha"
                placeholder="Digite uma senha"
                value={senha}
                onChange={(event) => setSenha(event.target.value)}
                autoComplete="new-password"
                disabled={carregando}
                required
              />
            </div>

            <div className="form-actions">
              <button
                type="button"
                className="secondary-button"
                onClick={() => navigate('/admin')}
                disabled={carregando}
              >
                Cancelar
              </button>

              <button
                type="submit"
                className="primary-button"
                disabled={carregando}
              >
                {carregando
                  ? 'Cadastrando...'
                  : 'Cadastrar usuário'}
              </button>
            </div>

            {mensagem && (
              <p className="form-message">
                {mensagem}
              </p>
            )}
          </form>
        </section>
      </section>
    </main>
  )
}

export default CadastroUsuario