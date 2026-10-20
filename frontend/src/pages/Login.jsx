import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'
import '../App.css'

function Login() {
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [mensagem, setMensagem] = useState('')
  const [carregando, setCarregando] = useState(false)

  async function fazerLogin(event) {
    event.preventDefault()

    setMensagem('')

    if (!email.trim() || !senha.trim()) {
      setMensagem('Preencha o e-mail e a senha.')
      return
    }

    setCarregando(true)

    try {
      const response = await api.post('/api/Usuarios/login', {
        email: email.trim(),
        senha
      })

      localStorage.setItem('token', response.data.token)

      navigate('/dashboard')
    } catch (error) {
      if (error.response?.status === 404) {
        setMensagem('Usuário não encontrado.')
      } else if (error.response?.status === 401) {
        setMensagem('E-mail ou senha incorretos.')
      } else {
        setMensagem('Não foi possível realizar o login.')
      }
    } finally {
      setCarregando(false)
    }
  }

  return (
    <main className="login-page">
      <section className="login-brand">
        <div className="brand-content">
          <span className="brand-name">Francisca Cucina</span>

          <h1>
            Bem-vindo
            <br />
            de volta.
          </h1>

          <p>
            Acesse o sistema para continuar.
          </p>
        </div>
      </section>

      <section className="login-area">
        <div className="login-box">
          <div className="login-header">
            <p className="login-label">ACESSO</p>

            <h2>Entrar no sistema</h2>

            <p>
              Informe seus dados para continuar.
            </p>
          </div>

          <form onSubmit={fazerLogin}>
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
                placeholder="Digite sua senha"
                value={senha}
                onChange={(event) => setSenha(event.target.value)}
                autoComplete="current-password"
                disabled={carregando}
              />
            </div>

            <button
              type="submit"
              disabled={carregando}
            >
              {carregando ? 'Entrando...' : 'Entrar'}
            </button>

            {mensagem && (
              <p className="login-message">
                {mensagem}
              </p>
            )}
          </form>
        </div>
      </section>
    </main>
  )
}

export default Login