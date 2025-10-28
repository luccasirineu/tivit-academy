const form = document.getElementById('loginForm');

const toggleSenha = document.getElementById('toggleSenha');
const inputSenha = document.getElementById('senha');

toggleSenha.addEventListener('click', () => {
  const isPassword = inputSenha.type === 'password';
  inputSenha.type = isPassword ? 'text' : 'password';
  toggleSenha.classList.toggle('bx-show', isPassword);
  toggleSenha.classList.toggle('bx-hide', !isPassword);
});

form.addEventListener('submit', (e) => {
  e.preventDefault();
  
  const tipo = document.getElementById('tipo').value;
  const usuario = document.getElementById('usuario').value;
  const senhaValor = document.getElementById('senha').value;

  if (!tipo || !usuario || !senhaValor) {
    alert('Por favor, preencha todos os campos.');
    return;
  }

  if (tipo === "administrador" && usuario === "admin123" && senhaValor === "admin123") {
    window.location.href = "admin/admin.html"; 
  }

  if (tipo === "professor" && usuario === "prof123" && senhaValor === "prof123") {
    window.location.href = "professor/professor.html"; 
  }

  if (tipo === "aluno" && usuario === "aluno123" && senhaValor === "aluno123") {
    window.location.href = "aluno/aluno.html"; 
  }

});
