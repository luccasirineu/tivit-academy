(function(){
  emailjs.init("uXff3S5w8KeovuO4C");
})();

// ===== Alternar entre seções =====
const menuItems = document.querySelectorAll(".sidebar nav ul li");
const dashboardButtons = document.querySelectorAll(".dashboard-btn");
const sections = document.querySelectorAll("main section");

function showSection(sectionId) {
  menuItems.forEach(i => i.classList.remove("active"));
  sections.forEach(sec => sec.classList.remove("active"));
  document.getElementById(sectionId).classList.add("active");

  const menuItem = [...menuItems].find(i => i.dataset.section === sectionId);
  if (menuItem) menuItem.classList.add("active");
}

// Clicar no menu lateral
menuItems.forEach(item => {
  item.addEventListener("click", () => showSection(item.dataset.section));
});

// Clicar nos botões do dashboard
dashboardButtons.forEach(btn => {
  btn.addEventListener("click", () => showSection(btn.dataset.section));
});


// ===== Alternar tema claro/escuro =====
const themeSwitch = document.getElementById("themeSwitch");
themeSwitch.addEventListener("change", () => {
  document.body.classList.toggle("light", themeSwitch.checked);
});

// ===== Cadastro e listagem =====
const form = document.getElementById("cadastroForm");
const tabelaBody = document.getElementById("tabelaBody");
const filtro = document.getElementById("filtro");

//confirma cadastro
form.addEventListener("submit", e => {
  e.preventDefault();

  const tipo = document.getElementById("tipo").value;
  const nome = document.getElementById("nome").value.trim();
  const email = document.getElementById("email").value.trim();
  const senha = document.getElementById("senha").value.trim();
  const curso = document.getElementById("curso").value.trim();

  if (!tipo || !nome || !email || !senha || !curso) {
    alert("Preencha todos os campos!");
    return;
  }

  if (!email.includes("@")) {
    alert("E-mail inválido!");
    return;
  }

  const params = { tipo, nome, email, senha, curso };

  // temporario enquanto ainda nao tem o back
  const row = document.createElement("tr");
  row.innerHTML = `<td>${tipo}</td><td>${nome}</td><td>${email}</td><td>${curso}</td>`;
  tabelaBody.appendChild(row);

  const botao = form.querySelector(".btn");
  botao.disabled = true;
  botao.textContent = "Enviando...";

  emailjs.send("tivit_academy", "template_zdwi0zn", params)
    .then(() => {
      alert("Cadastro realizado com sucesso! E-mail enviado ao usuário.");
      form.reset();
    })
    .catch((error) => {
      console.error("Erro ao enviar e-mail:", error);
      alert("Cadastro realizado, mas não foi possível enviar o e-mail.");
    })
    .finally(() => {
      botao.disabled = false;
      botao.textContent = "Cadastrar";
    });
});

// ===== Filtro =====
filtro.addEventListener("input", () => {
  const valor = filtro.value.toLowerCase();
  const linhas = tabelaBody.querySelectorAll("tr");

  linhas.forEach(linha => {
    const nome = linha.children[1].textContent.toLowerCase();
    linha.style.display = nome.includes(valor) ? "" : "none";
  });
});

// ===== Mostrar/Ocultar Senha =====
const toggleSenha = document.getElementById('toggleSenha');
const inputSenha = document.getElementById('senha');

toggleSenha.addEventListener('click', () => {
  const isPassword = inputSenha.type === 'password';
  inputSenha.type = isPassword ? 'text' : 'password';
  toggleSenha.classList.toggle('bx-show', isPassword);
  toggleSenha.classList.toggle('bx-hide', !isPassword);
});


