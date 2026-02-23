const form = document.getElementById('loginForm');

const toggleSenha = document.getElementById('toggleSenha');
const inputSenha = document.getElementById('senha');

toggleSenha.addEventListener('click', () => {
  const isPassword = inputSenha.type === 'password';
  inputSenha.type = isPassword ? 'text' : 'password';
  toggleSenha.classList.toggle('bx-show', isPassword);
  toggleSenha.classList.toggle('bx-hide', !isPassword);
});

form.addEventListener('submit', async (e) => {
  e.preventDefault();

  const tipo = document.getElementById('tipo').value;
  const usuario = document.getElementById('usuario').value;
  const senha = document.getElementById('senha').value;

  if (!tipo || !usuario || !senha) {
    alert('Por favor, preencha todos os campos.');
    return;
  }

  try {
    const response = await fetch('http://localhost:5027/api/Login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        Tipo: tipo,
        Cpf: usuario,
        Senha: senha
      })
    });

    if (!response.ok) {
      const erro = await response.json();
      alert(erro.mensagem || 'Erro ao realizar login');
      return;
    }

    const data = await response.json();

    localStorage.setItem('usuarioLogado', JSON.stringify({
      id: data.id,          
      nome: data.nome,
      tipo: data.tipo
    }));

    if (data.token) {
      localStorage.setItem('token', data.token);
    }

    if (data.tipo === 'administrador') {
      window.location.href = 'admin/admin.html';
    } else if (data.tipo === 'professor') {
      window.location.href = 'professor/professor.html';
    } else if (data.tipo === 'aluno') {
      window.location.href = 'aluno/aluno.html';
    }

  } catch (error) {
    console.error(error);
    alert('Erro de conexão com o servidor');
  }
});

